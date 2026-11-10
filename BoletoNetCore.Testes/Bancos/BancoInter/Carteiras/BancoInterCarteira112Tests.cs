using NUnit.Framework;
using System;
using System.Linq;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Banco Inter Carteira 112")]
    public class BancoInterCarteira112Tests
    {
        // Conta real da BlueCasa (banco 077, carteira 112). Código da empresa = 1425014 (7 dígitos).
        // Banco.Instancia devolve uma instância compartilhada: o beneficiário é preparado a cada teste.
        IBanco _banco;

        [SetUp]
        public void Preparar()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "0001",
                DigitoAgencia = "9",
                Conta = "48646340",
                DigitoConta = "0",
                CarteiraPadrao = "112",
                VariacaoCarteiraPadrao = "",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.BancoInter);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("48646340", "0", "1425014", contaBancaria);
            _banco.FormataBeneficiario();
        }

        private Boleto CriarBoleto(decimal valorTitulo, string nossoNumero, DateTime vencimento)
        {
            return new Boleto(_banco)
            {
                DataVencimento = vencimento,
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };
        }

        private static string SomenteDigitos(string texto) => new string(texto.Where(char.IsDigit).ToArray());

        // Valores registrados pelo próprio Banco Inter (oráculo: f_boleto da conta 005 da BlueCasa).
        [TestCase(10.05, "90481190281", "07791126100000010050001112142501490481190281", "07790001161214250149804811902818112610000001005", 2025, 11, 10)]
        [TestCase(502.09, "90497376247", "07791127700000502090001112142501490497376247", "07790001161214250149804973762471112770000050209", 2025, 11, 26)]
        [TestCase(502.09, "90497376304", "07794128100000502090001112142501490497376304", "07790001161214250149804973763040412810000050209", 2025, 11, 30)]
        public void Deve_criar_boleto_BancoInter_carteira112_com_codigo_de_barras_igual_ao_registrado_no_banco(decimal valorTitulo, string nossoNumero, string codigoDeBarrasEsperado, string linhaDigitavelEsperada, int ano, int mes, int dia)
        {
            var boleto = CriarBoleto(valorTitulo, nossoNumero, new DateTime(ano, mes, dia));

            boleto.ValidarDados();

            Assert.That(boleto.CodigoBarra.CodigoDeBarras, Is.EqualTo(codigoDeBarrasEsperado), "Código de barras diferente do registrado pelo Banco Inter");
            Assert.That(SomenteDigitos(boleto.CodigoBarra.LinhaDigitavel), Is.EqualTo(linhaDigitavelEsperada), "Linha digitável diferente da registrada pelo Banco Inter");
            Assert.That(boleto.CodigoBarra.CampoLivre, Has.Length.EqualTo(25), "Campo livre deve conter 25 dígitos");
            Assert.That(boleto.CodigoBarra.CampoLivre, Is.EqualTo("0001112" + "1425014" + nossoNumero), "Campo livre = agência + carteira + código da empresa + nosso número");
        }

        [Test]
        public void Deve_gerar_campo_livre_de_25_digitos_para_nosso_numero_de_11_digitos_do_defeito()
        {
            // Caso reproduzido no defeito: antes gerava "0001112909202151290" (19 dígitos).
            var boleto = CriarBoleto(10.00m, "90920215129", new DateTime(2025, 11, 10));
            boleto.ValidarDados();

            var campoLivre = _banco.FormataCodigoBarraCampoLivre(boleto);

            Assert.That(campoLivre, Is.EqualTo("0001112142501490920215129"));
            Assert.That(campoLivre, Has.Length.EqualTo(25));
        }

        [Test]
        public void Deve_formatar_nosso_numero_de_11_digitos_sem_recalcular_digito_verificador()
        {
            var boleto = CriarBoleto(10.05m, "90481190281", new DateTime(2025, 11, 10));

            boleto.ValidarDados();

            Assert.That(boleto.NossoNumero, Is.EqualTo("90481190281"), "Nosso número de 11 dígitos não deve ser alterado");
            Assert.That(boleto.NossoNumeroDV, Is.EqualTo("1"), "DV deve ser o último dígito do nosso número informado pelo Inter");
            Assert.That(boleto.NossoNumeroFormatado, Is.EqualTo("00019/112/9048119028-1"));
        }

        [Test]
        public void Deve_manter_composicao_de_25_digitos_para_nosso_numero_de_10_digitos()
        {
            // Fluxo legado de 10 dígitos: continua gerando campo livre de 25 dígitos com o código da empresa.
            var boleto = CriarBoleto(5.00m, "4309540", new DateTime(2023, 8, 31));
            boleto.ValidarDados();

            var campoLivre = _banco.FormataCodigoBarraCampoLivre(boleto);

            Assert.That(campoLivre, Has.Length.EqualTo(25));
            Assert.That(campoLivre, Does.StartWith("0001112" + "1425014"));
        }

        [Test]
        public void Deve_informar_quando_codigo_da_empresa_nao_tem_7_digitos()
        {
            _banco.Beneficiario.CodigoTransmissao = "";
            var boleto = CriarBoleto(10.00m, "90481190281", new DateTime(2025, 11, 10));

            Assert.That(() => boleto.ValidarDados(),
                Throws.Exception.With.Message.Contains("7 dígitos"));
        }
    }
}
