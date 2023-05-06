using BoletoNetCore.WebAPI.Models;

namespace BoletoNetCore.WebAPI.Extensions
{
    public class MetodosUteis
    {
        public ProblemDetailsCustom RetornarErroPersonalizado(int statusCode, string titulo, string detalhes, string url)
        {
            return new ProblemDetailsCustom()
            {
                StatusCode = statusCode,
                Titulo = titulo,
                Detalhe = detalhes,
                Url = url
            };
        }
    }
}
