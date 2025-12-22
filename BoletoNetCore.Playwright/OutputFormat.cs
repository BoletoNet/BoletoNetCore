namespace BoletoNetCore;

/// <summary>
/// Tipo base para especificações de formato de saída utilizando padrão de discriminated union.
/// Cada tipo derivado encapsula opções de renderização específicas do formato.
/// </summary>
public abstract record OutputFormat
{
    /// <summary>
    /// Obtém a extensão de arquivo para este formato de saída, incluindo o ponto inicial.
    /// </summary>
    public abstract string FileExtension { get; }
}

/// <summary>
/// Formato de saída PDF com opções específicas para impressão.
/// </summary>
/// <param name="PaperFormat">Formato do tamanho do papel (ex.: "A4", "Letter", "Legal"). Padrão: "A4".</param>
/// <param name="Landscape">Se deve usar orientação paisagem. Padrão: false (retrato).</param>
/// <param name="PrintBackground">Se deve imprimir backgrounds CSS. Padrão: true.</param>
/// <param name="Scale">Fator de escala entre 0.1 e 2.0. Padrão: 1.0.</param>
/// <param name="Margin">Margens da página. Se nulo, usa margens padrão de 10mm.</param>
/// <param name="PreferCssPageSize">Se verdadeiro, utiliza dimensões definidas por regras CSS @page em vez de <paramref name="PaperFormat"/>. Padrão: false.</param>
public sealed record PdfFormat(
    string PaperFormat = "A4",
    bool Landscape = false,
    bool PrintBackground = true,
    float Scale = 1.0f,
    PdfMargins? Margin = null,
    bool PreferCssPageSize = false) : OutputFormat
{
    /// <summary>
    /// Formato PDF padrão com papel A4, orientação retrato e margens de 10mm.
    /// </summary>
    public static PdfFormat Default { get; } = new();

    /// <inheritdoc/>
    public override string FileExtension => ".pdf";
}

/// <summary>
/// Formato de saída PNG com compressão sem perdas e suporte a transparência.
/// </summary>
/// <param name="FullPage">Captura a página completa com scroll. Padrão: true.</param>
/// <param name="OmitBackground">Renderiza com background transparente. Padrão: false.</param>
/// <param name="Margin">Margem em pixels ao redor do conteúdo. Padrão: 0.</param>
/// <param name="ViewportWidth">Largura opcional do viewport em pixels. Usa padrão se nulo.</param>
/// <param name="ViewportHeight">Altura opcional do viewport em pixels. Usa padrão se nulo.</param>
public sealed record PngFormat(
    bool FullPage = true,
    bool OmitBackground = false,
    int Margin = 0,
    int? ViewportWidth = null,
    int? ViewportHeight = null) : OutputFormat
{
    /// <summary>
    /// Formato PNG padrão com captura de página completa e background opaco.
    /// </summary>
    public static PngFormat Default { get; } = new();

    /// <inheritdoc/>
    public override string FileExtension => ".png";
}

/// <summary>
/// Formato de saída JPEG com qualidade configurável e compressão com perdas.
/// </summary>
/// <param name="Quality">Qualidade da imagem de 0 a 100. Valores maiores produzem arquivos maiores com melhor qualidade. Padrão: 80.</param>
/// <param name="FullPage">Captura a página completa com scroll. Padrão: true.</param>
/// <param name="Margin">Margem em pixels ao redor do conteúdo. Padrão: 0.</param>
/// <param name="ViewportWidth">Largura opcional do viewport em pixels. Usa padrão se nulo.</param>
/// <param name="ViewportHeight">Altura opcional do viewport em pixels. Usa padrão se nulo.</param>
public sealed record JpegFormat(
    int Quality = 80,
    bool FullPage = true,
    int Margin = 0,
    int? ViewportWidth = null,
    int? ViewportHeight = null) : OutputFormat
{
    /// <summary>
    /// Formato JPEG padrão com qualidade de 80% e captura de página completa.
    /// </summary>
    public static JpegFormat Default { get; } = new();

    /// <inheritdoc/>
    public override string FileExtension => ".jpeg";
}

/// <summary>
/// Formato de saída WebP com qualidade configurável, transparência e compressão moderna.
/// </summary>
/// <param name="Quality">Qualidade da imagem de 0 a 100. Valores maiores produzem arquivos maiores com melhor qualidade. Padrão: 80.</param>
/// <param name="FullPage">Captura a página completa com scroll. Padrão: true.</param>
/// <param name="OmitBackground">Renderiza com background transparente. Padrão: false.</param>
/// <param name="ViewportWidth">Largura opcional do viewport em pixels. Usa padrão se nulo.</param>
/// <param name="ViewportHeight">Altura opcional do viewport em pixels. Usa padrão se nulo.</param>
/// <remarks>
/// <strong>Nota:</strong> WebP não é suportado atualmente pelo SDK C# do Playwright.
/// Tentar usar este formato lançará <see cref="System.NotSupportedException"/>.
/// Use <see cref="PngFormat"/> ou <see cref="JpegFormat"/> como alternativa.
/// </remarks>
public sealed record WebPFormat(
    int Quality = 80,
    bool FullPage = true,
    bool OmitBackground = false,
    int? ViewportWidth = null,
    int? ViewportHeight = null) : OutputFormat
{
    /// <summary>
    /// Formato WebP padrão com qualidade de 80%, captura de página completa e background opaco.
    /// </summary>
    public static WebPFormat Default { get; } = new();

    /// <inheritdoc/>
    public override string FileExtension => ".webp";
}
