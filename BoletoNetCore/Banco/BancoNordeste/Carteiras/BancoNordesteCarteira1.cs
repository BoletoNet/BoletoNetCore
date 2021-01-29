using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoletoNetCore
{
    [CarteiraCodigo("1")]
    internal class BancoNordesteCarteira1 : ICarteira<BancoNordeste>
    {
        internal static Lazy<ICarteira<BancoNordeste>> Instance { get; } = new Lazy<ICarteira<BancoNordeste>>(() => new BancoNordesteCarteira1());

        private BancoNordesteCarteira1()
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
