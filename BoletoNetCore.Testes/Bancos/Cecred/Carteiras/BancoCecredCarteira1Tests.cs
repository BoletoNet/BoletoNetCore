using System;
using System.IO;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Cecred Carteira 1")]
    public class BancoCecredCarteira1Tests
    {
        readonly IBanco _banco;
        public BancoCecredCarteira1Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "0101",
                DigitoAgencia = "5",
                Conta = "231560",
                DigitoConta = "2",
                CarteiraPadrao = "1",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa,
                CodigoConvenio = "23156489"
            };
            _banco = Banco.Instancia(Bancos.Cecred);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("101104", "", "", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Cecred_1_REM240()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoCecredCarteira1Tests), 5, true, "?", 1);
        }

        [Test]
        public void Cecred_1_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoCecredCarteira1Tests), 5, true, "?", 1);
        }

        [TestCase(80.21, "74644", "02315602/74644", "1", "02315602000074644", "08591875700000080211011040231560200007464401", "08591.01107 40231.560208 00074.644014 1 87570000008021", 2021, 9, 28)]
        [TestCase(187.25, "74641", "02315602/74641", "2", "02315602000074641", "08592875700000187251011040231560200007464101", "08591.01107 40231.560208 00074.641010 2 87570000018725", 2021, 9, 28)]
        [TestCase(49, "75743", "02315602/75743", "3", "02315602000075743", "08593875700000049001011040231560200007574301", "08591.01107 40231.560208 00075.743013 3 87570000004900", 2021, 9, 28)]
        [TestCase(105, "74601", "02315602/74601", "4", "02315602000074601", "08594875700000105001011040231560200007460101", "08591.01107 40231.560208 00074.601014 4 87570000010500", 2021, 9, 28)]
        [TestCase(19.9, "74621", "02315602/74621", "5", "02315602000074621", "08595875700000019901011040231560200007462101", "08591.01107 40231.560208 00074.621012 5 87570000001990", 2021, 9, 28)]
        [TestCase(110, "74643", "02315602/74643", "6", "02315602000074643", "08596875700000110001011040231560200007464301", "08591.01107 40231.560208 00074.643016 6 87570000011000", 2021, 9, 28)]
        [TestCase(200, "74660", "02315602/74660", "7", "02315602000074660", "08597875700000200001011040231560200007466001", "08591.01107 40231.560208 00074.660010 7 87570000020000", 2021, 9, 28)]
        [TestCase(2100.5, "74669", "02315602/74669", "8", "02315602000074669", "08598875700002100501011040231560200007466901", "08591.01107 40231.560208 00074.669011 8 87570000210050", 2021, 9, 28)]
        [TestCase(89.9, "74603", "02315602/74603", "9", "02315602000074603", "08599875700000089901011040231560200007460301", "08591.01107 40231.560208 00074.603010 9 87570000008990", 2021, 9, 28)]
        public void Deve_criar_boleto_cecred_01_com_digito_verificador_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            //Ambiente
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

            //Ação
            boleto.ValidarDados();

            //Assertivas
            Assert.That(boleto.CodigoBarra.DigitoVerificador, Is.EqualTo(digitoVerificador), $"Dígito Verificador diferente de {digitoVerificador}");
        }

        [TestCase(80.21, "74644", "02315602/74644", "1", "02315602000074644", "08591875700000080211011040231560200007464401", "08591.01107 40231.560208 00074.644014 1 87570000008021", 2021, 9, 28)]
        [TestCase(187.25, "74641", "02315602/74641", "2", "02315602000074641", "08592875700000187251011040231560200007464101", "08591.01107 40231.560208 00074.641010 2 87570000018725", 2021, 9, 28)]
        [TestCase(49, "75743", "02315602/75743", "3", "02315602000075743", "08593875700000049001011040231560200007574301", "08591.01107 40231.560208 00075.743013 3 87570000004900", 2021, 9, 28)]
        [TestCase(105, "74601", "02315602/74601", "4", "02315602000074601", "08594875700000105001011040231560200007460101", "08591.01107 40231.560208 00074.601014 4 87570000010500", 2021, 9, 28)]
        [TestCase(19.9, "74621", "02315602/74621", "5", "02315602000074621", "08595875700000019901011040231560200007462101", "08591.01107 40231.560208 00074.621012 5 87570000001990", 2021, 9, 28)]
        [TestCase(110, "74643", "02315602/74643", "6", "02315602000074643", "08596875700000110001011040231560200007464301", "08591.01107 40231.560208 00074.643016 6 87570000011000", 2021, 9, 28)]
        [TestCase(200, "74660", "02315602/74660", "7", "02315602000074660", "08597875700000200001011040231560200007466001", "08591.01107 40231.560208 00074.660010 7 87570000020000", 2021, 9, 28)]
        [TestCase(2100.5, "74669", "02315602/74669", "8", "02315602000074669", "08598875700002100501011040231560200007466901", "08591.01107 40231.560208 00074.669011 8 87570000210050", 2021, 9, 28)]
        [TestCase(89.9, "74603", "02315602/74603", "9", "02315602000074603", "08599875700000089901011040231560200007460301", "08591.01107 40231.560208 00074.603010 9 87570000008990", 2021, 9, 28)]
        public void Deve_criar_boleto_cecred_01_com_nosso_numero_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            //Ambiente
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

            //Ação
            boleto.ValidarDados();

            //Assertivas
            Assert.That(boleto.NossoNumero, Is.EqualTo(nossoNumeroFormatado), "Nosso número inválido");
        }


        [TestCase(80.21, "74644", "02315602/74644", "1", "02315602000074644", "08591875700000080211011040231560200007464401", "08591.01107 40231.560208 00074.644014 1 87570000008021", 2021, 9, 28)]
        [TestCase(187.25, "74641", "02315602/74641", "2", "02315602000074641", "08592875700000187251011040231560200007464101", "08591.01107 40231.560208 00074.641010 2 87570000018725", 2021, 9, 28)]
        [TestCase(49, "75743", "02315602/75743", "3", "02315602000075743", "08593875700000049001011040231560200007574301", "08591.01107 40231.560208 00075.743013 3 87570000004900", 2021, 9, 28)]
        [TestCase(105, "74601", "02315602/74601", "4", "02315602000074601", "08594875700000105001011040231560200007460101", "08591.01107 40231.560208 00074.601014 4 87570000010500", 2021, 9, 28)]
        [TestCase(19.9, "74621", "02315602/74621", "5", "02315602000074621", "08595875700000019901011040231560200007462101", "08591.01107 40231.560208 00074.621012 5 87570000001990", 2021, 9, 28)]
        [TestCase(110, "74643", "02315602/74643", "6", "02315602000074643", "08596875700000110001011040231560200007464301", "08591.01107 40231.560208 00074.643016 6 87570000011000", 2021, 9, 28)]
        [TestCase(200, "74660", "02315602/74660", "7", "02315602000074660", "08597875700000200001011040231560200007466001", "08591.01107 40231.560208 00074.660010 7 87570000020000", 2021, 9, 28)]
        [TestCase(2100.5, "74669", "02315602/74669", "8", "02315602000074669", "08598875700002100501011040231560200007466901", "08591.01107 40231.560208 00074.669011 8 87570000210050", 2021, 9, 28)]
        [TestCase(89.9, "74603", "02315602/74603", "9", "02315602000074603", "08599875700000089901011040231560200007460301", "08591.01107 40231.560208 00074.603010 9 87570000008990", 2021, 9, 28)]
        public void Deve_criar_boleto_cecred_01_com_codigo_de_barras_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            //Ambiente
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

            //Ação
            boleto.ValidarDados();

            //Assertivas
            Assert.That(boleto.CodigoBarra.CodigoDeBarras, Is.EqualTo(codigoDeBarras), "Código de Barra inválido");
        }


        [TestCase(80.21, "74644", "02315602/74644", "1", "02315602000074644", "08591875700000080211011040231560200007464401", "08591.01107 40231.560208 00074.644014 1 87570000008021", 2021, 9, 28)]
        [TestCase(187.25, "74641", "02315602/74641", "2", "02315602000074641", "08592875700000187251011040231560200007464101", "08591.01107 40231.560208 00074.641010 2 87570000018725", 2021, 9, 28)]
        [TestCase(49, "75743", "02315602/75743", "3", "02315602000075743", "08593875700000049001011040231560200007574301", "08591.01107 40231.560208 00075.743013 3 87570000004900", 2021, 9, 28)]
        [TestCase(105, "74601", "02315602/74601", "4", "02315602000074601", "08594875700000105001011040231560200007460101", "08591.01107 40231.560208 00074.601014 4 87570000010500", 2021, 9, 28)]
        [TestCase(19.9, "74621", "02315602/74621", "5", "02315602000074621", "08595875700000019901011040231560200007462101", "08591.01107 40231.560208 00074.621012 5 87570000001990", 2021, 9, 28)]
        [TestCase(110, "74643", "02315602/74643", "6", "02315602000074643", "08596875700000110001011040231560200007464301", "08591.01107 40231.560208 00074.643016 6 87570000011000", 2021, 9, 28)]
        [TestCase(200, "74660", "02315602/74660", "7", "02315602000074660", "08597875700000200001011040231560200007466001", "08591.01107 40231.560208 00074.660010 7 87570000020000", 2021, 9, 28)]
        [TestCase(2100.5, "74669", "02315602/74669", "8", "02315602000074669", "08598875700002100501011040231560200007466901", "08591.01107 40231.560208 00074.669011 8 87570000210050", 2021, 9, 28)]
        [TestCase(89.9, "74603", "02315602/74603", "9", "02315602000074603", "08599875700000089901011040231560200007460301", "08591.01107 40231.560208 00074.603010 9 87570000008990", 2021, 9, 28)]
        public void Deve_criar_boleto_cecred_01_com_linha_digitavel_valida(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            //Ambiente
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

            //Ação
            boleto.ValidarDados();

            Assert.That(boleto.CodigoBarra.LinhaDigitavel, Is.EqualTo(linhaDigitavel), "Linha digitável inválida");
        }

        [Test]
        public void Cecred_1_REM240_Convenio()
        {
            //Ambiente
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoCecredCarteira1Tests), 5, true, "?", 1);

            //Ação
            var nomeArquivoREM = Path.Combine(Path.GetTempPath(), "BoletoNetCore", $"{nameof(BancoCecredCarteira1Tests)}_{TipoArquivo.CNAB240}.REM");
            var linhas = File.ReadAllLines(nomeArquivoREM);
            var convenio = linhas[1].Substring(33, 20).TrimEnd();
            //Assert
            Assert.AreEqual("23156489", convenio);
        }
    }
}