using System;

namespace BoletoNetCore
{
    [CarteiraCodigo("1")]
    internal class BancoSantanderCarteira1 : ICarteira<BancoSantander>
    {
        internal static Lazy<ICarteira<BancoSantander>> Instance { get; } = new Lazy<ICarteira<BancoSantander>>(() => new BancoSantanderCarteira1());

        private BancoSantanderCarteira1()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {
            
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            return "                         ";
        }
    }
}
