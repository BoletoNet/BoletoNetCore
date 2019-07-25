using System.Collections.Generic;

namespace Boleto2Net
{
    public class Boletos : List<Boleto>
    {
        public IBanco Banco { get; set; }
    }
}
