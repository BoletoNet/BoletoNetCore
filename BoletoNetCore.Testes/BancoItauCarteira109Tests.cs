using System;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Itau Carteira 109")]
    public class BancoItauCarteira109Tests
    {
        readonly IBanco _banco;
        public BancoItauCarteira109Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "",
                Conta = "56789",
                DigitoConta = "0",
                CarteiraPadrao = "109",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.Itau);
            _banco.Beneficiario = Utils.GerarBeneficiario("", "", "", contaBancaria);

            _banco.FormataBeneficiario();
        }

        [Test]
        public void Itau_109_REM400()
        {
            Utils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoItauCarteira109Tests), 5, true, "N", 223344);
        }

        [Test]
        public void Itau_109_REM240()
        {
            Utils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoItauCarteira109Tests), 5, true, "N", 223344);
        }

        [TestCase(600.00, "223350", "BB000001F", "1", "109/00223350-2", "34191719500000600001090022335021234567890000", "34191.09008 22335.021238 45678.900007 1 71950000060000", 2017, 06, 19)]
        [TestCase(800.08, "223352", "BB000001H", "2", "109/00223352-8", "34192719500000800081090022335281234567890000", "34191.09008 22335.281238 45678.900007 2 71950000080008", 2017, 06, 19)]
        [TestCase(100.00, "223345", "BB000001A", "3", "109/00223345-2", "34193719500000100001090022334521234567890000", "34191.09008 22334.521238 45678.900007 3 71950000010000", 2017, 06, 19)]
        [TestCase(200.00, "223346", "BB000002B", "4", "109/00223346-0", "34194722500000200001090022334601234567890000", "34191.09008 22334.601238 45678.900007 4 72250000020000", 2017, 07, 19)]
        [TestCase(900.03, "223353", "BB000001I", "5", "109/00223353-6", "34195719500000900031090022335361234567890000", "34191.09008 22335.361238 45678.900007 5 71950000090003", 2017, 06, 19)]
        [TestCase(700.01, "223351", "BB000001G", "6", "109/00223351-0", "34196719500000700011090022335101234567890000", "34191.09008 22335.101238 45678.900007 6 71950000070001", 2017, 06, 19)]
        [TestCase(400.00, "223348", "BB000004D", "7", "109/00223348-6", "34197728700000400001090022334861234567890000", "34191.09008 22334.861238 45678.900007 7 72870000040000", 2017, 09, 19)]
        [TestCase(500.00, "223349", "BB000005E", "8", "109/00223349-4", "34198731700000500001090022334941234567890000", "34191.09008 22334.941238 45678.900007 8 73170000050000", 2017, 10, 19)]
        [TestCase(300.00, "223347", "BB000003C", "9", "109/00223347-8", "34199725600000300001090022334781234567890000", "34191.09008 22334.781238 45678.900007 9 72560000030000", 2017, 08, 19)]

        public void Itau_109_BoletoOK(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            //Ambiente
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = Utils.GerarPagador()
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