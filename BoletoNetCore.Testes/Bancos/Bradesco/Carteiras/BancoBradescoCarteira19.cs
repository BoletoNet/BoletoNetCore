using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [System.ComponentModel.Category("Bradesco Carteira 19")]
    public class BancoBradescoCarteira19
    {
        readonly IBanco _banco;

        // Arquivo de retorno CNAB400 para a Carteira 19.
        // Estrutura identica ao da Carteira 09, com as seguintes diferencas:
        //   - Posicoes 022-024 dos registros Tipo 1: "019" (em vez de "009")
        //   - Posicoes 002-004 do registro Tipo 4 (QrCode): "019" (em vez de "009")
        //   - Posicao 108 dos registros Tipo 1: "9" (ultimo digito de "19" — mesmo comportamento da 09)
        // Contem 3 boletos: Entrada Confirmada (02) + QrCode, Liquidacao Normal (06), Baixa (10).
        const string arquivoRetorno = @"02RETORNO01COBRANCA       00000000000007654321YYYYYYY CLUBE XXXXX CONTA     237BRADESCO       0408250160000000103                                                                                                                                                                                                                                                                          050825         000001
1028888888800018400000190999900555000                         000000000000001068470000000000000000000000000902040825000001068400000000000000106847070825000000000400023704422  000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000                          P100000000                                                                  000002
4019099990055500000000106847qrpix.bradesco.com.br/qr/v2/cobv/ad6726a-aaaaa-b48ff27ecc382c758e025ba6cccc  20250804237093595554460000000011184                                                                                                                                                                                                                                                              000003
102888888880001840000019099990055500000000000000865100035197250000000000000008651P000000000000000000000000090604082500000086510000000000000008651P050725000000000300010400991  000000000000000000000000000000000000000000000000000000000000000000000000000000000000000300000000000000000000000000000   050825             00000000000000                                                                  000004
1028888888800018400000190999900555000                         000000000000000882370000000000000000000000000910040825000000882300000000000000088237030725000000000200023700000  000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000                          1600000000                                                                  000005
9                                                                                                                                                                                                                                                                                                                                                                                                         000006
";

        public BancoBradescoCarteira19()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "X",
                Conta = "123456",
                DigitoConta = "X",
                CarteiraPadrao = "19",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.Bradesco);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("1213141", "", "", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Bradesco_19_REM240()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoBradescoCarteira19), 5, true, "?", 223344);
        }

        [Test]
        public void Bradesco_19_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoBradescoCarteira19), 5, true, "?", 223344);
        }

        // Os valores de DV, NossoNumeroFormatado, CodigoDeBarras e LinhaDigitavel abaixo foram
        // calculados com base nas mesmas regras da Carteira 09, substituindo o prefixo "09" por "19"
        // no calculo do DV (Modulo 11 base 7) e no campo livre do codigo de barras.
        // A validacao dos calculos foi feita em Python contra os TestCases conhecidos da Carteira 09.

        // digitoVerificador = posicao 5 do codigo de barras (CB[4]) = DV do codigo de barras
        // nossoNumeroFormatado = "019/NNNNNNNNNNN-DV" onde DV e o resultado do Mod11 base 7
        [TestCase(141.50, "453", "BB943A", "4", "019/00000000453-8", "23794690400000141501234190000000045301234560", "23791.23413 90000.000043 53012.345608 4 69040000014150", 2016, 9, 1)]
        [TestCase(2717.16, "456", "BB874A", "7", "019/00000000456-2", "23797693400002717161234190000000045601234560", "23791.23413 90000.000043 56012.345601 7 69340000271716", 2016, 10, 1)]
        [TestCase(297.21, "444", "BB834A", "8", "019/00000000444-9", "23798690500000297211234190000000044401234560", "23791.23413 90000.000043 44012.345607 8 69050000029721", 2016, 9, 2)]
        [TestCase(297.21, "468", "BB856A", "9", "019/00000000468-6", "23799693500000297211234190000000046801234560", "23791.23413 90000.000043 68012.345606 9 69350000029721", 2016, 10, 2)]
        [TestCase(297.21, "443", "BB833A", "1", "019/00000000443-0", "23791690500000297211234190000000044301234560", "23791.23413 90000.000043 43012.345609 1 69050000029721", 2016, 9, 2)]
        [TestCase(649.39, "414", "BB815A", "1", "019/00000000414-7", "23791687300000649391234190000000041401234560", "23791.23413 90000.000043 14012.345600 1 68730000064939", 2016, 8, 1)]
        [TestCase(270.00, "561", "BB932A", "1", "019/00000000561-5", "23791702600000270001234190000000056101234560", "23791.23413 90000.000050 61012.345601 1 70260000027000", 2017, 1, 1)]
        [TestCase(2924.11, "445", "BB874A", "2", "019/00000000445-7", "23792690500002924111234190000000044501234560", "23791.23413 90000.000043 45012.345604 2 69050000292411", 2016, 9, 2)]
        [TestCase(830.00, "562", "BB933A", "3", "019/00000000562-3", "23793702600000830001234190000000056201234560", "23791.23413 90000.000050 62012.345609 3 70260000083000", 2017, 1, 1)]
        public void Deve_criar_boleto_bradesco_19_com_linha_digitavel_valida(
            decimal valorTitulo, string nossoNumero, string numeroDocumento,
            string digitoVerificador, string nossoNumeroFormatado,
            string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };
            boleto.ValidarDados();
            Assert.That(boleto.CodigoBarra.LinhaDigitavel, Is.EqualTo(linhaDigitavel), "Linha digitavel invalida");
        }

        [TestCase(141.50, "453", "BB943A", "4", "019/00000000453-8", "23794690400000141501234190000000045301234560", "23791.23413 90000.000043 53012.345608 4 69040000014150", 2016, 9, 1)]
        [TestCase(2717.16, "456", "BB874A", "7", "019/00000000456-2", "23797693400002717161234190000000045601234560", "23791.23413 90000.000043 56012.345601 7 69340000271716", 2016, 10, 1)]
        [TestCase(297.21, "444", "BB834A", "8", "019/00000000444-9", "23798690500000297211234190000000044401234560", "23791.23413 90000.000043 44012.345607 8 69050000029721", 2016, 9, 2)]
        [TestCase(297.21, "468", "BB856A", "9", "019/00000000468-6", "23799693500000297211234190000000046801234560", "23791.23413 90000.000043 68012.345606 9 69350000029721", 2016, 10, 2)]
        [TestCase(297.21, "443", "BB833A", "1", "019/00000000443-0", "23791690500000297211234190000000044301234560", "23791.23413 90000.000043 43012.345609 1 69050000029721", 2016, 9, 2)]
        [TestCase(649.39, "414", "BB815A", "1", "019/00000000414-7", "23791687300000649391234190000000041401234560", "23791.23413 90000.000043 14012.345600 1 68730000064939", 2016, 8, 1)]
        [TestCase(270.00, "561", "BB932A", "1", "019/00000000561-5", "23791702600000270001234190000000056101234560", "23791.23413 90000.000050 61012.345601 1 70260000027000", 2017, 1, 1)]
        [TestCase(2924.11, "445", "BB874A", "2", "019/00000000445-7", "23792690500002924111234190000000044501234560", "23791.23413 90000.000043 45012.345604 2 69050000292411", 2016, 9, 2)]
        [TestCase(830.00, "562", "BB933A", "3", "019/00000000562-3", "23793702600000830001234190000000056201234560", "23791.23413 90000.000050 62012.345609 3 70260000083000", 2017, 1, 1)]
        public void Deve_criar_boleto_bradesco_19_com_digito_verificador_valido(
            decimal valorTitulo, string nossoNumero, string numeroDocumento,
            string digitoVerificador, string nossoNumeroFormatado,
            string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };
            boleto.ValidarDados();
            Assert.That(boleto.CodigoBarra.DigitoVerificador, Is.EqualTo(digitoVerificador),
                $"Digito Verificador diferente de {digitoVerificador}");
        }

        [TestCase(141.50, "453", "BB943A", "4", "019/00000000453-8", "23794690400000141501234190000000045301234560", "23791.23413 90000.000043 53012.345608 4 69040000014150", 2016, 9, 1)]
        [TestCase(2717.16, "456", "BB874A", "7", "019/00000000456-2", "23797693400002717161234190000000045601234560", "23791.23413 90000.000043 56012.345601 7 69340000271716", 2016, 10, 1)]
        [TestCase(297.21, "444", "BB834A", "8", "019/00000000444-9", "23798690500000297211234190000000044401234560", "23791.23413 90000.000043 44012.345607 8 69050000029721", 2016, 9, 2)]
        [TestCase(297.21, "468", "BB856A", "9", "019/00000000468-6", "23799693500000297211234190000000046801234560", "23791.23413 90000.000043 68012.345606 9 69350000029721", 2016, 10, 2)]
        [TestCase(297.21, "443", "BB833A", "1", "019/00000000443-0", "23791690500000297211234190000000044301234560", "23791.23413 90000.000043 43012.345609 1 69050000029721", 2016, 9, 2)]
        [TestCase(649.39, "414", "BB815A", "1", "019/00000000414-7", "23791687300000649391234190000000041401234560", "23791.23413 90000.000043 14012.345600 1 68730000064939", 2016, 8, 1)]
        [TestCase(270.00, "561", "BB932A", "1", "019/00000000561-5", "23791702600000270001234190000000056101234560", "23791.23413 90000.000050 61012.345601 1 70260000027000", 2017, 1, 1)]
        [TestCase(2924.11, "445", "BB874A", "2", "019/00000000445-7", "23792690500002924111234190000000044501234560", "23791.23413 90000.000043 45012.345604 2 69050000292411", 2016, 9, 2)]
        [TestCase(830.00, "562", "BB933A", "3", "019/00000000562-3", "23793702600000830001234190000000056201234560", "23791.23413 90000.000050 62012.345609 3 70260000083000", 2017, 1, 1)]
        public void Deve_criar_boleto_bradesco_19_com_nosso_numero_valido(
            decimal valorTitulo, string nossoNumero, string numeroDocumento,
            string digitoVerificador, string nossoNumeroFormatado,
            string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };
            boleto.ValidarDados();
            Assert.That(boleto.NossoNumeroFormatado, Is.EqualTo(nossoNumeroFormatado), "Nosso numero invalido");
        }

        [TestCase(141.50, "453", "BB943A", "4", "019/00000000453-8", "23794690400000141501234190000000045301234560", "23791.23413 90000.000043 53012.345608 4 69040000014150", 2016, 9, 1)]
        [TestCase(2717.16, "456", "BB874A", "7", "019/00000000456-2", "23797693400002717161234190000000045601234560", "23791.23413 90000.000043 56012.345601 7 69340000271716", 2016, 10, 1)]
        [TestCase(297.21, "444", "BB834A", "8", "019/00000000444-9", "23798690500000297211234190000000044401234560", "23791.23413 90000.000043 44012.345607 8 69050000029721", 2016, 9, 2)]
        [TestCase(297.21, "468", "BB856A", "9", "019/00000000468-6", "23799693500000297211234190000000046801234560", "23791.23413 90000.000043 68012.345606 9 69350000029721", 2016, 10, 2)]
        [TestCase(297.21, "443", "BB833A", "1", "019/00000000443-0", "23791690500000297211234190000000044301234560", "23791.23413 90000.000043 43012.345609 1 69050000029721", 2016, 9, 2)]
        [TestCase(649.39, "414", "BB815A", "1", "019/00000000414-7", "23791687300000649391234190000000041401234560", "23791.23413 90000.000043 14012.345600 1 68730000064939", 2016, 8, 1)]
        [TestCase(270.00, "561", "BB932A", "1", "019/00000000561-5", "23791702600000270001234190000000056101234560", "23791.23413 90000.000050 61012.345601 1 70260000027000", 2017, 1, 1)]
        [TestCase(2924.11, "445", "BB874A", "2", "019/00000000445-7", "23792690500002924111234190000000044501234560", "23791.23413 90000.000043 45012.345604 2 69050000292411", 2016, 9, 2)]
        [TestCase(830.00, "562", "BB933A", "3", "019/00000000562-3", "23793702600000830001234190000000056201234560", "23791.23413 90000.000050 62012.345609 3 70260000083000", 2017, 1, 1)]
        public void Deve_criar_boleto_bradesco_19_com_codigo_de_barras_valido(
            decimal valorTitulo, string nossoNumero, string numeroDocumento,
            string digitoVerificador, string nossoNumeroFormatado,
            string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };
            boleto.ValidarDados();
            Assert.That(boleto.CodigoBarra.CodigoDeBarras, Is.EqualTo(codigoDeBarras), "Codigo de Barra invalido");
        }

        // ─── Leitura de Arquivo Retorno ───────────────────────────────────────────────

        // O arquivo de retorno contem 3 boletos (ocorrencias 02, 06 e 10) com carteira 19.
        // Diferenca para o retorno da carteira 09: posicoes 022-024 dos registros Tipo 1
        // contem "019" e posicoes 002-004 do registro Tipo 4 contem "019".

        [Test]
        public void LerRetorno_validando_Registro_QrCode()
        {
            string qrCode = "qrpix.bradesco.com.br/qr/v2/cobv/ad6726a-aaaaa-b48ff27ecc382c758e025ba6cccc  ";
            var buffer = Encoding.ASCII.GetBytes(arquivoRetorno);
            var mem = new MemoryStream(buffer);
            var boletos = new ArquivoRetorno(mem);

            Assert.AreEqual(3, boletos.Boletos.Count);
            Assert.AreEqual("Entrada Confirmada", boletos.Boletos[0].DescricaoMovimentoRetorno);
            Assert.AreEqual(qrCode, boletos.Boletos[0].QRCode);
            Assert.AreEqual("20250804237093595554460000000011184", boletos.Boletos[0].TxId);
        }

        [Test]
        public void LerRetorno_validando_Liquidacao()
        {
            var buffer = Encoding.ASCII.GetBytes(arquivoRetorno);
            var mem = new MemoryStream(buffer);
            var boletos = new ArquivoRetorno(mem);

            Assert.AreEqual(3, boletos.Boletos.Count);
            Assert.AreEqual("Liquidação normal", boletos.Boletos[1].DescricaoMovimentoRetorno);
            Assert.AreEqual(30.00M, boletos.Boletos[1].ValorPago);
            Assert.AreEqual(new DateTime(2025, 8, 5), boletos.Boletos[1].DataCredito);
        }

        [Test]
        public void LerRetorno_validando_Baixa()
        {
            var buffer = Encoding.ASCII.GetBytes(arquivoRetorno);
            var mem = new MemoryStream(buffer);
            var boletos = new ArquivoRetorno(mem);

            Assert.AreEqual(3, boletos.Boletos.Count);
            Assert.AreEqual("Baixado conforme instruções da Agência", boletos.Boletos[2].DescricaoMovimentoRetorno);
        }
    }
}
