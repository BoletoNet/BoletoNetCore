using System;
using System.ComponentModel;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [System.ComponentModel.Category("Bradesco Carteira 04")]
    public class BancoBradescoCarteira04
    {
        readonly IBanco _banco;
        public BancoBradescoCarteira04()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "X",
                Conta = "123456",
                DigitoConta = "X",
                CarteiraPadrao = "04",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.Bradesco);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("1213141", "", "", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Bradesco_04_REM240()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoBradescoCarteira04), 5, true, "?", 223344);
        }
        [Test]
        public void Bradesco_04_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoBradescoCarteira04), 5, true, "?", 223344);
        }


        [TestCase(141.50, "453", "BB943A", "2", "004/00000000453-1", "23792690400000141501234040000000045301234560", "23791.23405 40000.000048 53012.345608 2 69040000014150", 2016, 9, 1)]
        [TestCase(2717.16, "456", "BB874A", "5", "004/00000000456-6", "23795693400002717161234040000000045601234560","23791.23405 40000.000048 56012.345601 5 69340000271716", 2016, 10, 1)]
        [TestCase(297.21, "444", "BB834A", "6", "004/00000000444-2", "23796690500000297211234040000000044401234560", "23791.23405 40000.000048 44012.345607 6 69050000029721", 2016, 9, 2)]
        [TestCase(297.21, "468", "BB856A", "7", "004/00000000468-P", "23797693500000297211234040000000046801234560", "23791.23405 40000.000048 68012.345606 7 69350000029721", 2016, 10, 2)]
        [TestCase(297.21, "443", "BB833A", "8", "004/00000000443-4", "23798690500000297211234040000000044301234560", "23791.23405 40000.000048 43012.345609 8 69050000029721", 2016, 9, 2)]
        [TestCase(649.39, "414", "BB815A", "9", "004/00000000414-0", "23799687300000649391234040000000041401234560", "23791.23405 40000.000048 14012.345600 9 68730000064939", 2016, 8, 1)]
        [TestCase(270, "561", "BB932A", "1", "004/00000000561-9", "23791702600000270001234040000000056101234560",    "23791.23405 40000.000055 61012.345601 1 70260000027000", 2017, 1, 1)]
        [TestCase(2924.11, "445", "BB874A", "1", "004/00000000445-0", "23791690500002924111234040000000044501234560","23791.23405 40000.000048 45012.345604 1 69050000292411", 2016, 9, 2)]
        [TestCase(830, "562", "BB933A", "1", "004/00000000562-7", "23791702600000830001234040000000056201234560",    "23791.23405 40000.000055 62012.345609 1 70260000083000", 2017, 1, 1)]
        public void Deve_criar_boleto_bradesco_04_com_linha_digitavel_valida(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

            boleto.ValidarDados();

            Assert.That(boleto.CodigoBarra.LinhaDigitavel, Is.EqualTo(linhaDigitavel), "Linha digitável inválida");
        }

        [TestCase(141.50, "453", "BB943A", "2", "004/00000000453-1", "23792690400000141501234040000000045301234560", "23791.23405 40000.000048 53012.345608 2 69040000014150", 2016, 9, 1)]
        [TestCase(2717.16, "456", "BB874A", "5", "004/00000000456-6", "23795693400002717161234040000000045601234560", "23791.23405 40000.000048 56012.345601 5 69340000271716", 2016, 10, 1)]
        [TestCase(297.21, "444", "BB834A", "6", "004/00000000444-2", "23796690500000297211234040000000044401234560", "23791.23405 40000.000048 44012.345607 6 69050000029721", 2016, 9, 2)]
        [TestCase(297.21, "468", "BB856A", "7", "004/00000000468-P", "23797693500000297211234040000000046801234560", "23791.23405 40000.000048 68012.345606 7 69350000029721", 2016, 10, 2)]
        [TestCase(297.21, "443", "BB833A", "8", "004/00000000443-4", "23798690500000297211234040000000044301234560", "23791.23405 40000.000048 43012.345609 8 69050000029721", 2016, 9, 2)]
        [TestCase(649.39, "414", "BB815A", "9", "004/00000000414-0", "23799687300000649391234040000000041401234560", "23791.23405 40000.000048 14012.345600 9 68730000064939", 2016, 8, 1)]
        [TestCase(270, "561", "BB932A", "1", "004/00000000561-9", "23791702600000270001234040000000056101234560", "23791.23405 40000.000055 61012.345601 1 70260000027000", 2017, 1, 1)]
        [TestCase(2924.11, "445", "BB874A", "1", "004/00000000445-0", "23791690500002924111234040000000044501234560", "23791.23405 40000.000048 45012.345604 1 69050000292411", 2016, 9, 2)]
        [TestCase(830, "562", "BB933A", "1", "004/00000000562-7", "23791702600000830001234040000000056201234560", "23791.23405 40000.000055 62012.345609 1 70260000083000", 2017, 1, 1)]
        public void Deve_criar_boleto_bradesco_04_com_digito_verificador_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

            boleto.ValidarDados();

            Assert.That(boleto.CodigoBarra.DigitoVerificador, Is.EqualTo(digitoVerificador), $"Dígito Verificador diferente de {digitoVerificador}");
        }

        [TestCase(141.50, "453", "BB943A", "2", "004/00000000453-1", "23792690400000141501234040000000045301234560", "23791.23405 40000.000048 53012.345608 2 69040000014150", 2016, 9, 1)]
        [TestCase(2717.16, "456", "BB874A", "5", "004/00000000456-6", "23795693400002717161234040000000045601234560", "23791.23405 40000.000048 56012.345601 5 69340000271716", 2016, 10, 1)]
        [TestCase(297.21, "444", "BB834A", "6", "004/00000000444-2", "23796690500000297211234040000000044401234560", "23791.23405 40000.000048 44012.345607 6 69050000029721", 2016, 9, 2)]
        [TestCase(297.21, "468", "BB856A", "7", "004/00000000468-P", "23797693500000297211234040000000046801234560", "23791.23405 40000.000048 68012.345606 7 69350000029721", 2016, 10, 2)]
        [TestCase(297.21, "443", "BB833A", "8", "004/00000000443-4", "23798690500000297211234040000000044301234560", "23791.23405 40000.000048 43012.345609 8 69050000029721", 2016, 9, 2)]
        [TestCase(649.39, "414", "BB815A", "9", "004/00000000414-0", "23799687300000649391234040000000041401234560", "23791.23405 40000.000048 14012.345600 9 68730000064939", 2016, 8, 1)]
        [TestCase(270, "561", "BB932A", "1", "004/00000000561-9", "23791702600000270001234040000000056101234560", "23791.23405 40000.000055 61012.345601 1 70260000027000", 2017, 1, 1)]
        [TestCase(2924.11, "445", "BB874A", "1", "004/00000000445-0", "23791690500002924111234040000000044501234560", "23791.23405 40000.000048 45012.345604 1 69050000292411", 2016, 9, 2)]
        [TestCase(830, "562", "BB933A", "1", "004/00000000562-7", "23791702600000830001234040000000056201234560", "23791.23405 40000.000055 62012.345609 1 70260000083000", 2017, 1, 1)]
        public void Deve_criar_boleto_bradesco_04_com_nosso_numero_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

            boleto.ValidarDados();

            Assert.That(boleto.NossoNumeroFormatado, Is.EqualTo(nossoNumeroFormatado), "Nosso número inválido");
        }


        [TestCase(141.50, "453", "BB943A", "2", "004/00000000453-1", "23792690400000141501234040000000045301234560", "23791.23405 40000.000048 53012.345608 2 69040000014150", 2016, 9, 1)]
        [TestCase(2717.16, "456", "BB874A", "5", "004/00000000456-6", "23795693400002717161234040000000045601234560", "23791.23405 40000.000048 56012.345601 5 69340000271716", 2016, 10, 1)]
        [TestCase(297.21, "444", "BB834A", "6", "004/00000000444-2", "23796690500000297211234040000000044401234560", "23791.23405 40000.000048 44012.345607 6 69050000029721", 2016, 9, 2)]
        [TestCase(297.21, "468", "BB856A", "7", "004/00000000468-P", "23797693500000297211234040000000046801234560", "23791.23405 40000.000048 68012.345606 7 69350000029721", 2016, 10, 2)]
        [TestCase(297.21, "443", "BB833A", "8", "004/00000000443-4", "23798690500000297211234040000000044301234560", "23791.23405 40000.000048 43012.345609 8 69050000029721", 2016, 9, 2)]
        [TestCase(649.39, "414", "BB815A", "9", "004/00000000414-0", "23799687300000649391234040000000041401234560", "23791.23405 40000.000048 14012.345600 9 68730000064939", 2016, 8, 1)]
        [TestCase(270, "561", "BB932A", "1", "004/00000000561-9", "23791702600000270001234040000000056101234560", "23791.23405 40000.000055 61012.345601 1 70260000027000", 2017, 1, 1)]
        [TestCase(2924.11, "445", "BB874A", "1", "004/00000000445-0", "23791690500002924111234040000000044501234560", "23791.23405 40000.000048 45012.345604 1 69050000292411", 2016, 9, 2)]
        [TestCase(830, "562", "BB933A", "1", "004/00000000562-7", "23791702600000830001234040000000056201234560", "23791.23405 40000.000055 62012.345609 1 70260000083000", 2017, 1, 1)]
        public void Deve_criar_boleto_bradesco_04_com_codigo_de_barras_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

            boleto.ValidarDados();

            Assert.That(boleto.CodigoBarra.CodigoDeBarras, Is.EqualTo(codigoDeBarras), "Código de Barra inválido");
        }
    }
}