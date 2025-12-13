using System.Text;
using System.Text.RegularExpressions;

namespace BoletoNetCore.Playwright.Internal
{
    /// <summary>
    /// Internal utilities for HTML manipulation and composition for PDF rendering.
    /// </summary>
    internal static class HtmlUtilities
    {
        /// <summary>
        /// Extracts the content of the first &lt;style&gt; tag from an HTML document.
        /// </summary>
        /// <param name="html">HTML document containing style tags</param>
        /// <returns>CSS content from the first style tag, or empty string if not found</returns>
        internal static string ExtractStyleContent(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            var styleMatch = Regex.Match(html, @"<style[^>]*>(.*?)</style>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            return styleMatch.Success ? styleMatch.Groups[1].Value : string.Empty;
        }

        /// <summary>
        /// Extracts content between &lt;body&gt; tags from a full HTML document.
        /// Used to avoid nested HTML documents when combining multiple boletos.
        /// </summary>
        /// <param name="html">Full HTML document with body tags</param>
        /// <returns>Inner content of the body tag, or the original HTML if body tags not found</returns>
        internal static string ExtractBodyContent(string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            // Use regex to extract body content, handling attributes and whitespace
            var bodyMatch = Regex.Match(html, @"<body[^>]*>(.*?)</body>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (bodyMatch.Success)
            {
                return bodyMatch.Groups[1].Value;
            }

            // Fallback: return as-is if body tags not found
            return html;
        }

        /// <summary>
        /// Injects print-color-adjust CSS to ensure consistent color rendering in PDFs.
        /// Adds the CSS rule to existing style tags or creates a new style block if none exist.
        /// </summary>
        /// <param name="html">HTML content to inject CSS into</param>
        /// <returns>HTML with injected print-color-adjust CSS</returns>
        internal static string InjectPrintCss(string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            const string printCssRule = "* { -webkit-print-color-adjust: exact; print-color-adjust: exact; }";

            // Check if there's already a <style> tag in the <head>
            var headMatch = Regex.Match(html, @"<head[^>]*>", RegexOptions.IgnoreCase);
            if (headMatch.Success)
            {
                // Insert style tag right after <head>
                var insertPosition = headMatch.Index + headMatch.Length;
                return html.Insert(insertPosition,
                    $"\n<style>\n{printCssRule}\n</style>");
            }

            // Fallback: if no <head> found, insert before <body>
            var bodyMatch = Regex.Match(html, @"<body[^>]*>", RegexOptions.IgnoreCase);
            if (bodyMatch.Success)
            {
                var insertPosition = bodyMatch.Index;
                return html.Insert(insertPosition,
                    $"<style>\n{printCssRule}\n</style>\n");
            }

            // Last resort: prepend to the HTML
            return $"<style>\n{printCssRule}\n</style>\n{html}";
        }

        /// <summary>
        /// Combines multiple boletos into a single HTML document with CSS page breaks.
        /// Extracts body content from each boleto to avoid nested HTML documents.
        /// </summary>
        /// <param name="boletos">Collection of boletos to combine</param>
        /// <param name="convertLinhaDigitavelToImage">Convert linha digitavel to image to prevent malware</param>
        /// <param name="urlImagemLogoBeneficiario">URL or base64 image of beneficiary logo</param>
        /// <returns>Single HTML document containing all boletos with page breaks</returns>
        internal static string CombineBoletosHtml(
            Boletos boletos,
            bool convertLinhaDigitavelToImage,
            string? urlImagemLogoBeneficiario)
        {
            if (boletos == null || boletos.Count == 0)
                throw new ArgumentException("Boletos collection cannot be null or empty", nameof(boletos));

            var htmlBuilder = new StringBuilder();
            string? boletoCss = null;

            // Generate and collect HTML for each boleto, extracting CSS from first one
            var boletosHtml = new System.Collections.Generic.List<string>();
            foreach (var boleto in boletos)
            {
                var boletoBancario = new BoletoBancario { Boleto = boleto };
                var fullHtml = boletoBancario.MontaHtmlEmbedded(
                    convertLinhaDigitavelToImage,
                    usaCsspdf: false,
                    urlImagemLogoBeneficiario: urlImagemLogoBeneficiario);

                // Extract CSS from the first boleto (all boletos use the same CSS)
                if (boletoCss == null)
                {
                    boletoCss = ExtractStyleContent(fullHtml);
                }

                // Extract body inner HTML to avoid nested <html> documents
                var bodyContent = ExtractBodyContent(fullHtml);
                boletosHtml.Add(bodyContent);
            }

            // HTML document header with boleto CSS + print styles
            htmlBuilder.Append(@"<!DOCTYPE html>
<html>
<head>
<meta charset=""utf-8"">
<style>
");
            // Include the original boleto CSS
            if (!string.IsNullOrEmpty(boletoCss))
            {
                htmlBuilder.Append(boletoCss);
                htmlBuilder.Append("\n");
            }

            // Add print-specific styles
            htmlBuilder.Append(@"
@media print {
    .boleto-wrapper { page-break-after: always; }
    .boleto-wrapper:last-child { page-break-after: auto; }
}
* { -webkit-print-color-adjust: exact; print-color-adjust: exact; }
</style>
</head>
<body>");

            // Append all boleto body contents
            foreach (var bodyContent in boletosHtml)
            {
                htmlBuilder.Append("<div class=\"boleto-wrapper\">");
                htmlBuilder.Append(bodyContent);
                htmlBuilder.Append("</div>");
            }

            htmlBuilder.Append("</body></html>");
            return htmlBuilder.ToString();
        }
    }
}
