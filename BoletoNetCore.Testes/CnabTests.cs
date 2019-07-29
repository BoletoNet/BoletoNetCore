using System;
using System.Linq;
using BoletoNetCore.Extensions;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Testes Cnab")]
    public class CnabTests
    {
        [Test]
        [TestCase("02", "0617000000")]
        [TestCase("02", "0617")]
        [TestCase("02", "0617A6")]
        public void MotivoOcorrenciaDescricao(string codMovimentoRetorno, string codMotivo)
        {
            var motivos = Cnab.MotivoOcorrenciaCnab240(codMotivo, codMovimentoRetorno).ToList();
            Assert.IsTrue(motivos.Any( q=> q == "Tipo / Número de Inscrição do Beneficiário Inválidos"));
            Assert.IsTrue(motivos.Any(q => q == "Data de Vencimento Anterior a Data de Emissão"));
        }

        [TestCase("02", "06171915A6")]
        [TestCase("02", "0617A6")]
        [TestCase("02", "08180617A6")]
        public void MotivoOcorrenciaDescricao2(string codMovimentoRetorno, string codMotivo)
        {
            var motivos = Cnab.MotivoOcorrenciaCnab240(codMotivo, codMovimentoRetorno).ToList();
            Assert.IsTrue(motivos.Any(q => q == "Tipo / Número de Inscrição do Beneficiário Inválidos"));
            Assert.IsTrue(motivos.Any(q => q == "Data de Vencimento Anterior a Data de Emissão"));
            Assert.IsTrue(motivos.Any(q => q == "Código do Convenente Inválido ou Encerrado"));
        }

        [TestCase("02", "0000000000")]
        [TestCase("06", "")]
        public void MotivoOcorrenciaDescricaoVazio(string codMovimentoRetorno, string codMotivo)
        {
            var motivos = Cnab.MotivoOcorrenciaCnab240(codMotivo, codMovimentoRetorno).ToList();
            Assert.IsTrue(motivos.Count == 0);
            Assert.AreEqual(string.Join(",", motivos), "");
        }

        [TestCase("02", "000000001")]
        [TestCase("04", "000000002")]
        [TestCase("04", "002000002")]
        [TestCase("06", "1")]
        public void MotivoOcorrenciaDescricaoIncorretoNaoQuebra(string codMovimentoRetorno, string codMotivo)
        {
            var motivos = Cnab.MotivoOcorrenciaCnab240(codMotivo, codMovimentoRetorno).ToList();
            Assert.IsTrue(motivos.Count == 0);
            Assert.AreEqual(string.Join(",", motivos), "");
        }

        /// <summary>
        /// Em inconsistências parciais, a função tenta ler dentro do possível até o padrão de inconsistência encontrado
        /// </summary>
        /// <param name="codMovimentoRetorno"></param>
        /// <param name="codMotivo"></param>
        [TestCase("06", "000200002")]
        public void MotivoOcorrenciaDescricaoParcialmenteIncorretoNaoQuebra(string codMovimentoRetorno, string codMotivo)
        {
            var motivos = Cnab.MotivoOcorrenciaCnab240(codMotivo, codMovimentoRetorno).ToList();
            Assert.IsTrue(motivos.Count == 1);
        }


    }
}
