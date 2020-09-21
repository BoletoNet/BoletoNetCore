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
        public static long FatorVencimento(this DateTime data)
        {
            var dateBase = new DateTime(1997, 10, 7, 0, 0, 0);

            // Verifica se a data esta dentro do range utilizavel
            var rangeUtilizavel = DateTime.Now.Date.DateDiff(data, DateInterval.Day);

            if (rangeUtilizavel > 5500 || rangeUtilizavel < -3500)
                throw new Exception("Data do vencimento ("+data.ToString()+") fora do range de utilização proposto pela CENEGESC. Comunicado FEBRABAN de n° 082/2012 de 14/06/2012");

            while (data > dateBase.AddDays(9999))
                dateBase = data.AddDays(-(dateBase.DateDiff(data, DateInterval.Day) - 9999 - 1 + 1000));

            return dateBase.DateDiff(data, DateInterval.Day);
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