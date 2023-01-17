using System;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Itau Carteira 138")]
    public class BancoItauCarteira138Tests
    {
        readonly IBanco _banco;
        public BancoItauCarteira138Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "",
                Conta = "56789",
                DigitoConta = "0",
                CarteiraPadrao = "138",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.Itau);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("", "", "", contaBancaria);

            _banco.FormataBeneficiario();
        }

        [Test]
        public void Itau_138_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoItauCarteira138Tests), 5, true, "N", 223344);
        }

        [Test]
        public void Itau_138_REM240()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoItauCarteira138Tests), 5, true, "N", 223344);
        }

        [TestCase(100.00, "223345", "BB000001A", "2", "138/00223345-1", "34192719500000100001380022334511234567890000", "34191.38007 22334.511239 45678.900007 2 71950000010000", 2017, 06, 19)]
        [TestCase(200.00, "223346", "BB000002B", "1", "138/00223346-9", "34191722500000200001380022334691234567890000", "34191.38007 22334.691239 45678.900007 1 72250000020000", 2017, 07, 19)]
        [TestCase(300.00, "223347", "BB000003C", "8", "138/00223347-7", "34198725600000300001380022334771234567890000", "34191.38007 22334.771239 45678.900007 8 72560000030000", 2017, 08, 19)]
        [TestCase(400.00, "223348", "BB000004D", "6", "138/00223348-5", "34196728700000400001380022334851234567890000", "34191.38007 22334.851239 45678.900007 6 72870000040000", 2017, 09, 19)]
        [TestCase(500.00, "223349", "BB000005E", "7", "138/00223349-3", "34197731700000500001380022334931234567890000", "34191.38007 22334.931239 45678.900007 7 73170000050000", 2017, 10, 19)]
        [TestCase(600.00, "223350", "BB000001F", "9", "138/00223350-1", "34199719500000600001380022335011234567890000", "34191.38007 22335.011239 45678.900007 9 71950000060000", 2017, 06, 19)]
        [TestCase(700.01, "223351", "BB000001G", "1", "138/00223351-9", "34191719500000700011380022335191234567890000", "34191.38007 22335.191239 45678.900007 1 71950000070001", 2017, 06, 19)]
        [TestCase(800.08, "223352", "BB000001H", "1", "138/00223352-7", "34191719500000800081380022335271234567890000", "34191.38007 22335.271239 45678.900007 1 71950000080008", 2017, 06, 19)]
        [TestCase(900.03, "223353", "BB000001I", "4", "138/00223353-5", "34194719500000900031380022335351234567890000", "34191.38007 22335.351239 45678.900007 4 71950000090003", 2017, 06, 19)]

        public void Itau_138_BoletoOK(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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
        }
    }
}