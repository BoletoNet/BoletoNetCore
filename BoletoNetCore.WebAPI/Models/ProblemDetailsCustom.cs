namespace BoletoNetCore.WebAPI.Models
{
    public class ProblemDetailsCustom
    {
        public int StatusCode { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Detalhe { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
