using System.Threading;
using System.Threading.Tasks;
using BoletoNetCore;
using CpfCnpjComBr.BoletoNetCore;
using NUnit.Framework;

namespace CpfCnpjComBr.BoletoNetCore.Testes
{
    [TestFixture]
    public class PagadorResolverTests
    {
        private sealed class LookupFalso : IPessoaLookup
        {
            private readonly PessoaResultado _resultado;
            public LookupFalso(PessoaResultado resultado) => _resultado = resultado;

            public Task<PessoaResultado> ConsultarAsync(string documento, CancellationToken cancellationToken = default)
                => Task.FromResult(_resultado);
        }

        private static PessoaResultado ResultadoCnpj() => new PessoaResultado
        {
            Documento = "27272134000118",
            EhPessoaFisica = false,
            Nome = "TOKEN TEST LTDA",
            Logradouro = "Avenida das Flores",
            Numero = "1",
            Complemento = "Sala 1",
            Bairro = "Centro",
            Cidade = "Montes Claros",
            Uf = "MG",
            Cep = "39400111",
        };

        [Test]
        public void ResolverPagador_preencheTodosOsCampos()
        {
            var resolver = new PagadorResolver(new LookupFalso(ResultadoCnpj()));

            var pagador = resolver.ResolverPagadorAsync("27.272.134/0001-18").GetAwaiter().GetResult();

            Assert.Multiple(() =>
            {
                Assert.That(pagador.Nome, Is.EqualTo("TOKEN TEST LTDA"));
                Assert.That(pagador.CPFCNPJ, Is.EqualTo("27272134000118"));
                Assert.That(pagador.TipoCPFCNPJ("A"), Is.EqualTo("J"));
                Assert.That(pagador.Endereco.LogradouroEndereco, Is.EqualTo("Avenida das Flores"));
                Assert.That(pagador.Endereco.LogradouroNumero, Is.EqualTo("1"));
                Assert.That(pagador.Endereco.Bairro, Is.EqualTo("Centro"));
                Assert.That(pagador.Endereco.Cidade, Is.EqualTo("Montes Claros"));
                Assert.That(pagador.Endereco.UF, Is.EqualTo("MG"));
                Assert.That(pagador.Endereco.CEP, Is.EqualTo("39400111"));
            });
        }

        [Test]
        public void Preencher_naoDestrutivo_preservaCamposJaInformados()
        {
            var pagador = new Pagador
            {
                Nome = "Nome ja definido",
                Endereco = new Endereco { LogradouroComplemento = "Fundos" },
            };

            pagador.PreencherAsync(new LookupFalso(ResultadoCnpj()), "27272134000118", sobrescrever: false)
                   .GetAwaiter().GetResult();

            Assert.Multiple(() =>
            {
                Assert.That(pagador.Nome, Is.EqualTo("Nome ja definido"), "nao deve sobrescrever nome existente");
                Assert.That(pagador.Endereco.LogradouroComplemento, Is.EqualTo("Fundos"), "nao deve sobrescrever complemento existente");
                Assert.That(pagador.Endereco.Cidade, Is.EqualTo("Montes Claros"), "deve preencher campo vazio");
                Assert.That(pagador.CPFCNPJ, Is.EqualTo("27272134000118"));
            });
        }

        [Test]
        public void Preencher_naoDestrutivo_preservaCpfcnpjJaInformado()
        {
            var pagador = new Pagador { CPFCNPJ = "39053344705" };

            pagador.PreencherAsync(new LookupFalso(ResultadoCnpj()), "27272134000118", sobrescrever: false)
                   .GetAwaiter().GetResult();

            Assert.That(pagador.CPFCNPJ, Is.EqualTo("39053344705"), "nao deve sobrescrever o documento existente");
        }

        [Test]
        public void Preencher_sobrescrever_substituiTudo()
        {
            var pagador = new Pagador { Nome = "Antigo" };

            pagador.PreencherAsync(new LookupFalso(ResultadoCnpj()), "27272134000118", sobrescrever: true)
                   .GetAwaiter().GetResult();

            Assert.That(pagador.Nome, Is.EqualTo("TOKEN TEST LTDA"));
        }
    }
}
