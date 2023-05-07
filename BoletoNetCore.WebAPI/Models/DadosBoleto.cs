namespace BoletoNetCore.WebAPI.Models
{
    
    public class DadosBoleto
    {
       
        public PagadorResponse PagadorResponse { get; set; } = new PagadorResponse();
        public BeneficiarioResponse BeneficiarioResponse { get; set; } = new BeneficiarioResponse();
      
        public DateTime DataEmissao { get; set; }

        public DateTime DataProcessamento { get; set; }

        public DateTime DataVencimento { get; set; }

        public decimal ValorTitulo { get; set; }

        public string NossoNumero { get; set; } = string.Empty;

        public string NumeroDocumento { get; set; } = string.Empty;

        public string CampoLivre { get; set; } = string.Empty;
    }
}
