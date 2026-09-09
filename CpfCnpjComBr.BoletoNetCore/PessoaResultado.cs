namespace CpfCnpjComBr.BoletoNetCore
{
    /// <summary>
    /// Resultado neutro de uma consulta de CPF/CNPJ, ja mapeado para os campos
    /// que o BoletoNetCore precisa para montar um <c>Pagador</c> e seu <c>Endereco</c>.
    /// </summary>
    public sealed class PessoaResultado
    {
        /// <summary>Documento limpo (11 digitos para CPF, 14 posicoes para CNPJ).</summary>
        public string Documento { get; set; } = string.Empty;

        /// <summary>Nome da pessoa fisica ou razao social da pessoa juridica.</summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>Logradouro ja formatado (tipo + logradouro no caso de CNPJ).</summary>
        public string Logradouro { get; set; } = string.Empty;

        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;

        /// <summary>CEP apenas com digitos.</summary>
        public string Cep { get; set; } = string.Empty;

        /// <summary>Verdadeiro para CPF, falso para CNPJ.</summary>
        public bool EhPessoaFisica { get; set; }
    }
}
