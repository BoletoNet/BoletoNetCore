using BoletoNetCore.Exceptions;
using static System.String;

namespace BoletoNetCore
{
    public class ContaBancaria
    {
        public TipoCarteira TipoCarteiraPadrao { get; set; } = TipoCarteira.CarteiraCobrancaSimples;
        public string CarteiraPadrao { get; set; } = string.Empty;
        public string VariacaoCarteiraPadrao { get; set; } = string.Empty;
        public string CarteiraComVariacaoPadrao => string.IsNullOrEmpty(CarteiraPadrao) || string.IsNullOrEmpty(VariacaoCarteiraPadrao) ? $"{CarteiraPadrao}{VariacaoCarteiraPadrao}" : $"{CarteiraPadrao}/{VariacaoCarteiraPadrao}";

        public string Agencia { get; set; } = Empty;
        public string DigitoAgencia { get; set; } = Empty;
        public string Conta { get; set; } = Empty;
        public string DigitoConta { get; set; } = Empty;
        public string OperacaoConta { get; set; } = Empty;
        public TipoFormaCadastramento TipoFormaCadastramento { get; set; } = TipoFormaCadastramento.ComRegistro;
        public TipoImpressaoBoleto TipoImpressaoBoleto { get; set; } = TipoImpressaoBoleto.Empresa;
        public TipoDocumento TipoDocumento { get; set; } = TipoDocumento.Tradicional;
        public string LocalPagamento { get; set; } = "PAGÁVEL EM QUALQUER BANCO.";
        public string MensagemFixaTopoBoleto { get; set; } = "";
        public string MensagemFixaPagador { get; set; } = "";
        public int CodigoBancoCorrespondente { get; set; }
        public string NossoNumeroBancoCorrespondente { get; set; }
        public TipoDistribuicaoBoleto TipoDistribuicao { get; set; } = TipoDistribuicaoBoleto.ClienteDistribui;

        public void FormatarDados(string localPagamento, string mensagemFixaTopoBoleto, string mensagemFixaPagador, int digitosConta)
        {
            LocalPagamento = localPagamento;
            MensagemFixaTopoBoleto = mensagemFixaTopoBoleto;
            MensagemFixaPagador = mensagemFixaPagador;

            var agencia = Agencia;
            Agencia = agencia.Length <= 4 ? agencia.PadLeft(4, '0') : throw BoletoNetCoreException.AgenciaInvalida(agencia, 4);

            var conta = Conta;
            Conta = conta.Length <= digitosConta ? conta.PadLeft(digitosConta, '0') : throw BoletoNetCoreException.ContaInvalida(conta, digitosConta);
        }
    }
}
