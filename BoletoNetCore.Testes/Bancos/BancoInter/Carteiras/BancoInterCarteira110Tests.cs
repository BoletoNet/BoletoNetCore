using NUnit.Framework;
using System;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Banco Inter Carteira 110")]
    public class BancoInterCarteira110Tests
    {
        readonly IBanco _banco;
        public BancoInterCarteira110Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "0001",
                DigitoAgencia = "9",
                Conta = "12345606",
                DigitoConta = "0",
                CarteiraPadrao = "110",
                VariacaoCarteiraPadrao = "",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro
            };
            _banco = Banco.Instancia(Bancos.BancoInter);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("1234567", "", "", contaBancaria);
            _banco.FormataBeneficiario();
        }

        #region Tipo Impressao Empresa

        [Test]
        public void BancoInter_REM400_EmpresaEmite()
        {
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa;
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoInterCarteira110Tests) + "_EmpresaEmite", 5, true, "?", 1);
        }

        [Test]
        public void BancoInter_REM400_EmpresaEmiteremessa()
        {
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa;
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, "CI400_001_0000001", 1, true, "?", 0);
        }

        [TestCase(5.00, "4309540", "0004309540-1", 2023, 8, 31)]
        [TestCase(5.00, "1", "0000000001-4", 2023, 8, 31)]
        [TestCase(5.00, "4309541", "0004309541-9", 2023, 8, 31)]
        [TestCase(5.00, "4309542", "0004309542-7", 2023, 8, 31)]
        [TestCase(5.00, "4309543", "0004309543-5", 2023, 8, 31)]
        public void Deve_criar_boleto_BancoInter_com_tipo_emissao_empresa_e_nosso_numero_formatado_valido(decimal valorTitulo, string nossoNumero, string nossoNumeroFormatado, params int[] anoMesDia)
        {
            // Ambiente - Emissão pela empresa
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa;
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

            //Ação
            boleto.ValidarDados();

            //Assertivas
            Assert.That(boleto.NossoNumeroFormatado, Is.EqualTo(nossoNumeroFormatado), "Nosso número inválido");
        }


        [TestCase(276.15, "458", "07790.00116 10123.456708 00000.045864 2 69340000027615", 2016, 10, 1)]
        [TestCase(647.34, "451", "07790.00116 10123.456708 00000.045112 1 68730000064734", 2016, 8, 1)]
        [TestCase(293.23, "452", "07790.00116 10123.456708 00000.045294 3 69350000029323", 2016, 10, 2)]
        [TestCase(217.12, "453", "07790.00116 10123.456708 00000.045377 2 69050000021712", 2016, 9, 2)]
        [TestCase(131.57, "457", "07790.00116 10123.456708 00000.045781 9 69040000013157", 2016, 9, 1)]
        [TestCase(5.00, "1", "07790.00116 10123.456708 00000.000141 1 94590000000500", 2023, 8, 31)]
        public void Deve_criar_boleto_BancoInter_com_tipo_emissao_empresa_e_linha_digitavel_valida(decimal valorTitulo, string nossoNumero, string linhaDigitavel, params int[] anoMesDia)
        {
            // Ambiente - Emissão pela empresa
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa;
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

            //Ação
            boleto.ValidarDados();

            Assert.That(boleto.CodigoBarra.LinhaDigitavel, Is.EqualTo(linhaDigitavel), "Linha digitável inválida");
        }



        [TestCase(276.15, "458", "2", 2016, 10, 1)]
        [TestCase(647.34, "451", "1", 2016, 8, 1)]
        [TestCase(293.23, "452", "3", 2016, 10, 2)]
        [TestCase(217.12, "453", "2", 2016, 9, 2)]
        [TestCase(131.57, "457", "9", 2016, 9, 1)]
        [TestCase(5.00, "1", "1", 2023, 8, 31)]
        public void Deve_criar_boleto_BancoInter_com_tipo_emissao_empresa_e_digito_verificador_valido(decimal valorTitulo, string nossoNumero, string digitoVerificador, params int[] anoMesDia)
        {
            // Ambiente - Emissão pela empresa
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa;
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

            //Ação
            boleto.ValidarDados();

            Assert.That(boleto.CodigoBarra.DigitoVerificador, Is.EqualTo(digitoVerificador), $"Dígito Verificador diferente de {digitoVerificador}");

        }

        [TestCase(276.15, "458", "07792693400000276150001110123456700000004586", 2016, 10, 1)]
        [TestCase(647.34, "451", "07791687300000647340001110123456700000004511", 2016, 8, 1)]
        [TestCase(293.23, "452", "07793693500000293230001110123456700000004529", 2016, 10, 2)]
        [TestCase(217.12, "453", "07792690500000217120001110123456700000004537", 2016, 9, 2)]
        [TestCase(131.57, "457", "07799690400000131570001110123456700000004578", 2016, 9, 1)]
        [TestCase(5.00, "1", "07791945900000005000001110123456700000000014", 2023, 8, 31)]
        public void Deve_criar_boleto_BancoInter_com_tipo_emissao_empresa_e_codigo_de_barras_valido(decimal valorTitulo, string nossoNumero, string codigoDeBarras, params int[] anoMesDia)
        {
            // Ambiente - Emissão pela empresa
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa;
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

            //Ação
            boleto.ValidarDados();

            Assert.That(boleto.CodigoBarra.CodigoDeBarras, Is.EqualTo(codigoDeBarras), "Código de Barra inválido");
        }

        #endregion

        #region Tipo Impressao Banco

        [Test]
        public void BancoInter_1_REM400_BancoEmite()
        {
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Banco;
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoInterCarteira110Tests) + "_BancoEmite", 5, true, "?", 0);
        }

        [TestCase(5.00, "4309540", "0004309540-1", 2023, 8, 31)]
        [TestCase(5.00, "1", "0000000001-4", 2023, 8, 31)]
        [TestCase(5.00, "4309541", "0004309541-9", 2023, 8, 31)]
        [TestCase(5.00, "4309542", "0004309542-7", 2023, 8, 31)]
        [TestCase(5.00, "4309543", "0004309543-5", 2023, 8, 31)]
        public void Deve_criar_boleto_BancoInter_com_tipo_emissao_banco_e_nosso_numero_formatado_valido(decimal valorTitulo, string nossoNumero, string nossoNumeroFormatado, params int[] anoMesDia)
        {
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Banco;
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

            //Ação
            boleto.ValidarDados();

            //Assertivas
            Assert.That(boleto.NossoNumeroFormatado, Is.EqualTo(nossoNumeroFormatado), "Nosso número inválido");
        }


        [TestCase(276.15, "458", "07790.00116 10123.456708 00000.045864 2 69340000027615", 2016, 10, 1)]
        [TestCase(647.34, "451", "07790.00116 10123.456708 00000.045112 1 68730000064734", 2016, 8, 1)]
        [TestCase(293.23, "452", "07790.00116 10123.456708 00000.045294 3 69350000029323", 2016, 10, 2)]
        [TestCase(217.12, "453", "07790.00116 10123.456708 00000.045377 2 69050000021712", 2016, 9, 2)]
        [TestCase(131.57, "457", "07790.00116 10123.456708 00000.045781 9 69040000013157", 2016, 9, 1)]
        [TestCase(5.00, "1", "07790.00116 10123.456708 00000.000141 1 94590000000500", 2023, 8, 31)]
        public void Deve_criar_boleto_BancoInter_com_tipo_emissao_banco_e_linha_digitavel_valida(decimal valorTitulo, string nossoNumero, string linhaDigitavel, params int[] anoMesDia)
        {
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Banco;
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

            //Ação
            boleto.ValidarDados();

            Assert.That(boleto.CodigoBarra.LinhaDigitavel, Is.EqualTo(linhaDigitavel), "Linha digitável inválida");
        }



        [TestCase(276.15, "458", "2", 2016, 10, 1)]
        [TestCase(647.34, "451", "1", 2016, 8, 1)]
        [TestCase(293.23, "452", "3", 2016, 10, 2)]
        [TestCase(217.12, "453", "2", 2016, 9, 2)]
        [TestCase(131.57, "457", "9", 2016, 9, 1)]
        [TestCase(5.00, "1", "1", 2023, 8, 31)]
        public void Deve_criar_boleto_BancoInter_com_tipo_emissao_banco_e_digito_verificador_valido(decimal valorTitulo, string nossoNumero, string digitoVerificador, params int[] anoMesDia)
        {
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Banco;
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

            //Ação
            boleto.ValidarDados();

            Assert.That(boleto.CodigoBarra.DigitoVerificador, Is.EqualTo(digitoVerificador), $"Dígito Verificador diferente de {digitoVerificador}");

        }

        [TestCase(276.15, "458", "07792693400000276150001110123456700000004586", 2016, 10, 1)]
        [TestCase(647.34, "451", "07791687300000647340001110123456700000004511", 2016, 8, 1)]
        [TestCase(293.23, "452", "07793693500000293230001110123456700000004529", 2016, 10, 2)]
        [TestCase(217.12, "453", "07792690500000217120001110123456700000004537", 2016, 9, 2)]
        [TestCase(131.57, "457", "07799690400000131570001110123456700000004578", 2016, 9, 1)]
        [TestCase(5.00, "1", "07791945900000005000001110123456700000000014", 2023, 8, 31)]
        public void Deve_criar_boleto_BancoInter_com_tipo_emissao_banco_e_codigo_de_barras_valido(decimal valorTitulo, string nossoNumero, string codigoDeBarras, params int[] anoMesDia)
        {
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Banco;
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
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