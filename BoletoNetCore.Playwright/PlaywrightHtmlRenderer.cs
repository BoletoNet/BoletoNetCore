using Microsoft.Playwright;
using BoletoNetCore.Playwright.Internal;

namespace BoletoNetCore
{
    /// <summary>
    /// Implementação baseada em Playwright para renderização HTML de boletos.
    /// Utiliza navegador Chromium headless para geração de saída em alta fidelidade com suporte ARM64.
    /// Suporta formatos de saída PDF, PNG, JPEG e WebP.
    /// </summary>
    /// <remarks>
    /// Esta implementação mantém um único processo de navegador e cria contextos
    /// e páginas novos por renderização para isolamento. A concorrência é controlada via semáforo
    /// para evitar esgotamento de recursos. O navegador reinicia automaticamente em caso de crash
    /// quando <see cref="PlaywrightRendererOptions.RestartOnFailure"/> está habilitado.
    /// </remarks>
    public sealed class PlaywrightHtmlRenderer : IHtmlRenderer, IDisposable
    {
        private readonly PlaywrightRendererOptions _options;
        private readonly SemaphoreSlim _concurrencyGate;
        private readonly SemaphoreSlim _initializationLock = new(1, 1);

        private IPlaywright? _playwright;
        private BrowserManager? _browserManager;
        private bool _disposed;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PlaywrightHtmlRenderer"/>.
        /// </summary>
        /// <param name="options">Opções de configuração para o renderer. Se nulo, opções padrão são utilizadas.</param>
        public PlaywrightHtmlRenderer(PlaywrightRendererOptions? options = null)
        {
            _options = options ?? new PlaywrightRendererOptions();
            _concurrencyGate = new SemaphoreSlim(_options.MaxConcurrency, _options.MaxConcurrency);
        }

        /// <summary>
        /// Inicializa a instância do navegador utilizada para renderização de forma assíncrona.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento para cancelar a operação de inicialização.</param>
        /// <returns>Uma task que representa a operação de inicialização assíncrona.</returns>
        /// <remarks>
        /// Este método é idempotente e pode ser chamado múltiplas vezes com segurança.
        /// A chamada é opcional - o navegador será inicializado de forma lazy na primeira renderização.
        /// O pré-aquecimento via este método elimina atrasos de cold-start.
        /// </remarks>
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            await _initializationLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_playwright != null && _browserManager != null)
                    return;

                // Cria instância do Playwright
                _playwright = await Microsoft.Playwright.Playwright.CreateAsync().ConfigureAwait(false);

                // Configura opções de inicialização do navegador
                var launchOptions = new BrowserTypeLaunchOptions
                {
                    ExecutablePath = _options.BrowserExecutablePath,
                    Args = _options.BrowserLaunchArgs
                };

                // Cria gerenciador do navegador
                _browserManager = new BrowserManager(
                    _playwright,
                    launchOptions);

                // Pré-aquece o navegador se configurado
                if (_options.PrewarmOnStart)
                {
                    await _browserManager.GetOrCreateBrowserAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                _initializationLock.Release();
            }
        }

        /// <summary>
        /// Renderiza conteúdo HTML para o formato de saída especificado de forma assíncrona.
        /// </summary>
        /// <param name="html">O conteúdo HTML a ser renderizado. Deve ser um documento HTML completo.</param>
        /// <param name="format">Especificação do formato de saída (PDF, PNG, JPEG ou WebP).</param>
        /// <param name="cancellationToken">Token de cancelamento para cancelar a operação de renderização.</param>
        /// <returns>Uma task que representa a operação assíncrona. O resultado contém a saída gerada como um array de bytes.</returns>
        /// <exception cref="ArgumentNullException">Lançada quando <paramref name="html"/> ou <paramref name="format"/> é nulo.</exception>
        /// <exception cref="NotSupportedException">Lançada quando o formato de saída não é suportado.</exception>
        /// <exception cref="OperationCanceledException">Lançada quando a operação é cancelada ou ocorre timeout.</exception>
        public async Task<byte[]> RenderAsync(
            string html,
            OutputFormat format,
            CancellationToken cancellationToken = default)
        {
            if (html == null)
                throw new ArgumentNullException(nameof(html));
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            ThrowIfDisposed();

            // Garante que o navegador está inicializado (inicialização lazy)
            if (_playwright == null || _browserManager == null)
            {
                await InitializeAsync(cancellationToken).ConfigureAwait(false);
            }

            // Adquire gate de concorrência
            await _concurrencyGate.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                return await RenderWithRetryAsync(html, format, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _concurrencyGate.Release();
            }
        }

        /// <summary>
        /// Renderiza HTML com lógica de retry para recuperação de crash.
        /// Só tenta novamente em falhas transitórias do navegador/Playwright, não em cancelamento ou erros de entrada.
        /// </summary>
        private async Task<byte[]> RenderWithRetryAsync(
            string html,
            OutputFormat format,
            CancellationToken cancellationToken)
        {
            try
            {
                return await RenderInternalAsync(html, format, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (PlaywrightException ex) when (_options.RestartOnFailure && IsBrowserCrashError(ex))
            {
                await _browserManager!.DisposeBrowserAsync().ConfigureAwait(false);
                return await RenderInternalAsync(html, format, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Determina se uma PlaywrightException indica crash ou desconexão do navegador.
        /// </summary>
        private static bool IsBrowserCrashError(PlaywrightException ex)
        {
            var message = ex.Message;
            return message.IndexOf("Target closed", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   message.IndexOf("Browser closed", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   message.IndexOf("browser has been closed", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   message.IndexOf("Target page, context or browser has been closed", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   message.IndexOf("Protocol error", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   message.IndexOf("Connection closed", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Implementação interna de renderização que despacha para o handler de formato apropriado.
        /// </summary>
        private async Task<byte[]> RenderInternalAsync(
            string html,
            OutputFormat format,
            CancellationToken cancellationToken)
        {
            return format switch
            {
                PdfFormat pdf => await RenderPdfInternalAsync(html, pdf, cancellationToken).ConfigureAwait(false),
                PngFormat png => await RenderImageInternalAsync(html, ScreenshotType.Png, png.FullPage, png.OmitBackground, null, png.Margin, png.ViewportWidth, png.ViewportHeight, cancellationToken).ConfigureAwait(false),
                JpegFormat jpeg => await RenderImageInternalAsync(html, ScreenshotType.Jpeg, jpeg.FullPage, false, jpeg.Quality, jpeg.Margin, jpeg.ViewportWidth, jpeg.ViewportHeight, cancellationToken).ConfigureAwait(false),
                WebPFormat => throw new NotSupportedException("Formato WebP não é suportado pelo SDK C# do Playwright. Use PNG ou JPEG como alternativa."),
                _ => throw new NotSupportedException($"Formato de saída '{format.GetType().Name}' não é suportado.")
            };
        }

        /// <summary>
        /// Implementação interna de renderização PDF.
        /// </summary>
        private async Task<byte[]> RenderPdfInternalAsync(
            string html,
            PdfFormat format,
            CancellationToken cancellationToken)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(_options.RenderTimeout);

            var browser = await _browserManager!.GetOrCreateBrowserAsync(cts.Token).ConfigureAwait(false);

            IBrowserContext? context = null;
            IPage? page = null;

            try
            {
                context = await browser.NewContextAsync().ConfigureAwait(false);
                context.SetDefaultTimeout((float)_options.RenderTimeout.TotalMilliseconds);

                page = await context.NewPageAsync().ConfigureAwait(false);
                page.SetDefaultTimeout((float)_options.RenderTimeout.TotalMilliseconds);

                // Usa mídia Print para PDF
                await page.EmulateMediaAsync(new PageEmulateMediaOptions
                {
                    Media = Media.Print
                }).ConfigureAwait(false);

                var htmlWithPrintCss = HtmlUtilities.InjectPrintCss(html);

                await page.SetContentAsync(htmlWithPrintCss, new PageSetContentOptions
                {
                    WaitUntil = WaitUntilState.Load
                }).ConfigureAwait(false);

                var margin = format.Margin ?? new PdfMargins();
                var pdfOptions = new PagePdfOptions
                {
                    Format = format.PreferCssPageSize ? null : format.PaperFormat,
                    Landscape = format.Landscape,
                    PrintBackground = format.PrintBackground,
                    Scale = format.Scale,
                    Margin = new Margin
                    {
                        Top = margin.Top,
                        Bottom = margin.Bottom,
                        Left = margin.Left,
                        Right = margin.Right
                    }
                };

                return await page.PdfAsync(pdfOptions).ConfigureAwait(false);
            }
            finally
            {
                await CleanupPageAndContextAsync(page, context).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Implementação interna de renderização de imagem para formatos PNG e JPEG.
        /// </summary>
        private async Task<byte[]> RenderImageInternalAsync(
            string html,
            ScreenshotType type,
            bool fullPage,
            bool omitBackground,
            int? quality,
            int margin,
            int? viewportWidth,
            int? viewportHeight,
            CancellationToken cancellationToken)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(_options.RenderTimeout);

            var browser = await _browserManager!.GetOrCreateBrowserAsync(cts.Token).ConfigureAwait(false);

            // Configura viewport se especificado
            var contextOptions = new BrowserNewContextOptions();
            if (viewportWidth.HasValue && viewportHeight.HasValue)
            {
                contextOptions.ViewportSize = new ViewportSize
                {
                    Width = viewportWidth.Value,
                    Height = viewportHeight.Value
                };
            }

            IBrowserContext? context = null;
            IPage? page = null;

            try
            {
                context = await browser.NewContextAsync(contextOptions).ConfigureAwait(false);
                context.SetDefaultTimeout((float)_options.RenderTimeout.TotalMilliseconds);

                page = await context.NewPageAsync().ConfigureAwait(false);
                page.SetDefaultTimeout((float)_options.RenderTimeout.TotalMilliseconds);

                // Usa mídia Screen para imagens (não Print)
                await page.EmulateMediaAsync(new PageEmulateMediaOptions
                {
                    Media = Media.Screen
                }).ConfigureAwait(false);

                // Injeta CSS de margem se especificado
                var htmlToRender = margin > 0 ? InjectImageMarginCss(html, margin) : html;

                await page.SetContentAsync(htmlToRender, new PageSetContentOptions
                {
                    WaitUntil = WaitUntilState.Load
                }).ConfigureAwait(false);

                var screenshotOptions = new PageScreenshotOptions
                {
                    Type = type,
                    FullPage = fullPage,
                    OmitBackground = omitBackground
                };

                // Quality só se aplica ao formato JPEG
                if (quality.HasValue && type == ScreenshotType.Jpeg)
                {
                    screenshotOptions.Quality = quality.Value;
                }

                return await page.ScreenshotAsync(screenshotOptions).ConfigureAwait(false);
            }
            finally
            {
                await CleanupPageAndContextAsync(page, context).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Injeta CSS para adicionar margem/padding ao redor do conteúdo para renderização de imagem.
        /// </summary>
        private static string InjectImageMarginCss(string html, int marginPx)
        {
            var marginCss = $$"""
                              <style>
                              body {
                                  margin: 0;
                                  padding: {{marginPx}}px;
                                  box-sizing: border-box;
                                  background-color: white;
                              }
                              </style>
                              """;

            // Tenta injetar após <head>
            var headIndex = html.IndexOf("<head", StringComparison.OrdinalIgnoreCase);
            if (headIndex >= 0)
            {
                var headCloseIndex = html.IndexOf(">", headIndex);
                if (headCloseIndex >= 0)
                {
                    return html.Insert(headCloseIndex + 1, marginCss);
                }
            }

            // Fallback: injeta antes de <body>
            var bodyIndex = html.IndexOf("<body", StringComparison.OrdinalIgnoreCase);
            if (bodyIndex >= 0)
            {
                return html.Insert(bodyIndex, marginCss);
            }

            // Último recurso: adiciona no início
            return marginCss + html;
        }

        /// <summary>
        /// Limpa recursos de página e contexto.
        /// </summary>
        private static async Task CleanupPageAndContextAsync(IPage? page, IBrowserContext? context)
        {
            if (page != null)
            {
                try
                {
                    await page.CloseAsync().ConfigureAwait(false);
                }
                catch
                {
                    // Ignora erros na limpeza
                }
            }

            if (context != null)
            {
                try
                {
                    await context.CloseAsync().ConfigureAwait(false);
                }
                catch
                {
                    // Ignora erros na limpeza
                }
            }
        }

        /// <summary>
        /// Descarta o renderer de forma síncrona e libera todos os recursos.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            try
            {
                _browserManager?.Dispose();
                _playwright?.Dispose();
            }
            finally
            {
                _concurrencyGate.Dispose();
                _initializationLock.Dispose();
            }
        }

        /// <summary>
        /// Descarta o renderer de forma assíncrona e libera todos os recursos.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            _disposed = true;

            try
            {
                if (_browserManager != null)
                {
                    await _browserManager.DisposeAsync().ConfigureAwait(false);
                }

                _playwright?.Dispose();
            }
            finally
            {
                _concurrencyGate.Dispose();
                _initializationLock.Dispose();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(PlaywrightHtmlRenderer));
        }
    }
}
