using System.ComponentModel.DataAnnotations;

namespace BoletoNetCore.WebAPI.Models
{
    public class PagadorResponse
    {
        [Key]
        public int PagadorResponseId { get; set; }  

        public string CPFCNPJ { get; set; } = string.Empty;

        public string Nome { get; set; } = string.Empty;

        public string Observacoes { get; set; } = string.Empty;

        public EnderecoResponse EnderecoResponse { get; set; } = new EnderecoResponse();
    }
}
