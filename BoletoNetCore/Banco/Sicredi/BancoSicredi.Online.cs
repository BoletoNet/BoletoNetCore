using System.Threading.Tasks;

namespace BoletoNetCore
{
    partial class BancoSicredi : IBancoOnlineRest
    {
        public string UrlApi => "https://cobrancaonline.sicredi.com.br/sicredi-cobranca-ws-ecomm-api/ecomm/v1/boleto/";

        public Task<string> GerarToken()
        {
            throw new System.NotImplementedException();
        }

        public Task RegistrarBoleto(ref Boleto boleto, string registro)
        {
            throw new System.NotImplementedException();
        }
    }
}