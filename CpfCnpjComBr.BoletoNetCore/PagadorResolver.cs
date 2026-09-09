using System;
using System.Threading;
using System.Threading.Tasks;
using BoletoNetCore;

namespace CpfCnpjComBr.BoletoNetCore
{
    /// <summary>
    /// Resolve um <see cref="Pagador"/> nativo do BoletoNetCore a partir de um CPF/CNPJ,
    /// usando um <see cref="IPessoaLookup"/>. Aditivo: nao altera o core do BoletoNetCore.
    /// </summary>
    public sealed class PagadorResolver
    {
        private readonly IPessoaLookup _lookup;

        public PagadorResolver(IPessoaLookup lookup)
        {
            _lookup = lookup ?? throw new ArgumentNullException(nameof(lookup));
        }

        /// <summary>
        /// Consulta o documento e devolve um <see cref="Pagador"/> novo, com nome, endereco
        /// e CPFCNPJ preenchidos. O documento e entregue limpo ao setter de CPFCNPJ.
        /// </summary>
        public async Task<Pagador> ResolverPagadorAsync(string documento, CancellationToken cancellationToken = default)
        {
            var resultado = await _lookup.ConsultarAsync(documento, cancellationToken).ConfigureAwait(false);
            var pagador = new Pagador();
            AplicarResultado(pagador, resultado, sobrescrever: true);
            return pagador;
        }

        /// <summary>
        /// Consulta o documento e preenche um <see cref="Beneficiario"/> existente
        /// (util quando o mesmo cadastro serve de cedente).
        /// </summary>
        public async Task<Beneficiario> ResolverBeneficiarioAsync(string documento, CancellationToken cancellationToken = default)
        {
            var resultado = await _lookup.ConsultarAsync(documento, cancellationToken).ConfigureAwait(false);
            var beneficiario = new Beneficiario();
            beneficiario.CPFCNPJ = resultado.Documento;
            beneficiario.Nome = resultado.Nome;
            beneficiario.Endereco = MontarEndereco(resultado);
            return beneficiario;
        }

        internal static void AplicarResultado(Pagador pagador, PessoaResultado resultado, bool sobrescrever)
        {
            if (sobrescrever || string.IsNullOrEmpty(pagador.Nome))
                pagador.Nome = resultado.Nome;

            // O setter valida 11/14 e lanca ArgumentException; o documento ja vem limpo e validado.
            if (sobrescrever || string.IsNullOrEmpty(pagador.CPFCNPJ))
                pagador.CPFCNPJ = resultado.Documento;

            if (pagador.Endereco == null)
                pagador.Endereco = new Endereco();

            var e = pagador.Endereco;
            if (sobrescrever || string.IsNullOrEmpty(e.LogradouroEndereco))
                e.LogradouroEndereco = resultado.Logradouro;
            if (sobrescrever || string.IsNullOrEmpty(e.LogradouroNumero))
                e.LogradouroNumero = resultado.Numero;
            if (sobrescrever || string.IsNullOrEmpty(e.LogradouroComplemento))
                e.LogradouroComplemento = resultado.Complemento;
            if (sobrescrever || string.IsNullOrEmpty(e.Bairro))
                e.Bairro = resultado.Bairro;
            if (sobrescrever || string.IsNullOrEmpty(e.Cidade))
                e.Cidade = resultado.Cidade;
            if (sobrescrever || string.IsNullOrEmpty(e.UF))
                e.UF = resultado.Uf;
            if (sobrescrever || string.IsNullOrEmpty(e.CEP))
                e.CEP = resultado.Cep;
        }

        private static Endereco MontarEndereco(PessoaResultado r)
        {
            return new Endereco
            {
                LogradouroEndereco = r.Logradouro,
                LogradouroNumero = r.Numero,
                LogradouroComplemento = r.Complemento,
                Bairro = r.Bairro,
                Cidade = r.Cidade,
                UF = r.Uf,
                CEP = r.Cep,
            };
        }
    }
}
