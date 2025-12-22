using BoletoNetCore.Playwright.Internal;

namespace BoletoNetCore
{
    /// <summary>
    /// Métodos de extensão para renderização de objetos Boleto e Boletos utilizando Playwright.
    /// Suporta múltiplos formatos de saída: PDF, PNG e JPEG.
    /// </summary>
    /// <remarks>
    /// Esta classe fornece métodos síncronos e assíncronos para renderização.
    /// Os métodos assíncronos são preferíveis para melhor escalabilidade e utilização de recursos.
    /// Métodos que não recebem um renderer explícito utilizam um renderer padrão compartilhado
    /// que é inicializado de forma lazy no primeiro uso e reutilizado em todas as chamadas para throughput otimizado.
    /// </remarks>
    public static class PlaywrightExtensions
    {
        private static readonly Lazy<PlaywrightHtmlRenderer> DefaultRenderer = new(
            () => new PlaywrightHtmlRenderer(new PlaywrightRendererOptions()));

        #region Single Boleto Extensions

        /// <summary>
        /// Renderiza um único boleto para o formato de saída especificado de forma assíncrona utilizando um renderer.
        /// </summary>
        /// <param name="boleto">O boleto a ser renderizado.</param>
        /// <param name="renderer">O renderer HTML a ser utilizado.</param>
        /// <param name="format">A especificação do formato de saída (PDF, PNG ou JPEG).</param>
        /// <param name="convertLinhaDigitavelToImage">Se verdadeiro, converte a linha digitável em imagem.</param>
        /// <param name="urlImagemLogoBeneficiario">URL opcional ou data URI codificada em base64 para o logo do beneficiário.</param>
        /// <param name="cancellationToken">Token de cancelamento para cancelar a operação.</param>
        /// <returns>Uma task contendo a saída renderizada como um array de bytes.</returns>
        /// <exception cref="ArgumentNullException">Lançada quando <paramref name="boleto"/>, <paramref name="renderer"/> ou <paramref name="format"/> é nulo.</exception>
        public static async Task<byte[]> RenderPlaywrightAsync(
            this Boleto boleto,
            IHtmlRenderer renderer,
            OutputFormat format,
            bool convertLinhaDigitavelToImage = false,
            string? urlImagemLogoBeneficiario = null,
            CancellationToken cancellationToken = default)
        {
            if (boleto == null)
                throw new ArgumentNullException(nameof(boleto));
            if (renderer == null)
                throw new ArgumentNullException(nameof(renderer));
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            var boletoBancario = new BoletoBancario { Boleto = boleto };
            var html = boletoBancario.MontaHtmlEmbedded(
                convertLinhaDigitavelToImage,
                usaCsspdf: false,
                urlImagemLogoBeneficiario: urlImagemLogoBeneficiario);

            return await renderer.RenderAsync(html, format, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Renderiza um único boleto para o formato de saída especificado de forma assíncrona utilizando o renderer padrão.
        /// </summary>
        /// <param name="boleto">O boleto a ser renderizado.</param>
        /// <param name="format">A especificação do formato de saída (PDF, PNG ou JPEG).</param>
        /// <param name="convertLinhaDigitavelToImage">Se verdadeiro, converte a linha digitável em imagem.</param>
        /// <param name="urlImagemLogoBeneficiario">URL opcional ou data URI codificada em base64 para o logo do beneficiário.</param>
        /// <param name="cancellationToken">Token de cancelamento para cancelar a operação.</param>
        /// <returns>Uma task contendo a saída renderizada como um array de bytes.</returns>
        /// <exception cref="ArgumentNullException">Lançada quando <paramref name="boleto"/> ou <paramref name="format"/> é nulo.</exception>
        public static async Task<byte[]> RenderPlaywrightAsync(
            this Boleto boleto,
            OutputFormat format,
            bool convertLinhaDigitavelToImage = false,
            string? urlImagemLogoBeneficiario = null,
            CancellationToken cancellationToken = default)
        {
            return await boleto.RenderPlaywrightAsync(
                DefaultRenderer.Value,
                format,
                convertLinhaDigitavelToImage,
                urlImagemLogoBeneficiario,
                cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Renderiza um único boleto para o formato de saída especificado de forma síncrona utilizando o renderer padrão.
        /// </summary>
        /// <param name="boleto">O boleto a ser renderizado.</param>
        /// <param name="format">A especificação do formato de saída (PDF, PNG ou JPEG).</param>
        /// <param name="convertLinhaDigitavelToImage">Se verdadeiro, converte a linha digitável em imagem.</param>
        /// <param name="urlImagemLogoBeneficiario">URL opcional ou data URI codificada em base64 para o logo do beneficiário.</param>
        /// <returns>A saída renderizada como um array de bytes.</returns>
        public static byte[] RenderPlaywright(
            this Boleto boleto,
            OutputFormat format,
            bool convertLinhaDigitavelToImage = false,
            string? urlImagemLogoBeneficiario = null)
        {
            return boleto.RenderPlaywrightAsync(
                format,
                convertLinhaDigitavelToImage,
                urlImagemLogoBeneficiario,
                CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        #endregion

        #region Multiple Boletos Extensions

        /// <summary>
        /// Renderiza múltiplos boletos para o formato de saída especificado de forma assíncrona utilizando um renderer.
        /// </summary>
        /// <param name="boletos">A coleção de boletos a serem renderizados.</param>
        /// <param name="renderer">O renderer HTML a ser utilizado.</param>
        /// <param name="format">A especificação do formato de saída (PDF, PNG ou JPEG).</param>
        /// <param name="convertLinhaDigitavelToImage">Se verdadeiro, converte a linha digitável em imagem.</param>
        /// <param name="urlImagemLogoBeneficiario">URL opcional ou data URI codificada em base64 para o logo do beneficiário.</param>
        /// <param name="cancellationToken">Token de cancelamento para cancelar a operação.</param>
        /// <returns>Uma task contendo a saída renderizada como um array de bytes.</returns>
        /// <exception cref="ArgumentNullException">Lançada quando <paramref name="boletos"/>, <paramref name="renderer"/> ou <paramref name="format"/> é nulo.</exception>
        /// <remarks>
        /// Para saída PDF, todos os boletos são combinados em um único documento com quebras de página.
        /// Para formatos de imagem, todos os boletos são renderizados como uma única imagem alta.
        /// </remarks>
        public static async Task<byte[]> RenderPlaywrightAsync(
            this Boletos boletos,
            IHtmlRenderer renderer,
            OutputFormat format,
            bool convertLinhaDigitavelToImage = false,
            string? urlImagemLogoBeneficiario = null,
            CancellationToken cancellationToken = default)
        {
            if (boletos == null)
                throw new ArgumentNullException(nameof(boletos));
            if (renderer == null)
                throw new ArgumentNullException(nameof(renderer));
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            var html = HtmlUtilities.CombineBoletosHtml(
                boletos,
                convertLinhaDigitavelToImage,
                urlImagemLogoBeneficiario);

            return await renderer.RenderAsync(html, format, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Renderiza múltiplos boletos para o formato de saída especificado de forma assíncrona utilizando o renderer padrão.
        /// </summary>
        /// <param name="boletos">A coleção de boletos a serem renderizados.</param>
        /// <param name="format">A especificação do formato de saída (PDF, PNG ou JPEG).</param>
        /// <param name="convertLinhaDigitavelToImage">Se verdadeiro, converte a linha digitável em imagem.</param>
        /// <param name="urlImagemLogoBeneficiario">URL opcional ou data URI codificada em base64 para o logo do beneficiário.</param>
        /// <param name="cancellationToken">Token de cancelamento para cancelar a operação.</param>
        /// <returns>Uma task contendo a saída renderizada como um array de bytes.</returns>
        /// <exception cref="ArgumentNullException">Lançada quando <paramref name="boletos"/> ou <paramref name="format"/> é nulo.</exception>
        public static async Task<byte[]> RenderPlaywrightAsync(
            this Boletos boletos,
            OutputFormat format,
            bool convertLinhaDigitavelToImage = false,
            string? urlImagemLogoBeneficiario = null,
            CancellationToken cancellationToken = default)
        {
            return await boletos.RenderPlaywrightAsync(
                DefaultRenderer.Value,
                format,
                convertLinhaDigitavelToImage,
                urlImagemLogoBeneficiario,
                cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Renderiza múltiplos boletos para o formato de saída especificado de forma síncrona utilizando o renderer padrão.
        /// </summary>
        /// <param name="boletos">A coleção de boletos a serem renderizados.</param>
        /// <param name="format">A especificação do formato de saída (PDF, PNG ou JPEG).</param>
        /// <param name="convertLinhaDigitavelToImage">Se verdadeiro, converte a linha digitável em imagem.</param>
        /// <param name="urlImagemLogoBeneficiario">URL opcional ou data URI codificada em base64 para o logo do beneficiário.</param>
        /// <returns>A saída renderizada como um array de bytes.</returns>
        public static byte[] RenderPlaywright(
            this Boletos boletos,
            OutputFormat format,
            bool convertLinhaDigitavelToImage = false,
            string? urlImagemLogoBeneficiario = null)
        {
            return boletos.RenderPlaywrightAsync(
                format,
                convertLinhaDigitavelToImage,
                urlImagemLogoBeneficiario,
                CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        #endregion
    }
}
