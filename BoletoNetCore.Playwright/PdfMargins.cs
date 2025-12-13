namespace BoletoNetCore;

/// <summary>
/// Define as margens para renderização de páginas PDF.
/// </summary>
/// <remarks>
/// Todos os valores são especificados como strings no formato CSS (ex.: "10mm", "1in", "2.54cm").
/// O Playwright aceita valores numéricos com unidades: mm, cm, in, px.
/// </remarks>
public class PdfMargins
{
    /// <summary>
    /// Obtém ou define a margem superior.
    /// </summary>
    /// <value>
    /// String de margem no formato CSS. Padrão: "10mm".
    /// </value>
    public string Top { get; set; } = "10mm";

    /// <summary>
    /// Obtém ou define a margem inferior.
    /// </summary>
    /// <value>
    /// String de margem no formato CSS. Padrão: "10mm".
    /// </value>
    public string Bottom { get; set; } = "10mm";

    /// <summary>
    /// Obtém ou define a margem esquerda.
    /// </summary>
    /// <value>
    /// String de margem no formato CSS. Padrão: "10mm".
    /// </value>
    public string Left { get; set; } = "10mm";

    /// <summary>
    /// Obtém ou define a margem direita.
    /// </summary>
    /// <value>
    /// String de margem no formato CSS. Padrão: "10mm".
    /// </value>
    public string Right { get; set; } = "10mm";
}
