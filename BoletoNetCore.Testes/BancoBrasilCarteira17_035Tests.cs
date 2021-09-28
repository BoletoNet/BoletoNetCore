using System;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Brasil Carteira 17 Var 035")]
    public class BancoBrasilCarteira17_035Tests
    {
        readonly IBanco _banco;
        public BancoBrasilCarteira17_035Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "X",
                Conta = "123456",
                DigitoConta = "X",
                CarteiraPadrao = "17",
                VariacaoCarteiraPadrao = "035",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.BancoDoBrasil);
            _banco.Beneficiario = Utils.GerarBeneficiario("1234567", "", "", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Brasil_17_035_REM240()
        {
            Utils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoBrasilCarteira17_035Tests), 5, true, "?", 223344);
        }

        [Test]
        public void Brasil_17_035_REM400()
        {
            Utils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoBrasilCarteira17_035Tests), 5, true, "?", 223344);
        }



        [TestCase(400d, "5", "BO123456E", "1", "12345670000000005", "00191699100000400000000001234567000000000517", "00190.00009 01234.567004 00000.005173 1 69910000040000", 2016, 11, 27)]
        [TestCase(402d, "5", "BO123456E", "2", "12345670000000005", "00192699100000402000000001234567000000000517", "00190.00009 01234.567004 00000.005173 2 69910000040200", 2016, 11, 27)]
        [TestCase(200d, "3", "BO123456C", "3", "12345670000000003", "00193692900000200000000001234567000000000317", "00190.00009 01234.567004 00000.003178 3 69290000020000", 2016, 9, 26)]
        [TestCase(1232.78, "1", "BO123456A", "4", "12345670000000001", "00194688900001232780000001234567000000000117", "00190.00009 01234.567004 00000.001172 4 68890000123278", 2016, 8, 17)]
        [TestCase(800, "9", "BO123456I", "5", "12345670000000009", "00195710300000800000000001234567000000000917", "00190.00009 01234.567004 00000.009175 5 71030000080000", 2017, 3, 19)]
        [TestCase(306.52, "4", "BO123456D", "6", "12345670000000004", "00196695900000306520000001234567000000000417", "00190.00009 01234.567004 00000.004176 6 69590000030652", 2016, 10, 26)]
        [TestCase(300, "4", "BO123456D", "7", "12345670000000004", "00197695900000300000000001234567000000000417", "00190.00009 01234.567004 00000.004176 7 69590000030000", 2016, 10, 26)]
        [TestCase(609, "7", "BO123456G", "8", "12345670000000007", "00198705200000609000000001234567000000000717", "00190.00009 01234.567004 00000.007179 8 70520000060900", 2017, 1, 27)]
        [TestCase(600, "7", "BO123456G", "9", "12345670000000007", "00199705200000600000000001234567000000000717", "00190.00009 01234.567004 00000.007179 9 70520000060000", 2017, 1, 27)]
        public void Brasil_17_035_BoletoOK(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            //Ambiente
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = Utils.GerarPagador()
            };

            //Ação
            boleto.ValidarDados();

            //Assertivas
            Assert.That(boleto.CodigoBarra.DigitoVerificador, Is.EqualTo(digitoVerificador), $"Dígito Verificador diferente de {digitoVerificador}");
            Assert.That(boleto.NossoNumeroFormatado, Is.EqualTo(nossoNumeroFormatado), "Nosso número inválido");
            Assert.That(boleto.CodigoBarra.CodigoDeBarras, Is.EqualTo(codigoDeBarras), "Código de Barra inválido");
            Assert.That(boleto.CodigoBarra.LinhaDigitavel, Is.EqualTo(linhaDigitavel), "Linha digitável inválida");
        }
    }
}