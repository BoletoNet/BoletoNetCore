using System;
using System.Collections.Generic;
using System.Text;

namespace BoletoNetCore.QuestPdf
{
    internal static  class DecimalHelper
    {
        public static string FormatarMoeda(this decimal valor)
        {
            return valor.ToString("R$ ,0.00;R$ -,0.00");
        }

        public static string FormatarPorcentagem(this decimal valor)
        {
            return valor.ToString("N2") + "%";
        }

        public static decimal Arredondar(this decimal valor, int decimais)
        {
            return Math.Round(valor, decimais);
        }

        public static decimal Truncar(this decimal valor, int decimals)
        {
            var fator = 1;
            for (int i = 0; i < decimals; i++)
                fator = fator * 10;

            return Math.Truncate(fator * valor) / fator;
        }
    }
}
