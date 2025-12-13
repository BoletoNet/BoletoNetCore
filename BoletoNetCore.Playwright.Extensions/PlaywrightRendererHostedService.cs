using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace BoletoNetCore
{
    /// <summary>
    /// Hosted service que inicializa o renderizador Playwright durante a inicialização da aplicação.
    /// Fornece inicialização assíncrona adequada com logging e tratamento de erros.
    /// </summary>
    /// <remarks>
    /// Este serviço é sempre registrado por <see cref="PlaywrightServiceCollectionExtensions.AddPlaywrightRenderer"/>
    /// mas só executa a inicialização quando <see cref="PlaywrightRendererOptions.PrewarmOnStart"/> é true.
    /// Verificar em tempo de execução via <see cref="IOptions{TOptions}"/> garante que o prewarm funcione independente de como as opções
    /// são configuradas (delegate, binding de IConfiguration, etc.).
    /// </remarks>
    internal sealed class PlaywrightRendererHostedService : IHostedService
    {
        private readonly PlaywrightHtmlRenderer _renderer;
        private readonly IOptions<PlaywrightRendererOptions> _options;
        private readonly ILogger<PlaywrightRendererHostedService> _logger;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PlaywrightRendererHostedService"/>.
        /// </summary>
        /// <param name="renderer">O renderizador a ser inicializado.</param>
        /// <param name="options">As opções do renderizador para verificar a configuração PrewarmOnStart.</param>
        /// <param name="logger">O logger para saída de diagnóstico.</param>
        public PlaywrightRendererHostedService(
            PlaywrightHtmlRenderer renderer,
            IOptions<PlaywrightRendererOptions> options,
            ILogger<PlaywrightRendererHostedService> logger)
        {
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Inicializa o navegador Playwright durante a inicialização da aplicação se PrewarmOnStart estiver habilitado.
        /// </summary>
        /// <param name="cancellationToken">Um token de cancelamento que indica que o processo de inicialização foi abortado.</param>
        /// <returns>Uma task que representa a operação de inicialização assíncrona.</returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Check PrewarmOnStart at runtime - this ensures prewarm works regardless of how options
            // are configured (delegate, IConfiguration binding, PostConfigure, etc.)
            if (!_options.Value.PrewarmOnStart)
            {
                _logger.LogDebug("Playwright renderer prewarm is disabled, skipping initialization");
                return;
            }

            _logger.LogInformation("Initializing Playwright renderer...");

            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _renderer.InitializeAsync(cancellationToken).ConfigureAwait(false);

                stopwatch.Stop();
                _logger.LogInformation(
                    "Playwright renderer initialized successfully in {ElapsedMilliseconds}ms",
                    stopwatch.ElapsedMilliseconds);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Playwright renderer initialization was cancelled");
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(
                    ex,
                    "Failed to initialize Playwright renderer after {ElapsedMilliseconds}ms",
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        /// <summary>
        /// Chamado quando a aplicação está encerrando. Nenhuma ação é necessária pois o disposal
        /// do renderizador é tratado pelo container de DI.
        /// </summary>
        /// <param name="cancellationToken">Um token de cancelamento que indica que o processo de shutdown foi abortado.</param>
        /// <returns>Uma task completada.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Playwright renderer hosted service stopping");
            return Task.CompletedTask;
        }
    }
}
