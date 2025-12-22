namespace BoletoNetCore.Playwright.ConsoleApp.Configuration;

/// <summary>
/// Seção raiz de configuração para renderização Playwright.
/// </summary>
public sealed class PlaywrightPdfConfiguration
{
    /// <summary>
    /// Opções de ciclo de vida e comportamento do renderizador.
    /// </summary>
    public RendererConfiguration Renderer { get; set; } = new();

    /// <summary>
    /// Opções de saída PDF.
    /// </summary>
    public PdfOptionsConfiguration Options { get; set; } = new();
}

/// <summary>
/// Configuração do ciclo de vida do renderizador Playwright.
/// </summary>
public sealed class RendererConfiguration
{
    /// <summary>
    /// Número máximo de operações de renderização concorrentes.
    /// </summary>
    public int MaxConcurrency { get; set; } = 2;

    /// <summary>
    /// Timeout para uma única operação de renderização em segundos.
    /// </summary>
    public int RenderTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Se deve pré-aquecer o navegador na inicialização do renderizador.
    /// </summary>
    public bool PrewarmOnStart { get; set; }

    /// <summary>
    /// Se deve reiniciar automaticamente o navegador em caso de falha.
    /// </summary>
    public bool RestartOnFailure { get; set; } = true;

    /// <summary>
    /// Caminho para um executável customizado do navegador.
    /// </summary>
    public string? BrowserExecutablePath { get; set; }

    /// <summary>
    /// Argumentos de linha de comando adicionais para o navegador.
    /// </summary>
    public string[]? BrowserLaunchArgs { get; set; }

    /// <summary>
    /// Converte esta configuração para PlaywrightRendererOptions.
    /// </summary>
    public PlaywrightRendererOptions ToRendererOptions()
    {
        return new PlaywrightRendererOptions
        {
            MaxConcurrency = this.MaxConcurrency,
            RenderTimeout = TimeSpan.FromSeconds(this.RenderTimeoutSeconds),
            PrewarmOnStart = this.PrewarmOnStart,
            RestartOnFailure = this.RestartOnFailure,
            BrowserExecutablePath = this.BrowserExecutablePath,
            BrowserLaunchArgs = this.BrowserLaunchArgs
        };
    }
}

/// <summary>
/// Configuração para opções de saída PDF.
/// </summary>
public sealed class PdfOptionsConfiguration
{
    /// <summary>
    /// Formato do papel para o PDF (ex.: "A4", "Letter").
    /// </summary>
    public string Format { get; set; } = "A4";

    /// <summary>
    /// Se deve renderizar em orientação paisagem.
    /// </summary>
    public bool Landscape { get; set; }

    /// <summary>
    /// Se deve imprimir backgrounds CSS.
    /// </summary>
    public bool PrintBackground { get; set; } = true;

    /// <summary>
    /// Se deve preferir tamanho de página CSS sobre Format.
    /// </summary>
    public bool PreferCssPageSize { get; set; }

    /// <summary>
    /// Fator de escala para renderização (0.1 a 2.0).
    /// </summary>
    public float Scale { get; set; } = 1.0f;

    /// <summary>
    /// Margens da página.
    /// </summary>
    public MarginConfiguration Margin { get; set; } = new();

    /// <summary>
    /// Converte esta configuração para PdfFormat.
    /// </summary>
    public PdfFormat ToPdfFormat()
    {
        return new PdfFormat(
            PaperFormat: this.Format,
            Landscape: this.Landscape,
            PrintBackground: this.PrintBackground,
            Scale: this.Scale,
            Margin: new PdfMargins
            {
                Top = this.Margin.Top,
                Bottom = this.Margin.Bottom,
                Left = this.Margin.Left,
                Right = this.Margin.Right
            },
            PreferCssPageSize: this.PreferCssPageSize
        );
    }
}

/// <summary>
/// Configuração para margens do PDF.
/// </summary>
public sealed class MarginConfiguration
{
    public string Top { get; set; } = "10mm";
    public string Bottom { get; set; } = "10mm";
    public string Left { get; set; } = "10mm";
    public string Right { get; set; } = "10mm";
}
