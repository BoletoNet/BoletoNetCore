using System;

namespace CpfCnpjComBr.BoletoNetCore
{
    /// <summary>
    /// Erro devolvido pela API cpfcnpj.com.br (status 0) ou por falha de transporte.
    /// </summary>
    public sealed class CpfCnpjComBrException : Exception
    {
        /// <summary>Codigo de erro retornado pela API (campo erroCodigo), quando houver.</summary>
        public int? CodigoErro { get; }

        public CpfCnpjComBrException(string mensagem, int? codigoErro = null)
            : base(mensagem)
        {
            CodigoErro = codigoErro;
        }

        public CpfCnpjComBrException(string mensagem, Exception innerException)
            : base(mensagem, innerException)
        {
        }
    }
}
