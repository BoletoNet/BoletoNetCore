namespace BoletoNetCore.WebAPI.Models
{
    public class EnderecoResponse
    {
        public int EnderecoId { get; set; }

        public string Logradouro { get; set; } = string.Empty;

        public string Numero { get; set;} = string.Empty;
        
        public string Complemento { get; set; } = string.Empty;

        public string Bairro { get; set; } = string.Empty;

        public string Cidade { get; set; } = string.Empty;
        
        public string Estado { get; set; } = string.Empty;

        public string CEP { get; set; } = string.Empty;
    }
}
