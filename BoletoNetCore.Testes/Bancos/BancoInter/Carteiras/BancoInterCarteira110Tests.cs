using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
            _banco.Beneficiario = TestUtils.GerarBeneficiario("12345678", "8", "1234567", contaBancaria);
            _banco.FormataBeneficiario();
        }

        #region Tipo Impressao Empresa

        [Test]
        public void BancoInter_REM400_EmpresaEmite()
        {
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa;
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoInterCarteira110Tests) + "_EmpresaEmite", 5, true, "?", 1);
        }

        [TestCase(5.00, "4309540", "00019/110/0004309540-1", 2023, 8, 31)]
        [TestCase(5.00, "1", "00019/110/0000000001-4", 2023, 8, 31)]
        [TestCase(5.00, "4309541", "00019/110/0004309541-9", 2023, 8, 31)]
        [TestCase(5.00, "4309542", "00019/110/0004309542-7", 2023, 8, 31)]
        [TestCase(5.00, "4309543", "00019/110/0004309543-5", 2023, 8, 31)]
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

        [TestCase(5.00, "4309540", "00019/110/0004309540-1", 2023, 8, 31)]
        [TestCase(5.00, "1", "00019/110/0000000001-4", 2023, 8, 31)]
        [TestCase(5.00, "4309541", "00019/110/0004309541-9", 2023, 8, 31)]
        [TestCase(5.00, "4309542", "00019/110/0004309542-7", 2023, 8, 31)]
        [TestCase(5.00, "4309543", "00019/110/0004309543-5", 2023, 8, 31)]
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


        public string arquivoTeste = @"02RETORNO01COBRANCA                           EMPRESA TECNOLOGIA            077INTER          050923                                                                                                                                                                                                                                                                                                      000001
10207910714000186700011000010262458055CHAVEPRIMARIA1           00000000                11007040923                                        077000101             0000000000000000000                                                00000000000000Data de vencimento anterior ao dia de emissão do boleto! valor: 2023-09-03                                                                                000002
10207910714000186700011000010262458055CHAVEPRIMARIA1           00000000                11003040923                                        077000101             0000000000000000000                                                00000000000000Data de vencimento anterior ao dia de emissão do boleto! valor: 2023-09-03A data para desconto deve ser maior ou igual à data de emissão. va              000003
10207910714000186700011000010262458055CHAVEPRIMARIA1           00000000                11003040923                                        077000101             0000000000000000000                                                00000000000000Data de vencimento anterior ao dia de emissão do boleto! valor: 2023-09-03                                                                                000004
10207910714000186700011000010262458055CHAVEPRIMARIA1           00000000                11003040923                                        077000101             0000000000000000000                                                00000000000000Data de vencimento anterior ao dia de emissão do boleto! valor: 2023-09-03A data para desconto deve ser maior ou igual à data de emissão. va              000005
10207910714000186700011000010262458055CHAVEPRIMARIA1           00000000                11003040923                                        077000101             0000000000000000000                                                00000000000000Data de vencimento anterior ao dia de emissão do boleto! valor: 2023-09-03A data para desconto deve ser maior ou igual à data de emissão. va              000006
10207910714000186700011000010262458055CHAVEPRIMARIA1           00000000                11003040923                                        077000101             0000000000000000000                                                00000000000000Data de vencimento anterior ao dia de emissão do boleto! valor: 2023-09-03                                                                                000007
10207910714000186700011000010262458055CHAVEPRIMARIA1           00000000                11003040923                                        077000101             0000000000000000000                                                00000000000000Data da mora do título deve ser maior que a data de vencimento informada. valor: 2023-10-04Data da multa do título deve ser maior que a data              000008
10207910714000186700011000010262458055CHAVEPRIMARIA1           0000000030061000017     11002040923BB000001A 300610000170409230000000010000077000101             0000000000000000000   PAGADOR TESTE PJ                             71738978000101                                                                                                                                                          000009
10207910714000186700011000010262458055CHAVEPRIMARIA1           0000000030060999995     11006040923BB000001A 300609999950410230000000010000077000101             0000000009000040923   PAGADOR TESTE PJ                             71738978000101                                                                                                                                                          000010
10207910714000186700011000010262458055CHAVEPRIMARIA1           0000000030061000009     11006040923BB000001A 300610000090409230000000010000077000101             0000000010000040923   PAGADOR TESTE PJ                             71738978000101                                                                                                                                                          000011
9201077          00000010                                00001000000010000            00007                        00002000000020000                                                                                                                                                                                                                                                                      000012";


        [Test]
        public void LerRetorno_validando_motivo_retorno()
        {
           
            var buffer = Encoding.ASCII.GetBytes(arquivoTeste1);
            var mem = new MemoryStream(buffer);
            var boletos = new ArquivoRetorno(mem);

            Assert.AreEqual(1, boletos.Boletos.Count);
            Assert.AreEqual("Em aberto: ", boletos.Boletos[0].DescricaoMovimentoRetorno);
        }


        const string arquivoTeste1 = @"02RETORNO01COBRANCA                           UCONDO TECNOLOGIA S.A.        077INTER          311023                                                                                                                                                                                                                                                                                                      000001
1022394143300014700011000010262457938796B47DFE22F4D75933DA22C10000000030061000215     1100224102352A212E   300610002152510230000000000500077000101             0000000000000000000   ROBERTO CARLOS                               00044443206051                                                                                                                                                          000002
9201077          00000001                                00001000000000500            00000                        00000000000000000                                                                                                                                                                                                                                                                      000003";



        [Test]
        public void LerRetorno_validando_valor_pago()
        {

            var buffer = Encoding.ASCII.GetBytes(arquivoTeste);
            var mem = new MemoryStream(buffer);
            var boletos = new ArquivoRetorno(mem);

            Assert.AreEqual(10, boletos.Boletos.Count);
            Assert.AreEqual(0, boletos.Boletos[7].ValorPago);
            Assert.AreEqual(9, boletos.Boletos[8].ValorPago);
            Assert.AreEqual(10, boletos.Boletos[9].ValorPago);
        }

        [Test]
        public void LerRetorno_validando_valor_titulo()
        {

            var buffer = Encoding.ASCII.GetBytes(arquivoTeste1);
            var mem = new MemoryStream(buffer);
            var boletos = new ArquivoRetorno(mem);

            Assert.AreEqual(1, boletos.Boletos.Count);
            Assert.AreEqual(5, boletos.Boletos[0].ValorTitulo);
        }
    }
}