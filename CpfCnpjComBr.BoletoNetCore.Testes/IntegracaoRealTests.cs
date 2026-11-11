using System;
using BoletoNetCore;
using CpfCnpjComBr.BoletoNetCore;
using NUnit.Framework;

namespace CpfCnpjComBr.BoletoNetCore.Testes
{
    /// <summary>
    /// Testes de ponta a ponta contra a API real cpfcnpj.com.br usando o token de demonstracao.
    /// Categoria "Integracao" para poder ser filtrado em CI offline
    /// (dotnet test --filter TestCategory!=Integracao).
    /// </summary>
    [TestFixture]
    [Category("Integracao")]
    public class IntegracaoRealTests
    {
        private const string TokenDemo = "5ae973d7a997af13f0aaf2bf60e65803";

        private static CpfCnpjComBrLookup Lookup() => new CpfCnpjComBrLookup(TokenDemo);

        [Test]
        public void Cpf_real_devolvePagadorPreenchido()
        {
            var resolver = new PagadorResolver(Lookup());
            var pagador = resolver.ResolverPagadorAsync("390.533.447-05").GetAwaiter().GetResult();

            Assert.Multiple(() =>
            {
                Assert.That(pagador.CPFCNPJ, Is.EqualTo("39053344705"));
                Assert.That(pagador.TipoCPFCNPJ("A"), Is.EqualTo("F"));
                Assert.That(pagador.Nome, Is.Not.Empty);
                Assert.That(pagador.Endereco.LogradouroEndereco, Is.Not.Empty);
                Assert.That(pagador.Endereco.UF, Is.Not.Empty);
            });
        }

        [Test]
        public void Cnpj_real_devolvePagadorPreenchido()
        {
            var resolver = new PagadorResolver(Lookup());
            var pagador = resolver.ResolverPagadorAsync("27.272.134/0001-18").GetAwaiter().GetResult();

            Assert.Multiple(() =>
            {
                Assert.That(pagador.CPFCNPJ, Is.EqualTo("27272134000118"));
                Assert.That(pagador.TipoCPFCNPJ("A"), Is.EqualTo("J"));
                Assert.That(pagador.Nome, Is.Not.Empty);
                Assert.That(pagador.Endereco.Cidade, Is.Not.Empty);
                Assert.That(pagador.Endereco.CEP, Is.Not.Empty);
            });
        }

        [Test]
        public void Cpf_invalido_lancaExcecaoDaApi()
        {
            var lookup = Lookup();
            // documento com formato de CPF, porem digito verificador invalido:
            // barrado pela nossa validacao antes de chamar a rede.
            Assert.Throws<ArgumentException>(
                () => lookup.ConsultarAsync("00000000000").GetAwaiter().GetResult());
        }
    }
}
