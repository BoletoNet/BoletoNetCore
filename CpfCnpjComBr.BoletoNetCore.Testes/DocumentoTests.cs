using System;
using CpfCnpjComBr.BoletoNetCore;
using NUnit.Framework;

namespace CpfCnpjComBr.BoletoNetCore.Testes
{
    [TestFixture]
    public class DocumentoTests
    {
        [TestCase("390.533.447-05", "39053344705")]
        [TestCase("27.272.134/0001-18", "27272134000118")]
        [TestCase("12.abc.345/01de-35", "12ABC34501DE35")]
        public void Normalizar_removeSeparadores_eColocaEmMaiusculas(string entrada, string esperado)
        {
            Assert.That(Documento.Normalizar(entrada), Is.EqualTo(esperado));
        }

        [Test]
        public void Normalizar_nulo_retornaVazio()
        {
            Assert.That(Documento.Normalizar(null), Is.EqualTo(string.Empty));
        }

        [TestCase("39053344705")]
        [TestCase("11144477735")]
        public void ValidarCpf_cpfValido_retornaVerdadeiro(string cpf)
        {
            Assert.That(Documento.ValidarCpf(cpf), Is.True);
        }

        [TestCase("39053344700")]  // digito verificador errado
        [TestCase("11111111111")]  // todos iguais
        [TestCase("123")]          // tamanho errado
        public void ValidarCpf_cpfInvalido_retornaFalso(string cpf)
        {
            Assert.That(Documento.ValidarCpf(cpf), Is.False);
        }

        [Test]
        public void ValidarCnpj_numericoValido_retornaVerdadeiro()
        {
            Assert.That(Documento.ValidarCnpj("27272134000118"), Is.True);
        }

        [Test]
        public void ValidarCnpj_alfanumericoValido_retornaVerdadeiro()
        {
            // CNPJ alfanumerico com digitos verificadores calculados pela regra ord-48.
            Assert.That(Documento.ValidarCnpj("12ABC34501DE35"), Is.True);
        }

        [TestCase("27272134000119")]  // digito verificador errado
        [TestCase("00000000000000")]  // todos iguais
        [TestCase("2727213400011")]   // tamanho errado
        [TestCase("12ABC34501DEAB")]  // digito verificador nao numerico
        public void ValidarCnpj_invalido_retornaFalso(string cnpj)
        {
            Assert.That(Documento.ValidarCnpj(cnpj), Is.False);
        }

        [Test]
        public void NormalizarEValidar_documentoComMascara_devolveLimpo()
        {
            Assert.That(Documento.NormalizarEValidar("390.533.447-05"), Is.EqualTo("39053344705"));
            Assert.That(Documento.NormalizarEValidar("27.272.134/0001-18"), Is.EqualTo("27272134000118"));
        }

        [Test]
        public void NormalizarEValidar_documentoInvalido_lancaArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Documento.NormalizarEValidar("123456"));
        }
    }
}
