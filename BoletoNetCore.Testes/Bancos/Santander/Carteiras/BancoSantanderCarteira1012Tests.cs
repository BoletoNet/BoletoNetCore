using System;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Santander Carteira 101/2")]
    public class BancoSantanderCarteira1012Tests
    {
        readonly IBanco _banco;
        public BancoSantanderCarteira1012Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "5",
                Conta = "12345678",
                DigitoConta = "9",
                CarteiraPadrao = "101",
                VariacaoCarteiraPadrao = "2",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.Santander);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("1234567", "", "123400001234567", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Santander_1012_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoSantanderCarteira1012Tests), 5, true, "N", 223344);
        }

        [TestCase(2717.16, "456", "BB874A", "9", "0000000000456-1", "03391693400002717169123456700000000004560101", "03399.12347 56700.000005 00045.601010 9 69340000271716", 2016, 10, 1)]
        [TestCase(649.32, "414", "BB815A", "3", "0000000000414-6", "03392687300000649329123456700000000004140101", "03399.12347 56700.000005 00041.401019 3 68730000064932", 2016, 8, 1)]
        [TestCase(297.22, "444", "BB834A", "8", "0000000000444-8", "03393690500000297229123456700000000004440101", "03399.12347 56700.000005 00044.401016 8 69050000029722", 2016, 9, 2)]
        [TestCase(297.46, "13724", "BB834A", "4", "0000000013724-3", "03394690500000297469123456700000000137240101", "03399.12347 56700.000005 01372.401016 4 69050000029746", 2016, 9, 2)]
        [TestCase(297.34, "12428", "BB834A", "1", "0000000012428-1", "03395690500000297349123456700000000124280101", "03399.12347 56700.000005 01242.801015 1 69050000029734", 2016, 9, 2)]
        [TestCase(297.21, "443", "BB833A", "6", "0000000000443-0", "03396690500000297219123456700000000004430101", "03399.12347 56700.000005 00044.301018 6 69050000029721", 2016, 9, 2)]
        [TestCase(2924.11, "445", "BB874A", "1", "0000000000445-6", "03397690500002924119123456700000000004450101", "03399.12347 56700.000005 00044.501013 1 69050000292411", 2016, 9, 2)]
        [TestCase(141.50, "453", "BB943A", "7", "0000000000453-7", "03398690400000141509123456700000000004530101", "03399.12347 56700.000005 00045.301017 7 69040000014150", 2016, 9, 1)]
        [TestCase(297.45, "16278", "BB834A", "5", "0000000016278-7", "03399690500000297459123456700000000162780101", "03399.12347 56700.000005 01627.801010 5 69050000029745", 2016, 9, 2)]
        public void Deve_criar_boleto_santander_1012_com_digito_verificador_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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

        [TestCase(2717.16, "456", "BB874A", "9", "0000000000456-1", "03391693400002717169123456700000000004560101", "03399.12347 56700.000005 00045.601010 9 69340000271716", 2016, 10, 1)]
        [TestCase(649.32, "414", "BB815A", "3", "0000000000414-6", "03392687300000649329123456700000000004140101", "03399.12347 56700.000005 00041.401019 3 68730000064932", 2016, 8, 1)]
        [TestCase(297.22, "444", "BB834A", "8", "0000000000444-8", "03393690500000297229123456700000000004440101", "03399.12347 56700.000005 00044.401016 8 69050000029722", 2016, 9, 2)]
        [TestCase(297.46, "13724", "BB834A", "4", "0000000013724-3", "03394690500000297469123456700000000137240101", "03399.12347 56700.000005 01372.401016 4 69050000029746", 2016, 9, 2)]
        [TestCase(297.34, "12428", "BB834A", "1", "0000000012428-1", "03395690500000297349123456700000000124280101", "03399.12347 56700.000005 01242.801015 1 69050000029734", 2016, 9, 2)]
        [TestCase(297.21, "443", "BB833A", "6", "0000000000443-0", "03396690500000297219123456700000000004430101", "03399.12347 56700.000005 00044.301018 6 69050000029721", 2016, 9, 2)]
        [TestCase(2924.11, "445", "BB874A", "1", "0000000000445-6", "03397690500002924119123456700000000004450101", "03399.12347 56700.000005 00044.501013 1 69050000292411", 2016, 9, 2)]
        [TestCase(141.50, "453", "BB943A", "7", "0000000000453-7", "03398690400000141509123456700000000004530101", "03399.12347 56700.000005 00045.301017 7 69040000014150", 2016, 9, 1)]
        [TestCase(297.45, "16278", "BB834A", "5", "0000000016278-7", "03399690500000297459123456700000000162780101", "03399.12347 56700.000005 01627.801010 5 69050000029745", 2016, 9, 2)]
        public void Deve_criar_boleto_santander_1012_com_nosso_numero_formatado_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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

        [TestCase(2717.16, "456", "BB874A", "9", "0000000000456-1", "03399693400002717169123456700000000004560101", "03399.12347 56700.000005 00045.601010 9 69340000271716", 2016, 10, 1)]
        [TestCase(649.32, "414", "BB815A", "3", "0000000000414-6", "03393687300000649329123456700000000004140101", "03399.12347 56700.000005 00041.401019 3 68730000064932", 2016, 8, 1)]
        [TestCase(297.22, "444", "BB834A", "8", "0000000000444-8", "03398690500000297229123456700000000004440101", "03399.12347 56700.000005 00044.401016 8 69050000029722", 2016, 9, 2)]
        [TestCase(297.46, "13724", "BB834A", "4", "0000000013724-3", "03394690500000297469123456700000000137240101", "03399.12347 56700.000005 01372.401016 4 69050000029746", 2016, 9, 2)]
        [TestCase(297.34, "12428", "BB834A", "1", "0000000012428-1", "03391690500000297349123456700000000124280101", "03399.12347 56700.000005 01242.801015 1 69050000029734", 2016, 9, 2)]
        [TestCase(297.21, "443", "BB833A", "6", "0000000000443-0", "03396690500000297219123456700000000004430101", "03399.12347 56700.000005 00044.301018 6 69050000029721", 2016, 9, 2)]
        [TestCase(2924.11, "445", "BB874A", "1", "0000000000445-6", "03391690500002924119123456700000000004450101", "03399.12347 56700.000005 00044.501013 1 69050000292411", 2016, 9, 2)]
        [TestCase(141.50, "453", "BB943A", "7", "0000000000453-7", "03397690400000141509123456700000000004530101", "03399.12347 56700.000005 00045.301017 7 69040000014150", 2016, 9, 1)]
        [TestCase(297.45, "16278", "BB834A", "5", "0000000016278-7", "03395690500000297459123456700000000162780101", "03399.12347 56700.000005 01627.801010 5 69050000029745", 2016, 9, 2)]
        public void Deve_criar_boleto_santander_1012_com_codigo_de_barras_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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

        [TestCase(2717.16, "456", "BB874A", "9", "0000000000456-1", "03391693400002717169123456700000000004560101", "03399.12347 56700.000005 00045.601010 9 69340000271716", 2016, 10, 1)]
        [TestCase(649.32, "414", "BB815A", "3", "0000000000414-6", "03392687300000649329123456700000000004140101", "03399.12347 56700.000005 00041.401019 3 68730000064932", 2016, 8, 1)]
        [TestCase(297.22, "444", "BB834A", "8", "0000000000444-8", "03393690500000297229123456700000000004440101", "03399.12347 56700.000005 00044.401016 8 69050000029722", 2016, 9, 2)]
        [TestCase(297.46, "13724", "BB834A", "4", "0000000013724-3", "03394690500000297469123456700000000137240101", "03399.12347 56700.000005 01372.401016 4 69050000029746", 2016, 9, 2)]
        [TestCase(297.34, "12428", "BB834A", "1", "0000000012428-1", "03395690500000297349123456700000000124280101", "03399.12347 56700.000005 01242.801015 1 69050000029734", 2016, 9, 2)]
        [TestCase(297.21, "443", "BB833A", "6", "0000000000443-0", "03396690500000297219123456700000000004430101", "03399.12347 56700.000005 00044.301018 6 69050000029721", 2016, 9, 2)]
        [TestCase(2924.11, "445", "BB874A", "1", "0000000000445-6", "03397690500002924119123456700000000004450101", "03399.12347 56700.000005 00044.501013 1 69050000292411", 2016, 9, 2)]
        [TestCase(141.50, "453", "BB943A", "7", "0000000000453-7", "03398690400000141509123456700000000004530101", "03399.12347 56700.000005 00045.301017 7 69040000014150", 2016, 9, 1)]
        [TestCase(297.45, "16278", "BB834A", "5", "0000000016278-7", "03399690500000297459123456700000000162780101", "03399.12347 56700.000005 01627.801010 5 69050000029745", 2016, 9, 2)]
        public void Deve_criar_boleto_santander_1012_com_linha_digitavel_valida(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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