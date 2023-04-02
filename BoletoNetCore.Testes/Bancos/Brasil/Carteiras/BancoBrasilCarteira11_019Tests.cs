using NUnit.Framework;
using System;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Brasil Carteira 11 Var 019")]
    public class BancoBrasilCarteira11019Tests
    {
        readonly IBanco _banco;
        public BancoBrasilCarteira11019Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "X",
                Conta = "12345",
                DigitoConta = "X",
                CarteiraPadrao = "11",
                VariacaoCarteiraPadrao = "019",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Banco
            };
            _banco = Banco.Instancia(Bancos.BancoDoBrasil);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("1234567", "", "", contaBancaria);
            _banco.FormataBeneficiario();


        }

        [Test]
        public void Brasil_11_019_REM240()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoBrasilCarteira11019Tests), 5, true, "?", 0);
        }

        [Test]
        public void Brasil_11_019_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoBrasilCarteira11019Tests), 5, true, "?", 0);
        }

        [TestCase(800d, "12345670000000001", "BAN789A123", "1", "12345670000000001", "00191781400000800000000001234567000000000111", "00190.00009 01234.567004 00000.001115 1 78140000080000", 2019, 2, 28)]

        // teste falhando devido ao vencimento informado
        //[TestCase(500, "12345670000000003", "qwerqqwer", "2", "12345670000000003", "00192534400000500000000001234567000000000311", "00190.00009 01234.567004 00000.003111 2 53440000050000", 2012, 5, 25)]

        [TestCase(400, "00000000000000000", "123456789", "3", "00000000000000000", "00193739500000400000000000000000000000000011", "00190.00009 00000.000000 00000.000117 3 73950000040000", 2018, 1, 05)]
        [TestCase(804, "12345670000000321", "654321RT", "4", "12345670000000321", "00194739200000804000000001234567000000032111", "00190.00009 01234.567004 00000.321117 4 73920000080400", 2018, 1, 2)]
        [TestCase(220.58, "12345670000000003", "654321WA", "5", "12345670000000003", "00195716900000220580000001234567000000000311", "00190.00009 01234.567004 00000.003111 5 71690000022058", 2017, 05, 24)]
        [TestCase(200, "", "123456/2018-A", "6", "00000000000000000", "00196738400000200000000000000000000000000011", "00190.00009 00000.000000 00000.000117 6 73840000020000", 2017, 12, 25)]
        [TestCase(1200, "", "321as1234", "7", "00000000000000000", "00197739100001200000000000000000000000000011", "00190.00009 00000.000000 00000.000117 7 73910000120000", 2018, 1, 1)]
        [TestCase(791, "12345679999999901", "654321VW", "8", "12345679999999901", "00198782400000791000000001234567999999990111", "00190.00009 01234.567996 99999.901111 8 78240000079100", 2019, 3, 10)]
        [TestCase(350, "12345670047850002", "654321LA", "9", "12345670047850002", "00199665300000350000000001234567004785000211", "00190.00009 01234.567004 47850.002115 9 66530000035000", 2015, 12, 25)]
        public void Brasil_11_019_BoletoOK(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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
            Assert.That(boleto.NossoNumeroFormatado, Is.EqualTo(nossoNumeroFormatado), "Nosso número inválido");
            Assert.That(boleto.CodigoBarra.CodigoDeBarras, Is.EqualTo(codigoDeBarras), "Código de Barra inválido");
            Assert.That(boleto.CodigoBarra.LinhaDigitavel, Is.EqualTo(linhaDigitavel), "Linha digitável inválida");
        }
    }
}