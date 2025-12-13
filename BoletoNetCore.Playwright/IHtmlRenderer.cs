namespace BoletoNetCore
{
    /// <summary>
    /// Define o contrato para renderização de HTML em diversos formatos de saída utilizando um navegador headless.
    /// </summary>
    /// <remarks>
    /// Implementações desta interface gerenciam o ciclo de vida de uma instância do navegador
    /// e fornecem capacidade de renderização thread-safe. As instâncias devem ser
    /// descartadas corretamente para liberar os recursos do navegador.
    /// </remarks>
    public interface IHtmlRenderer : IAsyncDisposable
    {
        /// <summary>
        /// Renderiza conteúdo HTML para o formato de saída especificado de forma assíncrona.
        /// </summary>
        /// <param name="html">O conteúdo HTML a ser renderizado. Deve ser um documento HTML completo com DOCTYPE e estrutura adequados.</param>
        /// <param name="format">Especificação do formato de saída. Use <see cref="PdfFormat"/>, <see cref="PngFormat"/>, <see cref="JpegFormat"/> ou <see cref="WebPFormat"/>.</param>
        /// <param name="cancellationToken">Token de cancelamento para cancelar a operação de renderização.</param>
        /// <returns>Uma task que representa a operação assíncrona. O resultado contém a saída gerada como um array de bytes.</returns>
        /// <remarks>
        /// <para>
        /// Este método é thread-safe e pode ser chamado concorrentemente. A concorrência é limitada
        /// internamente pela configuração MaxConcurrency do renderer para evitar esgotamento de recursos.
        /// </para>
        /// <para>
        /// O navegador deve ser inicializado (via <see cref="InitializeAsync"/>) antes de chamar este método,
        /// ou a implementação deve tratar a inicialização lazy automaticamente.
        /// </para>
        /// <para>
        /// Para saída PDF, utiliza emulação de mídia print. Para formatos de imagem, utiliza emulação de mídia screen.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Lançada quando <paramref name="html"/> ou <paramref name="format"/> é nulo.</exception>
        /// <exception cref="NotSupportedException">Lançada quando o formato de saída não é suportado.</exception>
        /// <exception cref="OperationCanceledException">Lançada quando a operação é cancelada via <paramref name="cancellationToken"/> ou ocorre timeout.</exception>
        Task<byte[]> RenderAsync(string html, OutputFormat format, CancellationToken cancellationToken = default);

        /// <summary>
        /// Inicializa a instância do navegador utilizada para renderização de forma assíncrona.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento para cancelar a operação de inicialização.</param>
        /// <returns>Uma task que representa a operação de inicialização assíncrona.</returns>
        /// <remarks>
        /// A chamada explícita deste método é opcional. Permite pré-aquecer o navegador para evitar
        /// o atraso de cold-start na primeira renderização. Se não for chamado explicitamente, o navegador
        /// será inicializado de forma lazy na primeira chamada a <see cref="RenderAsync"/>.
        /// Chamar este método múltiplas vezes é seguro e idempotente - chamadas subsequentes não têm efeito
        /// se o navegador já estiver inicializado.
        /// </remarks>
        /// <exception cref="OperationCanceledException">Lançada quando a operação é cancelada via <paramref name="cancellationToken"/>.</exception>
        Task InitializeAsync(CancellationToken cancellationToken = default);
    }
}
