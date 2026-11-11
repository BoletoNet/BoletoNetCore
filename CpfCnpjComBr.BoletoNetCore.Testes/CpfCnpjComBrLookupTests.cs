using System;
using System.Net;
using System.Net.Http;
using CpfCnpjComBr.BoletoNetCore;
using NUnit.Framework;

namespace CpfCnpjComBr.BoletoNetCore.Testes
{
    [TestFixture]
    public class CpfCnpjComBrLookupTests
    {
        private const string RespostaCpf =
            "{\"status\":1,\"cpf\":\"390.533.447-05\",\"nome\":\"Test Token\",\"nascimento\":\"31/12/1900\"," +
            "\"endereco\":\"Rua A\",\"numero\":\"100 B\",\"complemento\":\"Apto 03\",\"bairro\":\"Centro\"," +
            "\"cep\":\"99999123\",\"cidade\":\"Sao Paulo\",\"uf\":\"SP\",\"ibge\":\"1234567\",\"pacoteUsado\":3}";

        private const string RespostaCnpj =
            "{\"status\":1,\"cnpj\":\"27.272.134/0001-18\",\"tipo\":\"Matriz\",\"razao\":\"TOKEN TEST LTDA\"," +
            "\"fantasia\":\"TOKEN TEST\",\"matrizEndereco\":{\"cep\":\"39400-111\",\"tipo\":\"Avenida\"," +
            "\"logradouro\":\"das Flores\",\"numero\":\"1\",\"complemento\":\"Sala 1\",\"bairro\":\"Centro\"," +
            "\"cidade\":\"Montes Claros\",\"uf\":\"MG\"},\"pacoteUsado\":5}";

        private const string RespostaErro =
            "{\"status\":0,\"cpf\":\"\",\"nome\":null,\"erro\":\"CPF invalido!\",\"pacoteUsado\":3,\"erroCodigo\":100}";

        private static CpfCnpjComBrLookup CriarLookup(FakeHandler handler)
        {
            var http = new HttpClient(handler);
            var options = new CpfCnpjComBrOptions { Token = "token-teste" };
            return new CpfCnpjComBrLookup(http, options);
        }

        [Test]
        public void Consultar_cpf_mapeiaCamposEUsaPacote3()
        {
            var handler = new FakeHandler(RespostaCpf);
            var lookup = CriarLookup(handler);

            var r = lookup.ConsultarAsync("390.533.447-05").GetAwaiter().GetResult();

            Assert.Multiple(() =>
            {
                Assert.That(r.EhPessoaFisica, Is.True);
                Assert.That(r.Documento, Is.EqualTo("39053344705"));
                Assert.That(r.Nome, Is.EqualTo("Test Token"));
                Assert.That(r.Logradouro, Is.EqualTo("Rua A"));
                Assert.That(r.Numero, Is.EqualTo("100 B"));
                Assert.That(r.Complemento, Is.EqualTo("Apto 03"));
                Assert.That(r.Bairro, Is.EqualTo("Centro"));
                Assert.That(r.Cidade, Is.EqualTo("Sao Paulo"));
                Assert.That(r.Uf, Is.EqualTo("SP"));
                Assert.That(r.Cep, Is.EqualTo("99999123"));
                Assert.That(handler.UltimaUrl.AbsoluteUri, Does.Contain("/token-teste/3/39053344705"));
            });
        }

        [Test]
        public void Consultar_cnpj_concatenaTipoLogradouroEUsaPacote5()
        {
            var handler = new FakeHandler(RespostaCnpj);
            var lookup = CriarLookup(handler);

            var r = lookup.ConsultarAsync("27272134000118").GetAwaiter().GetResult();

            Assert.Multiple(() =>
            {
                Assert.That(r.EhPessoaFisica, Is.False);
                Assert.That(r.Documento, Is.EqualTo("27272134000118"));
                Assert.That(r.Nome, Is.EqualTo("TOKEN TEST LTDA"));
                Assert.That(r.Logradouro, Is.EqualTo("Avenida das Flores"));
                Assert.That(r.Bairro, Is.EqualTo("Centro"));
                Assert.That(r.Cidade, Is.EqualTo("Montes Claros"));
                Assert.That(r.Uf, Is.EqualTo("MG"));
                Assert.That(r.Cep, Is.EqualTo("39400111"), "CEP deve ficar apenas com digitos");
                Assert.That(handler.UltimaUrl.AbsoluteUri, Does.Contain("/token-teste/5/27272134000118"));
            });
        }

        [Test]
        public void Consultar_statusZero_lancaExcecaoComCodigo()
        {
            var handler = new FakeHandler(RespostaErro, HttpStatusCode.BadRequest);
            var lookup = CriarLookup(handler);

            var ex = Assert.Throws<CpfCnpjComBrException>(
                () => lookup.ConsultarAsync("39053344705").GetAwaiter().GetResult());

            Assert.Multiple(() =>
            {
                Assert.That(ex.Message, Does.Contain("CPF invalido"));
                Assert.That(ex.CodigoErro, Is.EqualTo(100));
            });
        }

        [Test]
        public void Consultar_documentoInvalido_naoChamaRedeELancaArgumentException()
        {
            var handler = new FakeHandler(RespostaErro);
            var lookup = CriarLookup(handler);

            Assert.Throws<ArgumentException>(
                () => lookup.ConsultarAsync("123456").GetAwaiter().GetResult());
            Assert.That(handler.UltimaUrl, Is.Null, "documento invalido nao deve gerar chamada HTTP");
        }

        [TestCase("R", "das Flores", "R das Flores")]
        [TestCase("Rua", "das Flores", "Rua das Flores")]
        [TestCase("Rua", "Rua A", "Rua A")]
        [TestCase("R", "R DAS FLORES", "R DAS FLORES")]
        [TestCase("R", "Raimundo Nonato", "R Raimundo Nonato")]
        [TestCase("", "Avenida Brasil", "Avenida Brasil")]
        [TestCase("R", "Rua das Flores", "Rua das Flores")]   // tipo abreviado vs logradouro por extenso
        [TestCase("Av", "Avenida Brasil", "Avenida Brasil")]  // idem para avenida
        [TestCase("AV", "Av Brasil", "Av Brasil")]
        public void MontarLogradouro_respeitaAbreviacoes(string tipo, string logradouro, string esperado)
        {
            Assert.That(CpfCnpjComBrLookup.MontarLogradouro(tipo, logradouro), Is.EqualTo(esperado));
        }
    }
}
