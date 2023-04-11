using NUnit.Framework;
using System;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Banrisul Carteira 1")]
    public class BancoBanrisulCarteira1Tests
    {
        readonly IBanco _banco;
        public BancoBanrisulCarteira1Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "0340",
                DigitoAgencia = "",
                Conta = "12345606",
                DigitoConta = "",
                CarteiraPadrao = "1",
                VariacaoCarteiraPadrao = "",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro
            };
            _banco = Banco.Instancia(Bancos.Banrisul);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("0340123456063", "", "", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Banrisul_1_REM400_BancoEmite()
        {
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Banco;
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoBanrisulCarteira1Tests) + "_BancoEmite", 5, true, "?", 0);
        }
        [Test]
        public void Banrisul_1_REM400_EmpresaEmite()
        {
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa;
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoBanrisulCarteira1Tests) + "_EmpresaEmite", 5, true, "?", 12345);
        }

        #region Tipo Impressao Empresa

        [TestCase(276.15, "458", "BB874A", "1", "00000458-02", "04191693400000276152103401234560000004584090", "04192.10349 01234.560009 00045.840907 1 69340000027615", 2016, 10, 1)]
        [TestCase(647.34, "451", "BB815A", "2", "00000451-52", "04192687300000647342103401234560000004514041", "04192.10349 01234.560009 00045.140415 2 68730000064734", 2016, 8, 1)]
        [TestCase(293.23, "452", "BB856A", "3", "00000452-33", "04193693500000293232103401234560000004524020", "04192.10349 01234.560009 00045.240207 3 69350000029323", 2016, 10, 2)]
        [TestCase(217.12, "453", "BB874A", "4", "00000453-14", "04194690500000217122103401234560000004534018", "04192.10349 01234.560009 00045.340189 4 69050000021712", 2016, 9, 2)]
        [TestCase(829.21, "454", "BB933A", "5", "00000454-97", "04195702600000829212103401234560000004544080", "04192.10349 01234.560009 00045.440807 5 70260000082921", 2017, 1, 1)]
        [TestCase(270.54, "459", "BB932A", "6", "00000459-85", "04196702600000270542103401234560000004594088", "04192.10349 01234.560009 00045.940889 6 70260000027054", 2017, 1, 1)]
        [TestCase(287.25, "456", "BB834A", "7", "00000456-40", "04197690500000287252103401234560000004564030", "04192.10349 01234.560009 00045.640307 7 69050000028725", 2016, 9, 2)]
        [TestCase(288.26, "455", "BB833A", "8", "00000455-78", "04198690500000288262103401234560000004554051", "04192.10349 01234.560009 00045.540515 8 69050000028826", 2016, 9, 2)]
        [TestCase(131.57, "457", "BB943A", "9", "00000457-21", "04199690400000131572103401234560000004574028", "04192.10349 01234.560009 00045.740289 9 69040000013157", 2016, 9, 1)]
        public void Deve_criar_boleto_banrisul_01_com_tipo_emissao_empresa_e_nosso_numero_formatado_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            // Ambiente - Emissão pela empresa
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa;
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


        [TestCase(276.15, "458", "BB874A", "1", "00000458-02", "04191693400000276152103401234560000004584090", "04192.10349 01234.560009 00045.840907 1 69340000027615", 2016, 10, 1)]
        [TestCase(647.34, "451", "BB815A", "2", "00000451-52", "04192687300000647342103401234560000004514041", "04192.10349 01234.560009 00045.140415 2 68730000064734", 2016, 8, 1)]
        [TestCase(293.23, "452", "BB856A", "3", "00000452-33", "04193693500000293232103401234560000004524020", "04192.10349 01234.560009 00045.240207 3 69350000029323", 2016, 10, 2)]
        [TestCase(217.12, "453", "BB874A", "4", "00000453-14", "04194690500000217122103401234560000004534018", "04192.10349 01234.560009 00045.340189 4 69050000021712", 2016, 9, 2)]
        [TestCase(829.21, "454", "BB933A", "5", "00000454-97", "04195702600000829212103401234560000004544080", "04192.10349 01234.560009 00045.440807 5 70260000082921", 2017, 1, 1)]
        [TestCase(270.54, "459", "BB932A", "6", "00000459-85", "04196702600000270542103401234560000004594088", "04192.10349 01234.560009 00045.940889 6 70260000027054", 2017, 1, 1)]
        [TestCase(287.25, "456", "BB834A", "7", "00000456-40", "04197690500000287252103401234560000004564030", "04192.10349 01234.560009 00045.640307 7 69050000028725", 2016, 9, 2)]
        [TestCase(288.26, "455", "BB833A", "8", "00000455-78", "04198690500000288262103401234560000004554051", "04192.10349 01234.560009 00045.540515 8 69050000028826", 2016, 9, 2)]
        [TestCase(131.57, "457", "BB943A", "9", "00000457-21", "04199690400000131572103401234560000004574028", "04192.10349 01234.560009 00045.740289 9 69040000013157", 2016, 9, 1)]
        public void Deve_criar_boleto_banrisul_01_com_tipo_emissao_empresa_e_linha_digitavel_valida(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            // Ambiente - Emissão pela empresa
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa;
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



        [TestCase(276.15, "458", "BB874A", "1", "00000458-02", "04191693400000276152103401234560000004584090", "04192.10349 01234.560009 00045.840907 1 69340000027615", 2016, 10, 1)]
        [TestCase(647.34, "451", "BB815A", "2", "00000451-52", "04192687300000647342103401234560000004514041", "04192.10349 01234.560009 00045.140415 2 68730000064734", 2016, 8, 1)]
        [TestCase(293.23, "452", "BB856A", "3", "00000452-33", "04193693500000293232103401234560000004524020", "04192.10349 01234.560009 00045.240207 3 69350000029323", 2016, 10, 2)]
        [TestCase(217.12, "453", "BB874A", "4", "00000453-14", "04194690500000217122103401234560000004534018", "04192.10349 01234.560009 00045.340189 4 69050000021712", 2016, 9, 2)]
        [TestCase(829.21, "454", "BB933A", "5", "00000454-97", "04195702600000829212103401234560000004544080", "04192.10349 01234.560009 00045.440807 5 70260000082921", 2017, 1, 1)]
        [TestCase(270.54, "459", "BB932A", "6", "00000459-85", "04196702600000270542103401234560000004594088", "04192.10349 01234.560009 00045.940889 6 70260000027054", 2017, 1, 1)]
        [TestCase(287.25, "456", "BB834A", "7", "00000456-40", "04197690500000287252103401234560000004564030", "04192.10349 01234.560009 00045.640307 7 69050000028725", 2016, 9, 2)]
        [TestCase(288.26, "455", "BB833A", "8", "00000455-78", "04198690500000288262103401234560000004554051", "04192.10349 01234.560009 00045.540515 8 69050000028826", 2016, 9, 2)]
        [TestCase(131.57, "457", "BB943A", "9", "00000457-21", "04199690400000131572103401234560000004574028", "04192.10349 01234.560009 00045.740289 9 69040000013157", 2016, 9, 1)]
        public void Deve_criar_boleto_banrisul_01_com_tipo_emissao_empresa_e_digito_verificador_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            // Ambiente - Emissão pela empresa
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa;
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

            Assert.That(boleto.CodigoBarra.DigitoVerificador, Is.EqualTo(digitoVerificador), $"Dígito Verificador diferente de {digitoVerificador}");

        }

        [TestCase(276.15, "458", "BB874A", "1", "00000458-02", "04191693400000276152103401234560000004584090", "04192.10349 01234.560009 00045.840907 1 69340000027615", 2016, 10, 1)]
        [TestCase(647.34, "451", "BB815A", "2", "00000451-52", "04192687300000647342103401234560000004514041", "04192.10349 01234.560009 00045.140415 2 68730000064734", 2016, 8, 1)]
        [TestCase(293.23, "452", "BB856A", "3", "00000452-33", "04193693500000293232103401234560000004524020", "04192.10349 01234.560009 00045.240207 3 69350000029323", 2016, 10, 2)]
        [TestCase(217.12, "453", "BB874A", "4", "00000453-14", "04194690500000217122103401234560000004534018", "04192.10349 01234.560009 00045.340189 4 69050000021712", 2016, 9, 2)]
        [TestCase(829.21, "454", "BB933A", "5", "00000454-97", "04195702600000829212103401234560000004544080", "04192.10349 01234.560009 00045.440807 5 70260000082921", 2017, 1, 1)]
        [TestCase(270.54, "459", "BB932A", "6", "00000459-85", "04196702600000270542103401234560000004594088", "04192.10349 01234.560009 00045.940889 6 70260000027054", 2017, 1, 1)]
        [TestCase(287.25, "456", "BB834A", "7", "00000456-40", "04197690500000287252103401234560000004564030", "04192.10349 01234.560009 00045.640307 7 69050000028725", 2016, 9, 2)]
        [TestCase(288.26, "455", "BB833A", "8", "00000455-78", "04198690500000288262103401234560000004554051", "04192.10349 01234.560009 00045.540515 8 69050000028826", 2016, 9, 2)]
        [TestCase(131.57, "457", "BB943A", "9", "00000457-21", "04199690400000131572103401234560000004574028", "04192.10349 01234.560009 00045.740289 9 69040000013157", 2016, 9, 1)]
        public void Deve_criar_boleto_banrisul_01_com_tipo_emissao_empresa_e_codigo_de_barras_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            // Ambiente - Emissão pela empresa
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa;
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

            Assert.That(boleto.CodigoBarra.CodigoDeBarras, Is.EqualTo(codigoDeBarras), "Código de Barra inválido");
        }

        #endregion

        #region Tipo Impressao Banco

        [TestCase(276.15, "458", "BB874A", "1", "00000458-02", "04191693400000276152103401234560000004584090", "04192.10349 01234.560009 00045.840907 1 69340000027615", 2016, 10, 1)]
        [TestCase(647.34, "451", "BB815A", "2", "00000451-52", "04192687300000647342103401234560000004514041", "04192.10349 01234.560009 00045.140415 2 68730000064734", 2016, 8, 1)]
        [TestCase(293.23, "452", "BB856A", "3", "00000452-33", "04193693500000293232103401234560000004524020", "04192.10349 01234.560009 00045.240207 3 69350000029323", 2016, 10, 2)]
        [TestCase(217.12, "453", "BB874A", "4", "00000453-14", "04194690500000217122103401234560000004534018", "04192.10349 01234.560009 00045.340189 4 69050000021712", 2016, 9, 2)]
        [TestCase(829.21, "454", "BB933A", "5", "00000454-97", "04195702600000829212103401234560000004544080", "04192.10349 01234.560009 00045.440807 5 70260000082921", 2017, 1, 1)]
        [TestCase(270.54, "459", "BB932A", "6", "00000459-85", "04196702600000270542103401234560000004594088", "04192.10349 01234.560009 00045.940889 6 70260000027054", 2017, 1, 1)]
        [TestCase(287.25, "456", "BB834A", "7", "00000456-40", "04197690500000287252103401234560000004564030", "04192.10349 01234.560009 00045.640307 7 69050000028725", 2016, 9, 2)]
        [TestCase(288.26, "455", "BB833A", "8", "00000455-78", "04198690500000288262103401234560000004554051", "04192.10349 01234.560009 00045.540515 8 69050000028826", 2016, 9, 2)]
        [TestCase(131.57, "457", "BB943A", "9", "00000457-21", "04199690400000131572103401234560000004574028", "04192.10349 01234.560009 00045.740289 9 69040000013157", 2016, 9, 1)]
        public void Deve_criar_boleto_banrisul_01_com_tipo_emissao_banco_e_nosso_numero_formatado_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            // Ambiente - Emissão pela empresa
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Banco;
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


        [TestCase(276.15, "458", "BB874A", "1", "00000458-02", "04191693400000276152103401234560000004584090", "04192.10349 01234.560009 00045.840907 1 69340000027615", 2016, 10, 1)]
        [TestCase(647.34, "451", "BB815A", "2", "00000451-52", "04192687300000647342103401234560000004514041", "04192.10349 01234.560009 00045.140415 2 68730000064734", 2016, 8, 1)]
        [TestCase(293.23, "452", "BB856A", "3", "00000452-33", "04193693500000293232103401234560000004524020", "04192.10349 01234.560009 00045.240207 3 69350000029323", 2016, 10, 2)]
        [TestCase(217.12, "453", "BB874A", "4", "00000453-14", "04194690500000217122103401234560000004534018", "04192.10349 01234.560009 00045.340189 4 69050000021712", 2016, 9, 2)]
        [TestCase(829.21, "454", "BB933A", "5", "00000454-97", "04195702600000829212103401234560000004544080", "04192.10349 01234.560009 00045.440807 5 70260000082921", 2017, 1, 1)]
        [TestCase(270.54, "459", "BB932A", "6", "00000459-85", "04196702600000270542103401234560000004594088", "04192.10349 01234.560009 00045.940889 6 70260000027054", 2017, 1, 1)]
        [TestCase(287.25, "456", "BB834A", "7", "00000456-40", "04197690500000287252103401234560000004564030", "04192.10349 01234.560009 00045.640307 7 69050000028725", 2016, 9, 2)]
        [TestCase(288.26, "455", "BB833A", "8", "00000455-78", "04198690500000288262103401234560000004554051", "04192.10349 01234.560009 00045.540515 8 69050000028826", 2016, 9, 2)]
        [TestCase(131.57, "457", "BB943A", "9", "00000457-21", "04199690400000131572103401234560000004574028", "04192.10349 01234.560009 00045.740289 9 69040000013157", 2016, 9, 1)]
        public void Deve_criar_boleto_banrisul_01_com_tipo_emissao_banco_e_linha_digitavel_valida(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            // Ambiente - Emissão pela empresa
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Banco;
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



        [TestCase(276.15, "458", "BB874A", "1", "00000458-02", "04191693400000276152103401234560000004584090", "04192.10349 01234.560009 00045.840907 1 69340000027615", 2016, 10, 1)]
        [TestCase(647.34, "451", "BB815A", "2", "00000451-52", "04192687300000647342103401234560000004514041", "04192.10349 01234.560009 00045.140415 2 68730000064734", 2016, 8, 1)]
        [TestCase(293.23, "452", "BB856A", "3", "00000452-33", "04193693500000293232103401234560000004524020", "04192.10349 01234.560009 00045.240207 3 69350000029323", 2016, 10, 2)]
        [TestCase(217.12, "453", "BB874A", "4", "00000453-14", "04194690500000217122103401234560000004534018", "04192.10349 01234.560009 00045.340189 4 69050000021712", 2016, 9, 2)]
        [TestCase(829.21, "454", "BB933A", "5", "00000454-97", "04195702600000829212103401234560000004544080", "04192.10349 01234.560009 00045.440807 5 70260000082921", 2017, 1, 1)]
        [TestCase(270.54, "459", "BB932A", "6", "00000459-85", "04196702600000270542103401234560000004594088", "04192.10349 01234.560009 00045.940889 6 70260000027054", 2017, 1, 1)]
        [TestCase(287.25, "456", "BB834A", "7", "00000456-40", "04197690500000287252103401234560000004564030", "04192.10349 01234.560009 00045.640307 7 69050000028725", 2016, 9, 2)]
        [TestCase(288.26, "455", "BB833A", "8", "00000455-78", "04198690500000288262103401234560000004554051", "04192.10349 01234.560009 00045.540515 8 69050000028826", 2016, 9, 2)]
        [TestCase(131.57, "457", "BB943A", "9", "00000457-21", "04199690400000131572103401234560000004574028", "04192.10349 01234.560009 00045.740289 9 69040000013157", 2016, 9, 1)]
        public void Deve_criar_boleto_banrisul_01_com_tipo_emissao_banco_e_digito_verificador_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            // Ambiente - Emissão pela empresa
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Banco;
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

            Assert.That(boleto.CodigoBarra.DigitoVerificador, Is.EqualTo(digitoVerificador), $"Dígito Verificador diferente de {digitoVerificador}");

        }

        [TestCase(276.15, "458", "BB874A", "1", "00000458-02", "04191693400000276152103401234560000004584090", "04192.10349 01234.560009 00045.840907 1 69340000027615", 2016, 10, 1)]
        [TestCase(647.34, "451", "BB815A", "2", "00000451-52", "04192687300000647342103401234560000004514041", "04192.10349 01234.560009 00045.140415 2 68730000064734", 2016, 8, 1)]
        [TestCase(293.23, "452", "BB856A", "3", "00000452-33", "04193693500000293232103401234560000004524020", "04192.10349 01234.560009 00045.240207 3 69350000029323", 2016, 10, 2)]
        [TestCase(217.12, "453", "BB874A", "4", "00000453-14", "04194690500000217122103401234560000004534018", "04192.10349 01234.560009 00045.340189 4 69050000021712", 2016, 9, 2)]
        [TestCase(829.21, "454", "BB933A", "5", "00000454-97", "04195702600000829212103401234560000004544080", "04192.10349 01234.560009 00045.440807 5 70260000082921", 2017, 1, 1)]
        [TestCase(270.54, "459", "BB932A", "6", "00000459-85", "04196702600000270542103401234560000004594088", "04192.10349 01234.560009 00045.940889 6 70260000027054", 2017, 1, 1)]
        [TestCase(287.25, "456", "BB834A", "7", "00000456-40", "04197690500000287252103401234560000004564030", "04192.10349 01234.560009 00045.640307 7 69050000028725", 2016, 9, 2)]
        [TestCase(288.26, "455", "BB833A", "8", "00000455-78", "04198690500000288262103401234560000004554051", "04192.10349 01234.560009 00045.540515 8 69050000028826", 2016, 9, 2)]
        [TestCase(131.57, "457", "BB943A", "9", "00000457-21", "04199690400000131572103401234560000004574028", "04192.10349 01234.560009 00045.740289 9 69040000013157", 2016, 9, 1)]
        public void Deve_criar_boleto_banrisul_01_com_tipo_emissao_banco_e_codigo_de_barras_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            // Ambiente - Emissão pela empresa
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Banco;
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

            Assert.That(boleto.CodigoBarra.CodigoDeBarras, Is.EqualTo(codigoDeBarras), "Código de Barra inválido");
        }


        #endregion
    }
}