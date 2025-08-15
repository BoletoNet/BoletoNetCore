using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Santander Carteira 101")]
    public class BancoSantanderCarteira101Tests
    {
        readonly IBanco _banco;

        const string arquivoRetorno = @"02RETORNO01COBRANCA       11221654321916543219AAAAAA BBBBBBBBBB NOVO HAMBURG033SANTANDER      02092400000000000163305                                                                                                                                                                                                                                                                                  153000001
1029169988400017011341300340913003409900028566000000000000714400071447                                     50202092400000071440007144795508       15092400000000003000331134701000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000 N 020924Teste Testety                        00000000000000000000000000000000000000000C           153000002
2 pix.santander.com.br/qr/v2/cobv/aaaaaaaa-3dfa-4321-1234-aa00bb22cc33         XYZ000123455000000006044802099024                                                                                                                                                                                                                                                                                       153000003
9201033          000023900000001234567800000138                                                  000000000000000000000000000000          000000000000000000000000000000 ";

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


        [TestCase(TipoChavePix.CPF, "00000000191")]
        [TestCase(TipoChavePix.CNPJ, "81293564000161")]
        [TestCase(TipoChavePix.Celular, "51966331122")]
        [TestCase(TipoChavePix.Email, "teste@teste.com.br")]
        [TestCase(TipoChavePix.Aleatoria, "CBFEC37D-5C71-4D48-A952-C21A91BD552A")]
        public void Deve_criar_remessa_boleto_santander_com_segmento8_valido(TipoChavePix tipoChavePix, string chavePix)
        {
            var nomeArquivoREM = Path.Combine(Path.GetTempPath(), "BoletoNetCore", $"{Guid.NewGuid()}.REM");

            try
            {
                _banco.Beneficiario.ContaBancaria.TipoChavePix = tipoChavePix;
                _banco.Beneficiario.ContaBancaria.ChavePix = chavePix;

                int contadorLinha = 0;
                int quantidadeBoletos = 1;

                var boletos = TestUtils.GerarBoletos(_banco, quantidadeBoletos, "N", 223344);
                var arquivoRemessa = new ArquivoRemessa(boletos.Banco, TipoArquivo.CNAB400, 1);

                CriarPathArquivoRemessa(nomeArquivoREM);

                using (var fileStream = new FileStream(nomeArquivoREM, FileMode.Create))
                    arquivoRemessa.GerarArquivoRemessa(boletos, fileStream);

                if (!File.Exists(nomeArquivoREM))
                    Assert.Fail("Arquivo Remessa não encontrado: " + nomeArquivoREM);

                foreach (string linha in File.ReadLines(nomeArquivoREM))
                {
                    contadorLinha++;

                    Assert.AreEqual(contadorLinha, int.Parse(linha.Substring(397, 3)));
                    Assert.AreEqual(400, linha.Length);

                    if (linha.Substring(0, 1) == "8")
                    {
                        Assert.AreEqual(chavePix.ToUpper(), linha.Substring(43, chavePix.Length));
                    }
                }

                Assert.AreEqual((quantidadeBoletos * 2) + 2, contadorLinha);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    Assert.Fail(e.InnerException.ToString());
                else
                    Assert.Fail(e.Message);
            }
            finally
            {
                if (File.Exists(nomeArquivoREM))
                    File.Delete(nomeArquivoREM);
            }
        }

        private static void CriarPathArquivoRemessa(string nomeArquivoREM)
        {
            if (!Directory.Exists(Path.GetDirectoryName(nomeArquivoREM)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(nomeArquivoREM));
            }

            if (File.Exists(nomeArquivoREM))
            {
                File.Delete(nomeArquivoREM);
                if (File.Exists(nomeArquivoREM))
                    Assert.Fail("Arquivo Remessa não foi excluído: " + nomeArquivoREM);
            }
        }

        [Test]
        public void LerRetorno_validando_beneficiario()
        {
            var buffer = Encoding.ASCII.GetBytes(arquivoRetorno);
            var mem = new MemoryStream(buffer);
            var boletos = new ArquivoRetorno(mem);

            Assert.AreEqual("AAAAAA BBBBBBBBBB NOVO HAMBURG", boletos.Banco.Beneficiario.Nome);
            Assert.AreEqual("1122", boletos.Banco.Beneficiario.ContaBancaria.Agencia);
            Assert.AreEqual("9", boletos.Banco.Beneficiario.ContaBancaria.DigitoConta);
            Assert.AreEqual("1654321", boletos.Banco.Beneficiario.ContaBancaria.Conta);            
        }

        [Test]
        public void LerRetorno_validando_Registro_QrCode()
        {
            string qrCode = "pix.santander.com.br/qr/v2/cobv/aaaaaaaa-3dfa-4321-1234-aa00bb22cc33         ";
            var buffer = Encoding.ASCII.GetBytes(arquivoRetorno);
            var mem = new MemoryStream(buffer);
            var boletos = new ArquivoRetorno(mem);

            Assert.AreEqual(1, boletos.Boletos.Count);
            Assert.AreEqual("Entrada boleto Confirmada", boletos.Boletos[0].DescricaoMovimentoRetorno);
            Assert.AreEqual(qrCode, boletos.Boletos[0].QRCode);
            Assert.AreEqual("XYZ000123455000000006044802099024  ", boletos.Boletos[0].TxId);
        }
    }
}