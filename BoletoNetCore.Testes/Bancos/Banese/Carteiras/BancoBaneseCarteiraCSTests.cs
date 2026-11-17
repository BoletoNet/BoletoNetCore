using NUnit.Framework;
using System;
using System.IO;
using Wkhtmltopdf.NetCore;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Banese Carteira CS")]
    public class BancoBaneseCarteiraCSTests
    {
        readonly IBanco _banco;

        public BancoBaneseCarteiraCSTests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "0034",
                DigitoAgencia = "",
                Conta = "123456",
                DigitoConta = "",
                CarteiraPadrao = "CS",
                VariacaoCarteiraPadrao = "",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.Banese);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("123456", "", "", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Banese_CS_PDF_Html()
        {
            var boleto = TestUtils.GerarBoleto(_banco, 1, "N", 1, "1/1");
            boleto.ValidarDados();
            var html = new BoletoBancario { Boleto = boleto, OcultarInstrucoes = false }.MontaHtmlEmbedded();
            Assert.That(html, Does.Contain("047-7"));
            Assert.That(html, Does.Contain("PAGÁVEL PREFERENCIALMENTE NA REDE BANESE"));
        }

        [TestCase(123.45, "1", "00000001-3", "04794350000000123453400012345600000001304726", "04793.40002 12345.600006 00013.047261 4 35000000012345", 2007, 5, 8)]
        [TestCase(0.01, "2", "00000002-1", "04791360000000000013400012345600000002104720", "04793.40002 12345.600006 00021.047204 1 36000000000001", 2007, 8, 16)]
        [TestCase(99999.99, "3", "00000003-0", "04793370000099999993400012345600000003004711", "04793.40002 12345.600006 00030.047112 3 37000009999999", 2007, 11, 24)]
        [TestCase(7.77, "4", "00000004-8", "04796380000000007773400012345600000004804711", "04793.40002 12345.600006 00048.047112 6 38000000000777", 2008, 3, 3)]
        [TestCase(25.00, "5", "00000005-6", "04791410000000025003400012345600000005604708", "04793.40002 12345.600006 00056.047087 1 41000000002500", 2008, 12, 28)]
        public void Deve_criar_boleto_banese_cs_com_nosso_numero_codigo_de_barras_e_linha_digitavel_validos(decimal valorTitulo, string nossoNumero, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = "NF" + nossoNumero,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

            boleto.ValidarDados();

            Assert.That(boleto.NossoNumeroFormatado, Is.EqualTo(nossoNumeroFormatado), "Nosso número inválido");
            Assert.That(boleto.CodigoBarra.CodigoDeBarras, Is.EqualTo(codigoDeBarras), "Código de Barras inválido");
            Assert.That(boleto.CodigoBarra.LinhaDigitavel, Is.EqualTo(linhaDigitavel), "Linha digitável inválida");
        }

        /// <summary>
        /// Teste manual: reproduz o boleto do ASP clássico (boleto-Banese.asp) e grava HTML e PDF em %TEMP%\BoletoNetCore
        /// para comparar visualmente com os demais bancos (que gravam na mesma pasta via TestarHomologacao).
        /// Execute com: dotnet test --filter "Name=Banese_CS_Manual_PDF"
        /// </summary>
        [Test, Explicit("Teste manual - gera arquivos na pasta Temp/BoletoNetCore")]
        public void Banese_CS_Manual_PDF()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "0002",
                Conta = "031039820",
                CarteiraPadrao = "CS",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            var banco = Banco.Instancia(Bancos.Banese);
            banco.Beneficiario = new Beneficiario
            {
                CPFCNPJ = "00.000.000/0001-91",
                Nome = "JAMSOFT SISTEMAS DE INFORMATICA LTDA",
                Codigo = "031039820",
                Endereco = new Endereco
                {
                    LogradouroEndereco = "AV. LOURIVAL BATISTA",
                    LogradouroNumero = "294",
                    Bairro = "CENTRO",
                    Cidade = "NOSSA SENHORA DA GLORIA",
                    UF = "SE",
                    CEP = "49680000"
                },
                ContaBancaria = contaBancaria
            };
            banco.FormataBeneficiario();

            var boleto = new Boleto(banco)
            {
                Pagador = new Pagador
                {
                    CPFCNPJ = "23.738.246/0001-60",
                    Nome = "CLIENTE RAZAO SOCIAL 32367",
                    Observacoes = "Vend.: Cristiane (56)",
                    Endereco = new Endereco
                    {
                        LogradouroEndereco = "ENDERECO COB TESTE, 32367",
                        Bairro = "ZONA RURAL",
                        Cidade = "PORTO DA FOLHA",
                        UF = "SE",
                        CEP = "49800000"
                    }
                },
                DataEmissao = new DateTime(2026, 8, 13),
                DataProcessamento = new DateTime(2026, 8, 13),
                DataVencimento = new DateTime(2026, 9, 25),
                ValorTitulo = 4445120.00m,
                NossoNumero = "100053",
                NumeroDocumento = "PR328362 01",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Aceite = "N",
                MensagemInstrucoesCaixa = "Mora por dia de atraso R$ 7.408,53"
            };
            boleto.ValidarDados();

            var pasta = Path.Combine(Path.GetTempPath(), "BoletoNetCore");
            Directory.CreateDirectory(pasta);
            var arquivoHtml = Path.Combine(pasta, "BancoBaneseCarteiraCSTests_Manual_CNAB400.html");
            var arquivoPdf = Path.ChangeExtension(arquivoHtml, ".PDF");

            var html = "<div style=\"page-break-after: always;\">" + new BoletoBancario
            {
                Boleto = boleto,
                OcultarInstrucoes = false,
                MostrarComprovanteEntrega = false,
                MostrarEnderecoBeneficiario = true,
                ExibirDemonstrativo = true
            }.MontaHtmlEmbedded() + "</div>";
            File.WriteAllText(arquivoHtml, html);

            RotativaConfiguration.Setup();
            File.WriteAllBytes(arquivoPdf, new Wkhtmltopdf.NetCore.HtmlAsPdf().GetPDF(html));

            TestContext.WriteLine($"Linha digitável: {boleto.CodigoBarra.LinhaDigitavel}");
            TestContext.WriteLine($"Código de barras: {boleto.CodigoBarra.CodigoDeBarras}");
            TestContext.WriteLine($"PDF: {arquivoPdf}");
            Assert.That(File.Exists(arquivoPdf));
        }

        [Test]
        public void Deve_rejeitar_nosso_numero_com_mais_de_8_digitos()
        {
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(2008, 3, 3),
                ValorTitulo = 10,
                NossoNumero = "123456789",
                NumeroDocumento = "NF1",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };
            Assert.That(() => boleto.ValidarDados(), Throws.Exception);
        }
    }
}
