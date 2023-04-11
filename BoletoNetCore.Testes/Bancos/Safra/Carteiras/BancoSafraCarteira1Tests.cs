using System;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Safra Carteira 1")]
    public class BancoSafraCarteira1
    {
        readonly IBanco _banco;
        public BancoSafraCarteira1()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "5",
                Conta = "123456",
                DigitoConta = "7",
                CarteiraPadrao = "1",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.Safra);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("", "", "", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Safra_1_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoSafraCarteira1), 5, true, "?", 223344);
        }


        [TestCase(141.50, "453", "BB943A", "1", "00000453-7", "42291690400000141507123450012345670000045372", "42297.12346 50012.345679 00000.453720 1 69040000014150", 2016, 9, 1)]
        [TestCase(2711.12, "456", "BB874A", "2", "00000456-1", "42292693400002711127123450012345670000045612", "42297.12346 50012.345679 00000.456129 2 69340000271112", 2016, 10, 1)]
        [TestCase(645.39, "414", "BB815A", "3", "00000414-6", "42293687300000645397123450012345670000041462", "42297.12346 50012.345679 00000.414623 3 68730000064539", 2016, 8, 1)]
        [TestCase(292.21, "444", "BB834A", "4", "00000444-8", "42294690500000292217123450012345670000044482", "42297.12346 50012.345679 00000.444828 4 69050000029221", 2016, 9, 2)]
        [TestCase(838, "562", "BB933A", "5", "00000562-2", "42295702600000838007123450012345670000056222", "42297.12346 50012.345679 00000.562223 5 70260000083800", 2017, 1, 1)]
        [TestCase(2927.11, "445", "BB874A", "6", "00000445-6", "42296690500002927117123450012345670000044562", "42297.12346 50012.345679 00000.445627 6 69050000292711", 2016, 9, 2)]
        [TestCase(2921.27, "443", "BB833A", "7", "00000443-0", "42297690500002921277123450012345670000044302", "42297.12346 50012.345679 00000.443028 7 69050000292127", 2016, 9, 2)]
        [TestCase(293.21, "468", "BB856A", "8", "00000468-5", "42298693500000293217123450012345670000046852", "42297.12346 50012.345679 00000.468520 8 69350000029321", 2016, 10, 2)]
        [TestCase(276, "561", "BB932A", "9", "00000561-4", "42299702600000276007123450012345670000056142", "42297.12346 50012.345679 00000.561423 9 70260000027600", 2017, 1, 1)]
        public void Deve_criar_boleto_safra_01_com_digito_verificador_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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


        [TestCase(141.50, "453", "BB943A", "1", "00000453-7", "42291690400000141507123450012345670000045372", "42297.12346 50012.345679 00000.453720 1 69040000014150", 2016, 9, 1)]
        [TestCase(2711.12, "456", "BB874A", "2", "00000456-1", "42292693400002711127123450012345670000045612", "42297.12346 50012.345679 00000.456129 2 69340000271112", 2016, 10, 1)]
        [TestCase(645.39, "414", "BB815A", "3", "00000414-6", "42293687300000645397123450012345670000041462", "42297.12346 50012.345679 00000.414623 3 68730000064539", 2016, 8, 1)]
        [TestCase(292.21, "444", "BB834A", "4", "00000444-8", "42294690500000292217123450012345670000044482", "42297.12346 50012.345679 00000.444828 4 69050000029221", 2016, 9, 2)]
        [TestCase(838, "562", "BB933A", "5", "00000562-2", "42295702600000838007123450012345670000056222", "42297.12346 50012.345679 00000.562223 5 70260000083800", 2017, 1, 1)]
        [TestCase(2927.11, "445", "BB874A", "6", "00000445-6", "42296690500002927117123450012345670000044562", "42297.12346 50012.345679 00000.445627 6 69050000292711", 2016, 9, 2)]
        [TestCase(2921.27, "443", "BB833A", "7", "00000443-0", "42297690500002921277123450012345670000044302", "42297.12346 50012.345679 00000.443028 7 69050000292127", 2016, 9, 2)]
        [TestCase(293.21, "468", "BB856A", "8", "00000468-5", "42298693500000293217123450012345670000046852", "42297.12346 50012.345679 00000.468520 8 69350000029321", 2016, 10, 2)]
        [TestCase(276, "561", "BB932A", "9", "00000561-4", "42299702600000276007123450012345670000056142", "42297.12346 50012.345679 00000.561423 9 70260000027600", 2017, 1, 1)]
        public void Deve_criar_boleto_safra_01_com_nosso_numero_formatado_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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


        [TestCase(141.50, "453", "BB943A", "1", "00000453-7", "42291690400000141507123450012345670000045372", "42297.12346 50012.345679 00000.453720 1 69040000014150", 2016, 9, 1)]
        [TestCase(2711.12, "456", "BB874A", "2", "00000456-1", "42292693400002711127123450012345670000045612", "42297.12346 50012.345679 00000.456129 2 69340000271112", 2016, 10, 1)]
        [TestCase(645.39, "414", "BB815A", "3", "00000414-6", "42293687300000645397123450012345670000041462", "42297.12346 50012.345679 00000.414623 3 68730000064539", 2016, 8, 1)]
        [TestCase(292.21, "444", "BB834A", "4", "00000444-8", "42294690500000292217123450012345670000044482", "42297.12346 50012.345679 00000.444828 4 69050000029221", 2016, 9, 2)]
        [TestCase(838, "562", "BB933A", "5", "00000562-2", "42295702600000838007123450012345670000056222", "42297.12346 50012.345679 00000.562223 5 70260000083800", 2017, 1, 1)]
        [TestCase(2927.11, "445", "BB874A", "6", "00000445-6", "42296690500002927117123450012345670000044562", "42297.12346 50012.345679 00000.445627 6 69050000292711", 2016, 9, 2)]
        [TestCase(2921.27, "443", "BB833A", "7", "00000443-0", "42297690500002921277123450012345670000044302", "42297.12346 50012.345679 00000.443028 7 69050000292127", 2016, 9, 2)]
        [TestCase(293.21, "468", "BB856A", "8", "00000468-5", "42298693500000293217123450012345670000046852", "42297.12346 50012.345679 00000.468520 8 69350000029321", 2016, 10, 2)]
        [TestCase(276, "561", "BB932A", "9", "00000561-4", "42299702600000276007123450012345670000056142", "42297.12346 50012.345679 00000.561423 9 70260000027600", 2017, 1, 1)]
        public void Deve_criar_boleto_safra_01_com_codigo_de_barras_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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


        [TestCase(141.50, "453", "BB943A", "1", "00000453-7", "42291690400000141507123450012345670000045372", "42297.12346 50012.345679 00000.453720 1 69040000014150", 2016, 9, 1)]
        [TestCase(2711.12, "456", "BB874A", "2", "00000456-1", "42292693400002711127123450012345670000045612", "42297.12346 50012.345679 00000.456129 2 69340000271112", 2016, 10, 1)]
        [TestCase(645.39, "414", "BB815A", "3", "00000414-6", "42293687300000645397123450012345670000041462", "42297.12346 50012.345679 00000.414623 3 68730000064539", 2016, 8, 1)]
        [TestCase(292.21, "444", "BB834A", "4", "00000444-8", "42294690500000292217123450012345670000044482", "42297.12346 50012.345679 00000.444828 4 69050000029221", 2016, 9, 2)]
        [TestCase(838, "562", "BB933A", "5", "00000562-2", "42295702600000838007123450012345670000056222", "42297.12346 50012.345679 00000.562223 5 70260000083800", 2017, 1, 1)]
        [TestCase(2927.11, "445", "BB874A", "6", "00000445-6", "42296690500002927117123450012345670000044562", "42297.12346 50012.345679 00000.445627 6 69050000292711", 2016, 9, 2)]
        [TestCase(2921.27, "443", "BB833A", "7", "00000443-0", "42297690500002921277123450012345670000044302", "42297.12346 50012.345679 00000.443028 7 69050000292127", 2016, 9, 2)]
        [TestCase(293.21, "468", "BB856A", "8", "00000468-5", "42298693500000293217123450012345670000046852", "42297.12346 50012.345679 00000.468520 8 69350000029321", 2016, 10, 2)]
        [TestCase(276, "561", "BB932A", "9", "00000561-4", "42299702600000276007123450012345670000056142", "42297.12346 50012.345679 00000.561423 9 70260000027600", 2017, 1, 1)]
        public void Deve_criar_boleto_safra_01_com_linha_digitavel_valida(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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