using System.Collections.Generic;

namespace BoletoNetCore
{
    public class Boletos : List<Boleto>
    {
        public IBanco Banco { get; set; }
    }
}
