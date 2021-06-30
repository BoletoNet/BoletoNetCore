using System.Threading.Tasks;

namespace BoletoNetCore
{
    partial class BancoSicredi : IBancoOnlineRest
    {
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