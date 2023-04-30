namespace BoletoNetCore.WebAPI.Models
{
    public class ContaBancariaResponse
    {
        public int ContaBancariaId { get; set; }

        public TipoCarteira TipoCarteiraPadrao { get; set; } = TipoCarteira.CarteiraCobrancaSimples;

        public string CarteiraPadrao { get; set; } = string.Empty;

        public string Agencia { get; set; } = string.Empty;

        public string DigitoAgencia { get; set; } = string.Empty;

        public string Conta { get; set; } = string.Empty;

        public string DigitoConta { get; set; } = string.Empty;

        public string OperacaoConta { get; set; } = string.Empty;

        public TipoFormaCadastramento TipoFormaCadastramento { get; set; } = TipoFormaCadastramento.ComRegistro;

        public TipoImpressaoBoleto TipoImpressaoBoleto { get; set; } = TipoImpressaoBoleto.Empresa;

        public TipoDocumento TipoDocumento { get; set; } = TipoDocumento.Tradicional;

        public string LocalPagamento { get; set; } = string.Empty;

        public string MensagemFixaTopoBoleto { get; set; } = string.Empty;

        public string MensagemFixaPagador { get; set; } = string.Empty;

        public TipoDistribuicaoBoleto TipoDistribuicao { get; set; } = TipoDistribuicaoBoleto.ClienteDistribui;
    }
}
