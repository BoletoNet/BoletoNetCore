using System;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Uniprime Norte PR Carteira 09")]
    public class BancoUniprimeNortePRCarteira09Tests
    {
        readonly IBanco _banco;
        public BancoUniprimeNortePRCarteira09Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "0012",
                DigitoAgencia = "4",
                Conta = "47744",
                DigitoConta = "3",
                CarteiraPadrao = "09",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.UniprimeNortePR);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("", "", "", contaBancaria);
            _banco.FormataBeneficiario();
        }

        /*
        [Test]
        public void Bradesco_09_REM240()
        {
            Utils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoBradescoCarteira09), 5, true, "?", 223344);
        }
        [Test]
        public void Bradesco_09_REM400()
        {
            Utils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoBradescoCarteira09), 5, true, "?", 223344);
        }
        */


        [TestCase(712.4, "10035", "923/10", "7", "09/00000010035-0", "08497948400000712400012090000001003500477440", "08490.01201 90000.001009 35004.774408 7 94840000071240", 2023, 9, 25)]
        [TestCase(180.83, "10012", "770/5", "6", "09/00000010012-1", "08496903200000180830012090000001001200477440", "08490.01201 90000.001009 12004.774407 6 90320000018083", 2022, 06, 30)]
        [TestCase(274.15, "10020", "857/7", "9", "09/00000010020-2", "08499929500000274150012090000001002000477440", "08490.01201 90000.001009 20004.774400 9 92950000027415", 2023, 3, 20)]        
        [TestCase(180.83, "10008", "770/1", "8", "09/00000010008-3", "08498891000000180830012090000001000800477440", "08490.01201 90000.001009 08004.774405 8 89100000018083", 2022, 2, 28)]        
        [TestCase(718.33, "10002", "692/3", "5", "09/00000010002-4", "08495889400000718330012090000001000200477440", "08490.01201 90000.001009 02004.774408 5 88940000071833", 2022, 2, 12)]
        [TestCase(180.83, "10010", "770/3", "2", "09/00000010010-5", "08492897100000180830012090000001001000477440", "08490.01201 90000.001009 10004.774401 2 89710000018083", 2022, 4, 30)]
        [TestCase(274.13, "10015", "857/2", "6", "09/00000010015-6", "08496914400000274130012090000001001500477440", "08490.01201 90000.001009 15004.774400 6 91440000027413", 2022, 10, 20)]
        [TestCase(3153.20, "10023", "853/4", "1", "09/00000010023-7", "08491921200003153200012090000001002300477440", "08490.01201 90000.001009 23004.774404 1 92120000315320", 2022, 12, 27)]
        [TestCase(880.00, "10014", "790/1", "1", "09/00000010014-8", "08491900200000880000012090000001001400477440", "08490.01201 90000.001009 14004.774403 1 90020000088000", 2022, 05, 31)]
        [TestCase(500, "10005", "694/2", "1", "09/00000010005-9", "08491886800000500000012090000001000500477440", "08490.01201 90000.001009 05004.774401 1 88680000050000", 2022, 1, 17)]
        [TestCase(274.13, "10019", "857/6", "8", "09/00000010019-9", "08498926700000274130012090000001001900477440", "08490.01201 90000.001009 19004.774402 8 92670000027413", 2023, 2, 20)]
        [TestCase(712.40, "10027", "923/2", "4", "09/00000010027-P", "08494924100000712400012090000001002700477440", "08490.01201 90000.001009 27004.774405 4 92410000071240", 2023, 01, 25)]
        public void UniprimeNortePR_09_BoletoOK(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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
            Assert.That(boleto.NumeroDocumento, Is.EqualTo(numeroDocumento), "Número do Documento");
            Assert.That(boleto.ValorTitulo, Is.EqualTo(valorTitulo), "Valor do Título");
        }
    }
}