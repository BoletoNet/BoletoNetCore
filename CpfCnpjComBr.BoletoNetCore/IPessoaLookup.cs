using System.Threading;
using System.Threading.Tasks;

namespace CpfCnpjComBr.BoletoNetCore
{
    /// <summary>
    /// Contrato de consulta de pessoa por CPF/CNPJ. Implementacoes podem usar
    /// a API cpfcnpj.com.br, um cache proprio, ou um duble em testes.
    /// </summary>
    public interface IPessoaLookup
    {
        /// <summary>
        /// Consulta os dados cadastrais de um CPF ou CNPJ.
        /// </summary>
        /// <param name="documento">CPF ou CNPJ, com ou sem mascara.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Dados mapeados para os campos de pagador/endereco do boleto.</returns>
        Task<PessoaResultado> ConsultarAsync(string documento, CancellationToken cancellationToken = default);
    }
}
