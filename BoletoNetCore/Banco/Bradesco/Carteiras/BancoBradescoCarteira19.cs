using System;

namespace BoletoNetCore
{
    [CarteiraCodigo("19")]
    internal class BancoBradescoCarteira19 : BancoBradescoCarteiraBase, ICarteira<BancoBradesco>
    {
        internal static Lazy<ICarteira<BancoBradesco>> Instance { get; } =
            new Lazy<ICarteira<BancoBradesco>>(() => new BancoBradescoCarteira19());

        private BancoBradescoCarteira19()
        {
        }
    }
}