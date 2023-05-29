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

        public Bancos RetornarBancoEmissor(int tipoBancoEmissor)
        {
            switch (tipoBancoEmissor)
            {
                case 001:
                    return Bancos.BancoDoBrasil;
                case 004:
                    return Bancos.BancoDoNordeste;
                case 033:
                    return Bancos.Santander;
                case 041:
                    return Bancos.Banrisul;
                case 084:
                    return Bancos.UniprimeNortePR;
                case 085:
                    return Bancos.Cecred;
                case 104:
                    return Bancos.Caixa;
                case 237:
                    return Bancos.Bradesco;
                case 341:
                    return Bancos.Itau;
                case 422:
                    return Bancos.Safra;
                case 748:
                    return Bancos.Sicredi;
                case 756:
                    return Bancos.Sicoob;
                default:
                    throw new ArgumentException("Banco não implementado", nameof(tipoBancoEmissor));
            }
        }
    }
}
