using System;

namespace BoletoNetCore
{
    [CarteiraCodigo("09")]
    internal class BancoCresolCarteira09 : BancoCresolCarteiraBase, ICarteira<BancoCresol>
    {
        internal static Lazy<ICarteira<BancoCresol>> Instance { get; } = new Lazy<ICarteira<BancoCresol>>(() => new BancoCresolCarteira09());

        private BancoCresolCarteira09()
        {
        }
    }
}
