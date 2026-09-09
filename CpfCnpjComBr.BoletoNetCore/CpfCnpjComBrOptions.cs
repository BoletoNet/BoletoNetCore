using System;

namespace CpfCnpjComBr.BoletoNetCore
{
    /// <summary>
    /// Configuracao do cliente da API cpfcnpj.com.br.
    /// </summary>
    public sealed class CpfCnpjComBrOptions
    {
        /// <summary>Token de autenticacao emitido em https://www.cpfcnpj.com.br/dev/</summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>Endereco base da API. Padrao: https://api.cpfcnpj.com.br</summary>
        public string BaseUrl { get; set; } = "https://api.cpfcnpj.com.br";

        /// <summary>Pacote usado para CPF (nome + endereco). Padrao: 3.</summary>
        public int PacoteCpf { get; set; } = 3;

        /// <summary>Pacote usado para CNPJ (razao + endereco da matriz). Padrao: 5.</summary>
        public int PacoteCnpj { get; set; } = 5;

        internal void Validar()
        {
            if (string.IsNullOrWhiteSpace(Token))
                throw new InvalidOperationException("CpfCnpjComBrOptions.Token nao foi informado.");
            if (string.IsNullOrWhiteSpace(BaseUrl))
                throw new InvalidOperationException("CpfCnpjComBrOptions.BaseUrl nao foi informado.");
        }
    }
}
