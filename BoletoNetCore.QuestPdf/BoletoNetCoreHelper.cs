using BoletoNetCore.QuestPdf;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoletoNetCore
{
    public static class BoletoNetCoreHelper
    {
        public static byte[] ImprimirCarnePdf(this Boletos listaBoletos)
        {
            return new BoletoCarne().BoletoPdf(listaBoletos);
        }
    }
}
