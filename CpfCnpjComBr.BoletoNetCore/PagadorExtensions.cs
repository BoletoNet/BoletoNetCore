using System;
using System.Threading;
using System.Threading.Tasks;
using BoletoNetCore;

namespace CpfCnpjComBr.BoletoNetCore
{
    /// <summary>
    /// Metodos de extensao aditivos para preencher um <see cref="Pagador"/> existente
    /// a partir de um CPF/CNPJ, sem alterar o core do BoletoNetCore.
    /// </summary>
    public static class PagadorExtensions
    {
        /// <summary>
        /// Preenche o <see cref="Pagador"/> a partir do documento consultado.
        /// </summary>
        /// <param name="pagador">Pagador a ser preenchido.</param>
        /// <param name="lookup">Fonte de consulta.</param>
        /// <param name="documento">CPF ou CNPJ, com ou sem mascara.</param>
        /// <param name="sobrescrever">
        /// Quando <c>true</c> (padrao) substitui todos os campos; quando <c>false</c>
        /// preenche apenas os campos vazios, preservando o que ja estava informado.
        /// </param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        public static async Task PreencherAsync(
            this Pagador pagador,
            IPessoaLookup lookup,
            string documento,
            bool sobrescrever = true,
            CancellationToken cancellationToken = default)
        {
            if (pagador == null)
                throw new ArgumentNullException(nameof(pagador));
            if (lookup == null)
                throw new ArgumentNullException(nameof(lookup));

            var resultado = await lookup.ConsultarAsync(documento, cancellationToken).ConfigureAwait(false);
            PagadorResolver.AplicarResultado(pagador, resultado, sobrescrever);
        }
    }
}
