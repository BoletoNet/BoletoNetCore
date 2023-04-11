using System;
using System.Linq;
using BoletoNetCore.Extensions;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Outros Testes")]
    public class DateTimeExtensionsTest
    {
        [Test]
        public void GeracaoCorretaDeFatorVencimento()
        {
            var inicio = new DateTime(1997, 10, 07, 0, 0, 0);
            var ajusteRange = (DateTime.Now - inicio).Days - 3000;
            inicio = inicio.AddDays(ajusteRange);
            var totalDiasAnalisados = (new DateTime(2033, 08, 15, 0, 0, 0) - inicio).Days;

            for (int i = 0; i < totalDiasAnalisados; i++)
            {
                var fatorVencimento = ajusteRange + i;
                var dateTime = inicio.AddDays(i);

                Assert.AreEqual(fatorVencimento, dateTime.FatorVencimento());
                if (fatorVencimento == 9999)
                    ajusteRange = 999 - i;
            }

        }

        /// <summary>
        /// Em 21/02/2025 o fator atingirá o limite 9999
        /// Em 22/02/2025 deverá retornar para o fator 1000.
        /// Nessa transição, a data base de cálculo será 29/05/2022.
        /// Obs: Somente poderá emitir boleto com vencimento até 10 anos a posterior.
        /// </summary>
        [Test]
        public void Deve_retornar_fator_9999_em_21_02_2025()
        { 
            var vencimentoFator9999 = new DateTime(2025, 02, 21, 0, 0, 0);
            Assert.AreEqual(9999, vencimentoFator9999.FatorVencimento());
            
        }

        /// <summary>
        /// Em 21/02/2025 o fator atingirá o limite 9999
        /// Em 22/02/2025 deverá retornar para o fator 1000.
        /// Nessa transição, a data base de cálculo será 29/05/2022.
        /// Obs: Somente poderá emitir boleto com vencimento até 10 anos a posterior.
        /// </summary>
        [Test]
        public void Deve_retornar_fator_1000_em_22_02_2025()
        { 
            var vencimentoFator1000 = new DateTime(2025, 02, 22, 0, 0, 0);
            Assert.AreEqual(1000, vencimentoFator1000.FatorVencimento());
        }
    }
}
