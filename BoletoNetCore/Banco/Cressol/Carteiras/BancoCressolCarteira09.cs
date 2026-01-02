using System;

namespace BoletoNetCore
{
    [CarteiraCodigo("09")]
    internal class BancoCressolCarteira09 : BancoCressolCarteiraBase, ICarteira<BancoCressol>
    {
        internal static Lazy<ICarteira<BancoCressol>> Instance { get; } = new Lazy<ICarteira<BancoCressol>>(() => new BancoCressolCarteira09());

        private BancoCressolCarteira09()
        {
        }
    }
}
