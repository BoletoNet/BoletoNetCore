using System;

namespace BoletoNetCore.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        ///     Fator de vencimento do boleto.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Retorno Fator de Vencimento</returns>
        /// <remarks>
        ///     Wellington(wcarvalho@novatela.com.br)
        ///     Com base na proposta feita pela CENEGESC de acordo com o comunicado FEBRABAN de n° 082/2012 de 14/06/2012 segue
        ///     regra para implantação.
        ///     No dia 21/02/2025 o fator vencimento chegará em 9999 assim atigindo o tempo de utilização, para contornar esse
        ///     problema foi definido com uma nova regra
        ///     de utilizaçao criando um range de uso o range funcionara controlando a emissão dos boletos.
        ///     Exemplo:
        ///     Data Atual: 12/03/2014 = 6000
        ///     Para os boletos vencidos, anterior a data atual é de 3000 fatores cerca de =/- 8 anos. Os boletos que forem gerados
        ///     acima dos 3000 não serão aceitos pelas instituições financeiras.
        ///     Para os boletos a vencer, posterior a data atual é de 5500 fatores cerca de +/- 15 anos. Os boletos que forem
        ///     gerados acima dos 5500 não serão aceitos pelas instituições financeiras.
        ///     Quando o fator de vencimento atingir 9999 ele retorna para 1000
        ///     Exemplo:
        ///     21/02/2025 = 9999
        ///     22/02/2025 = 1000
        ///     23/02/2025 = 1001
        ///     ...
        ///     05/03/2025 = 1011
        /// </remarks>
        //public static long FatorVencimento(this DateTime data)
        //{
        //    var dateBase = new DateTime(1997, 10, 7, 0, 0, 0);

        //    // Verifica se a data esta dentro do range utilizavel
        //    var rangeUtilizavel = DateTime.Now.Date.DateDiff(data, DateInterval.Day);

        //    if (rangeUtilizavel > 5500 || rangeUtilizavel < -5000)
        //        throw new Exception("Data do vencimento ("+data.ToString()+") fora do range de utilização proposto pela CENEGESC. Comunicado FEBRABAN de n° 082/2012 de 14/06/2012");

        //    while (data > dateBase.AddDays(9999))
        //        dateBase = data.AddDays(-(dateBase.DateDiff(data, DateInterval.Day) - 9999 - 1 + 1000));

        //    return dateBase.DateDiff(data, DateInterval.Day);
        //}
        public static long FatorVencimento(this DateTime data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "A data de vencimento não pode ser nula.");

            // Remove informações de hora para garantir o cálculo exato de dias
            DateTime dataVencimento = data.Date;
            DateTime dataDivisor = new DateTime(2025, 2, 21);

            if (dataVencimento <= dataDivisor)
            {
                // Regra Antiga (Vencimentos até 21/02/2025)
                DateTime dateBaseAntiga = new DateTime(1997, 10, 7);
                long fator = (long)(dataVencimento - dateBaseAntiga).TotalDays;

                if (fator < 0)
                    throw new Exception("Data de vencimento anterior à data base de 1997.");

                return fator;
            }
            else
            {
                // Nova Regra FEBRABAN (Vencimentos a partir de 22/02/2025)
                // O fator reinicia em 1000 no dia 22/02/2025.
                DateTime dateBaseNova = new DateTime(2025, 2, 22);
                long fator = 1000 + (long)(dataVencimento - dateBaseNova).TotalDays;

                // O novo teto é 9999 (acontecerá em 13/10/2049)
                if (fator > 9999)
                    throw new Exception("Data de vencimento além do limite permitido pela nova regra da FEBRABAN (Fator > 9999).");

                return fator;
            }
        }

        internal static long DateDiff(this DateTime startDate, DateTime endDate, DateInterval interval)
        {
            var timeSpan = endDate - startDate;
            switch (interval)
            {
                case DateInterval.Day:
                    return timeSpan.Days;
                case DateInterval.Hour:
                    return (long)timeSpan.TotalHours;
                case DateInterval.Minute:
                    return (long)timeSpan.TotalMinutes;
                case DateInterval.Month:
                    return timeSpan.Days / 30;
                case DateInterval.Quarter:
                    return (timeSpan.Days / 30) / 3;
                case DateInterval.Second:
                    return (long)timeSpan.TotalSeconds;
                case DateInterval.Week:
                    return timeSpan.Days / 7;
                case DateInterval.Year:
                    return timeSpan.Days / 365;
                default:
                    throw new ArgumentException("Intervalo não suportado");
            }
        }

    }
}