using System;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Santander Carteira 101")]
    public class BancoSantanderCarteira101Tests
    {
        readonly IBanco _banco;
        public BancoSantanderCarteira101Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "5",
                Conta = "12345678",
                DigitoConta = "9",
                CarteiraPadrao = "101",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.Santander);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("1234567", "", "123400001234567", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Santander_101_REM240()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoSantanderCarteira101Tests), 5, true, "N", 223344);
        }

        [TestCase(2717.16, "456", "BB874A", "1", "000000000456-1", "03391693400002717169123456700000000045610101", "03399.12347 56700.000005 00456.101013 1 69340000271716", 2016, 10, 1)]
        [TestCase(649.32, "414", "BB815A", "2", "000000000414-6", "03392687300000649329123456700000000041460101", "03399.12347 56700.000005 00414.601013 2 68730000064932", 2016, 8, 1)]
        [TestCase(297.22, "444", "BB834A", "3", "000000000444-8", "03393690500000297229123456700000000044480101", "03399.12347 56700.000005 00444.801013 3 69050000029722", 2016, 9, 2)]
        [TestCase(297.46, "13724", "BB834A", "4", "000000013724-3", "03394690500000297469123456700000001372430101", "03399.12347 56700.000005 13724.301018 4 69050000029746", 2016, 9, 2)]
        [TestCase(297.34, "12428", "BB834A", "5", "000000012428-1", "03395690500000297349123456700000001242810101", "03399.12347 56700.000005 12428.101013 5 69050000029734", 2016, 9, 2)]
        [TestCase(297.21, "443", "BB833A", "6", "000000000443-0", "03396690500000297219123456700000000044300101", "03399.12347 56700.000005 00443.001011 6 69050000029721", 2016, 9, 2)]
        [TestCase(2924.11, "445", "BB874A", "7", "000000000445-6", "03397690500002924119123456700000000044560101", "03399.12347 56700.000005 00445.601016 7 69050000292411", 2016, 9, 2)]
        [TestCase(141.50, "453", "BB943A", "8", "000000000453-7", "03398690400000141509123456700000000045370101", "03399.12347 56700.000005 00453.701013 8 69040000014150", 2016, 9, 1)]
        [TestCase(297.45, "16278", "BB834A", "9", "000000016278-7", "03399690500000297459123456700000001627870101", "03399.12347 56700.000005 16278.701012 9 69050000029745", 2016, 9, 2)]
        public void Deve_criar_boleto_santander_101_com_digito_verificador_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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

        [TestCase(2717.16, "456", "BB874A", "1", "000000000456-1", "03391693400002717169123456700000000045610101", "03399.12347 56700.000005 00456.101013 1 69340000271716", 2016, 10, 1)]
        [TestCase(649.32, "414", "BB815A", "2", "000000000414-6", "03392687300000649329123456700000000041460101", "03399.12347 56700.000005 00414.601013 2 68730000064932", 2016, 8, 1)]
        [TestCase(297.22, "444", "BB834A", "3", "000000000444-8", "03393690500000297229123456700000000044480101", "03399.12347 56700.000005 00444.801013 3 69050000029722", 2016, 9, 2)]
        [TestCase(297.46, "13724", "BB834A", "4", "000000013724-3", "03394690500000297469123456700000001372430101", "03399.12347 56700.000005 13724.301018 4 69050000029746", 2016, 9, 2)]
        [TestCase(297.34, "12428", "BB834A", "5", "000000012428-1", "03395690500000297349123456700000001242810101", "03399.12347 56700.000005 12428.101013 5 69050000029734", 2016, 9, 2)]
        [TestCase(297.21, "443", "BB833A", "6", "000000000443-0", "03396690500000297219123456700000000044300101", "03399.12347 56700.000005 00443.001011 6 69050000029721", 2016, 9, 2)]
        [TestCase(2924.11, "445", "BB874A", "7", "000000000445-6", "03397690500002924119123456700000000044560101", "03399.12347 56700.000005 00445.601016 7 69050000292411", 2016, 9, 2)]
        [TestCase(141.50, "453", "BB943A", "8", "000000000453-7", "03398690400000141509123456700000000045370101", "03399.12347 56700.000005 00453.701013 8 69040000014150", 2016, 9, 1)]
        [TestCase(297.45, "16278", "BB834A", "9", "000000016278-7", "03399690500000297459123456700000001627870101", "03399.12347 56700.000005 16278.701012 9 69050000029745", 2016, 9, 2)]
        public void Deve_criar_boleto_santander_101_com_nosso_numero_formatado_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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

        [TestCase(2717.16, "456", "BB874A", "1", "000000000456-1", "03391693400002717169123456700000000045610101", "03399.12347 56700.000005 00456.101013 1 69340000271716", 2016, 10, 1)]
        [TestCase(649.32, "414", "BB815A", "2", "000000000414-6", "03392687300000649329123456700000000041460101", "03399.12347 56700.000005 00414.601013 2 68730000064932", 2016, 8, 1)]
        [TestCase(297.22, "444", "BB834A", "3", "000000000444-8", "03393690500000297229123456700000000044480101", "03399.12347 56700.000005 00444.801013 3 69050000029722", 2016, 9, 2)]
        [TestCase(297.46, "13724", "BB834A", "4", "000000013724-3", "03394690500000297469123456700000001372430101", "03399.12347 56700.000005 13724.301018 4 69050000029746", 2016, 9, 2)]
        [TestCase(297.34, "12428", "BB834A", "5", "000000012428-1", "03395690500000297349123456700000001242810101", "03399.12347 56700.000005 12428.101013 5 69050000029734", 2016, 9, 2)]
        [TestCase(297.21, "443", "BB833A", "6", "000000000443-0", "03396690500000297219123456700000000044300101", "03399.12347 56700.000005 00443.001011 6 69050000029721", 2016, 9, 2)]
        [TestCase(2924.11, "445", "BB874A", "7", "000000000445-6", "03397690500002924119123456700000000044560101", "03399.12347 56700.000005 00445.601016 7 69050000292411", 2016, 9, 2)]
        [TestCase(141.50, "453", "BB943A", "8", "000000000453-7", "03398690400000141509123456700000000045370101", "03399.12347 56700.000005 00453.701013 8 69040000014150", 2016, 9, 1)]
        [TestCase(297.45, "16278", "BB834A", "9", "000000016278-7", "03399690500000297459123456700000001627870101", "03399.12347 56700.000005 16278.701012 9 69050000029745", 2016, 9, 2)]
        public void Deve_criar_boleto_santander_101_com_codigo_de_barras_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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

        [TestCase(2717.16, "456", "BB874A", "1", "000000000456-1", "03391693400002717169123456700000000045610101", "03399.12347 56700.000005 00456.101013 1 69340000271716", 2016, 10, 1)]
        [TestCase(649.32, "414", "BB815A", "2", "000000000414-6", "03392687300000649329123456700000000041460101", "03399.12347 56700.000005 00414.601013 2 68730000064932", 2016, 8, 1)]
        [TestCase(297.22, "444", "BB834A", "3", "000000000444-8", "03393690500000297229123456700000000044480101", "03399.12347 56700.000005 00444.801013 3 69050000029722", 2016, 9, 2)]
        [TestCase(297.46, "13724", "BB834A", "4", "000000013724-3", "03394690500000297469123456700000001372430101", "03399.12347 56700.000005 13724.301018 4 69050000029746", 2016, 9, 2)]
        [TestCase(297.34, "12428", "BB834A", "5", "000000012428-1", "03395690500000297349123456700000001242810101", "03399.12347 56700.000005 12428.101013 5 69050000029734", 2016, 9, 2)]
        [TestCase(297.21, "443", "BB833A", "6", "000000000443-0", "03396690500000297219123456700000000044300101", "03399.12347 56700.000005 00443.001011 6 69050000029721", 2016, 9, 2)]
        [TestCase(2924.11, "445", "BB874A", "7", "000000000445-6", "03397690500002924119123456700000000044560101", "03399.12347 56700.000005 00445.601016 7 69050000292411", 2016, 9, 2)]
        [TestCase(141.50, "453", "BB943A", "8", "000000000453-7", "03398690400000141509123456700000000045370101", "03399.12347 56700.000005 00453.701013 8 69040000014150", 2016, 9, 1)]
        [TestCase(297.45, "16278", "BB834A", "9", "000000016278-7", "03399690500000297459123456700000001627870101", "03399.12347 56700.000005 16278.701012 9 69050000029745", 2016, 9, 2)]
        public void Deve_criar_boleto_santander_101_com_linha_digitavel_valida(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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