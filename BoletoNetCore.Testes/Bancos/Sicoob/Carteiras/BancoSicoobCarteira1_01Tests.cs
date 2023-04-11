using System;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Sicoob Carteira 1 Var 01")]
    public class BancoSicoobCarteira101Tests
    {
        readonly IBanco _banco;
        public BancoSicoobCarteira101Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "4277",
                DigitoAgencia = "3",
                Conta = "6498",
                DigitoConta = "0",
                CarteiraPadrao = "1",
                VariacaoCarteiraPadrao = "01",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.Sicoob);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("17227", "8", "", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Sicoob_1_01_REM240()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoSicoobCarteira101Tests), 5, true, "?", 223344);
        }

        [Test]
        public void Sicoob_1_01_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoSicoobCarteira101Tests), 5, true, "?", 223344);
        }


        [TestCase(300, "4", "BO123456D", "1", "0000004-0", "75691697400000300001427701017227800000040001", "75691.42776 01017.227800 00000.400010 1 69740000030000", 2016, 11, 10)]
        [TestCase(300, "3", "BO123456D", "2", "0000003-3", "75692711100000300001427701017227800000033001", "75691.42776 01017.227800 00000.330019 2 71110000030000", 2017, 03, 27)]
        [TestCase(400, "4", "BO123456D", "3", "0000004-0", "75693714200000400001427701017227800000040001", "75691.42776 01017.227800 00000.400010 3 71420000040000", 2017, 04, 27)]
        [TestCase(500, "6", "BO123456F", "4", "0000006-5", "75694703000000500001427701017227800000065001", "75691.42776 01017.227800 00000.650010 4 70300000050000", 2017, 01, 05)]
        [TestCase(900, "9", "BO123456F", "5", "0000009-7", "75695729500000900001427701017227800000097001", "75691.42776 01017.227800 00000.970012 5 72950000090000", 2017, 09, 27)]
        [TestCase(600, "7", "BO123456G", "6", "0000007-2", "75696706300000600001427701017227800000072001", "75691.42776 01017.227800 00000.720011 6 70630000060000", 2017, 2, 07)]
        [TestCase(4011.24, "12349", "BO123456F", "7", "0012349-2", "75697702200004011241427701017227800123492001", "75691.42776 01017.227800 01234.920013 7 70220000401124", 2016, 12, 28)]
        [TestCase(700, "8", "BO123456B", "8", "0000008-0", "75698709500000700001427701017227800000080001", "75691.42776 01017.227800 00000.800011 8 70950000070000", 2017, 3, 11)]
        [TestCase(409, "5", "BO123456E", "9", "0000005-8", "75699700200000409001427701017227800000058001", "75691.42776 01017.227800 00000.580019 9 70020000040900", 2016, 12, 08)]
        public void Deve_criar_boleto_sicoob_1_01_com_digito_verificador_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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

        [TestCase(300, "4", "BO123456D", "1", "0000004-0", "75691697400000300001427701017227800000040001", "75691.42776 01017.227800 00000.400010 1 69740000030000", 2016, 11, 10)]
        [TestCase(300, "3", "BO123456D", "2", "0000003-3", "75692711100000300001427701017227800000033001", "75691.42776 01017.227800 00000.330019 2 71110000030000", 2017, 03, 27)]
        [TestCase(400, "4", "BO123456D", "3", "0000004-0", "75693714200000400001427701017227800000040001", "75691.42776 01017.227800 00000.400010 3 71420000040000", 2017, 04, 27)]
        [TestCase(500, "6", "BO123456F", "4", "0000006-5", "75694703000000500001427701017227800000065001", "75691.42776 01017.227800 00000.650010 4 70300000050000", 2017, 01, 05)]
        [TestCase(900, "9", "BO123456F", "5", "0000009-7", "75695729500000900001427701017227800000097001", "75691.42776 01017.227800 00000.970012 5 72950000090000", 2017, 09, 27)]
        [TestCase(600, "7", "BO123456G", "6", "0000007-2", "75696706300000600001427701017227800000072001", "75691.42776 01017.227800 00000.720011 6 70630000060000", 2017, 2, 07)]
        [TestCase(4011.24, "12349", "BO123456F", "7", "0012349-2", "75697702200004011241427701017227800123492001", "75691.42776 01017.227800 01234.920013 7 70220000401124", 2016, 12, 28)]
        [TestCase(700, "8", "BO123456B", "8", "0000008-0", "75698709500000700001427701017227800000080001", "75691.42776 01017.227800 00000.800011 8 70950000070000", 2017, 3, 11)]
        [TestCase(409, "5", "BO123456E", "9", "0000005-8", "75699700200000409001427701017227800000058001", "75691.42776 01017.227800 00000.580019 9 70020000040900", 2016, 12, 08)]
        public void Deve_criar_boleto_sicoob_1_01_com_nosso_numero_formatado_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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
            Assert.That(boleto.NossoNumeroFormatado, Is.EqualTo(nossoNumeroFormatado), "Nosso número inválido");
        }

        [TestCase(300, "4", "BO123456D", "1", "0000004-0", "75691697400000300001427701017227800000040001", "75691.42776 01017.227800 00000.400010 1 69740000030000", 2016, 11, 10)]
        [TestCase(300, "3", "BO123456D", "2", "0000003-3", "75692711100000300001427701017227800000033001", "75691.42776 01017.227800 00000.330019 2 71110000030000", 2017, 03, 27)]
        [TestCase(400, "4", "BO123456D", "3", "0000004-0", "75693714200000400001427701017227800000040001", "75691.42776 01017.227800 00000.400010 3 71420000040000", 2017, 04, 27)]
        [TestCase(500, "6", "BO123456F", "4", "0000006-5", "75694703000000500001427701017227800000065001", "75691.42776 01017.227800 00000.650010 4 70300000050000", 2017, 01, 05)]
        [TestCase(900, "9", "BO123456F", "5", "0000009-7", "75695729500000900001427701017227800000097001", "75691.42776 01017.227800 00000.970012 5 72950000090000", 2017, 09, 27)]
        [TestCase(600, "7", "BO123456G", "6", "0000007-2", "75696706300000600001427701017227800000072001", "75691.42776 01017.227800 00000.720011 6 70630000060000", 2017, 2, 07)]
        [TestCase(4011.24, "12349", "BO123456F", "7", "0012349-2", "75697702200004011241427701017227800123492001", "75691.42776 01017.227800 01234.920013 7 70220000401124", 2016, 12, 28)]
        [TestCase(700, "8", "BO123456B", "8", "0000008-0", "75698709500000700001427701017227800000080001", "75691.42776 01017.227800 00000.800011 8 70950000070000", 2017, 3, 11)]
        [TestCase(409, "5", "BO123456E", "9", "0000005-8", "75699700200000409001427701017227800000058001", "75691.42776 01017.227800 00000.580019 9 70020000040900", 2016, 12, 08)]
        public void Deve_criar_boleto_sicoob_1_01_com_codigo_de_barras_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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

        [TestCase(300, "4", "BO123456D", "1", "0000004-0", "75691697400000300001427701017227800000040001", "75691.42776 01017.227800 00000.400010 1 69740000030000", 2016, 11, 10)]
        [TestCase(300, "3", "BO123456D", "2", "0000003-3", "75692711100000300001427701017227800000033001", "75691.42776 01017.227800 00000.330019 2 71110000030000", 2017, 03, 27)]
        [TestCase(400, "4", "BO123456D", "3", "0000004-0", "75693714200000400001427701017227800000040001", "75691.42776 01017.227800 00000.400010 3 71420000040000", 2017, 04, 27)]
        [TestCase(500, "6", "BO123456F", "4", "0000006-5", "75694703000000500001427701017227800000065001", "75691.42776 01017.227800 00000.650010 4 70300000050000", 2017, 01, 05)]
        [TestCase(900, "9", "BO123456F", "5", "0000009-7", "75695729500000900001427701017227800000097001", "75691.42776 01017.227800 00000.970012 5 72950000090000", 2017, 09, 27)]
        [TestCase(600, "7", "BO123456G", "6", "0000007-2", "75696706300000600001427701017227800000072001", "75691.42776 01017.227800 00000.720011 6 70630000060000", 2017, 2, 07)]
        [TestCase(4011.24, "12349", "BO123456F", "7", "0012349-2", "75697702200004011241427701017227800123492001", "75691.42776 01017.227800 01234.920013 7 70220000401124", 2016, 12, 28)]
        [TestCase(700, "8", "BO123456B", "8", "0000008-0", "75698709500000700001427701017227800000080001", "75691.42776 01017.227800 00000.800011 8 70950000070000", 2017, 3, 11)]
        [TestCase(409, "5", "BO123456E", "9", "0000005-8", "75699700200000409001427701017227800000058001", "75691.42776 01017.227800 00000.580019 9 70020000040900", 2016, 12, 08)]
        public void Deve_criar_boleto_sicoob_1_01_com_linha_digitavel_valida(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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
            Assert.That(boleto.CodigoBarra.LinhaDigitavel, Is.EqualTo(linhaDigitavel), "Linha digitável inválida");
        }
    }
}