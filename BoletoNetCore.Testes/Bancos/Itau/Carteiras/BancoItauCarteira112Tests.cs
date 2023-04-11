using System;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Itau Carteira 112")]
    public class BancoItauCarteira112Tests
    {
        readonly IBanco _banco;
        public BancoItauCarteira112Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "",
                Conta = "12345",
                DigitoConta = "6",
                CarteiraPadrao = "112",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Banco
            };
            _banco = Banco.Instancia(Bancos.Itau);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("", "", "", contaBancaria);

            _banco.FormataBeneficiario();
        }

        [Test]
        public void Itau_112_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoItauCarteira112Tests), 5, true, "N", 223344);
        }


        [TestCase(101.01, "223345", "BB000001A", "1", "112/00223345-8", "34191719500000101011120022334581234123456000", "34191.12002 22334.581232 41234.560005 1 71950000010101", 2017, 06, 19)]
        [TestCase(202.06, "223346", "BB000002B", "2", "112/00223346-6", "34192722500000202061120022334661234123456000", "34191.12002 22334.661232 41234.560005 2 72250000020206", 2017, 07, 19)]
        [TestCase(903.47, "223353", "BB000001I", "3", "112/00223353-2", "34193719500000903471120022335321234123456000", "34191.12002 22335.321232 41234.560005 3 71950000090347", 2017, 06, 19)]
        [TestCase(704.16, "223351", "BB000001G", "4", "112/00223351-6", "34194719500000704161120022335161234123456000", "34191.12002 22335.161232 41234.560005 4 71950000070416", 2017, 06, 19)]
        [TestCase(405.06, "223348", "BB000004D", "5", "112/00223348-2", "34195728700000405061120022334821234123456000", "34191.12002 22334.821232 41234.560005 5 72870000040506", 2017, 09, 19)]
        [TestCase(506.08, "223349", "BB000005E", "6", "112/00223349-0", "34196731700000506081120022334901234123456000", "34191.12002 22334.901232 41234.560005 6 73170000050608", 2017, 10, 19)]
        [TestCase(307.15, "223347", "BB000003C", "7", "112/00223347-4", "34197725600000307151120022334741234123456000", "34191.12002 22334.741232 41234.560005 7 72560000030715", 2017, 08, 19)]
        [TestCase(608.12, "223350", "BB000001F", "8", "112/00223350-8", "34198719500000608121120022335081234123456000", "34191.12002 22335.081232 41234.560005 8 71950000060812", 2017, 06, 19)]
        [TestCase(609.14, "223352", "BB000001F", "9", "112/00223352-4", "34199719500000609141120022335241234123456000", "34191.12002 22335.241232 41234.560005 9 71950000060914", 2017, 06, 19)]


        public void Deve_criar_boleto_itau_112_com_digito_verificador_valida(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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

        [TestCase(101.01, "223345", "BB000001A", "1", "112/00223345-8", "34191719500000101011120022334581234123456000", "34191.12002 22334.581232 41234.560005 1 71950000010101", 2017, 06, 19)]
        [TestCase(202.06, "223346", "BB000002B", "2", "112/00223346-6", "34192722500000202061120022334661234123456000", "34191.12002 22334.661232 41234.560005 2 72250000020206", 2017, 07, 19)]
        [TestCase(903.47, "223353", "BB000001I", "3", "112/00223353-2", "34193719500000903471120022335321234123456000", "34191.12002 22335.321232 41234.560005 3 71950000090347", 2017, 06, 19)]
        [TestCase(704.16, "223351", "BB000001G", "4", "112/00223351-6", "34194719500000704161120022335161234123456000", "34191.12002 22335.161232 41234.560005 4 71950000070416", 2017, 06, 19)]
        [TestCase(405.06, "223348", "BB000004D", "5", "112/00223348-2", "34195728700000405061120022334821234123456000", "34191.12002 22334.821232 41234.560005 5 72870000040506", 2017, 09, 19)]
        [TestCase(506.08, "223349", "BB000005E", "6", "112/00223349-0", "34196731700000506081120022334901234123456000", "34191.12002 22334.901232 41234.560005 6 73170000050608", 2017, 10, 19)]
        [TestCase(307.15, "223347", "BB000003C", "7", "112/00223347-4", "34197725600000307151120022334741234123456000", "34191.12002 22334.741232 41234.560005 7 72560000030715", 2017, 08, 19)]
        [TestCase(608.12, "223350", "BB000001F", "8", "112/00223350-8", "34198719500000608121120022335081234123456000", "34191.12002 22335.081232 41234.560005 8 71950000060812", 2017, 06, 19)]
        [TestCase(609.14, "223352", "BB000001F", "9", "112/00223352-4", "34199719500000609141120022335241234123456000", "34191.12002 22335.241232 41234.560005 9 71950000060914", 2017, 06, 19)]


        public void Deve_criar_boleto_itau_112_com_nosso_numero_formatado_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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

        [TestCase(101.01, "223345", "BB000001A", "1", "112/00223345-8", "34191719500000101011120022334581234123456000", "34191.12002 22334.581232 41234.560005 1 71950000010101", 2017, 06, 19)]
        [TestCase(202.06, "223346", "BB000002B", "2", "112/00223346-6", "34192722500000202061120022334661234123456000", "34191.12002 22334.661232 41234.560005 2 72250000020206", 2017, 07, 19)]
        [TestCase(903.47, "223353", "BB000001I", "3", "112/00223353-2", "34193719500000903471120022335321234123456000", "34191.12002 22335.321232 41234.560005 3 71950000090347", 2017, 06, 19)]
        [TestCase(704.16, "223351", "BB000001G", "4", "112/00223351-6", "34194719500000704161120022335161234123456000", "34191.12002 22335.161232 41234.560005 4 71950000070416", 2017, 06, 19)]
        [TestCase(405.06, "223348", "BB000004D", "5", "112/00223348-2", "34195728700000405061120022334821234123456000", "34191.12002 22334.821232 41234.560005 5 72870000040506", 2017, 09, 19)]
        [TestCase(506.08, "223349", "BB000005E", "6", "112/00223349-0", "34196731700000506081120022334901234123456000", "34191.12002 22334.901232 41234.560005 6 73170000050608", 2017, 10, 19)]
        [TestCase(307.15, "223347", "BB000003C", "7", "112/00223347-4", "34197725600000307151120022334741234123456000", "34191.12002 22334.741232 41234.560005 7 72560000030715", 2017, 08, 19)]
        [TestCase(608.12, "223350", "BB000001F", "8", "112/00223350-8", "34198719500000608121120022335081234123456000", "34191.12002 22335.081232 41234.560005 8 71950000060812", 2017, 06, 19)]
        [TestCase(609.14, "223352", "BB000001F", "9", "112/00223352-4", "34199719500000609141120022335241234123456000", "34191.12002 22335.241232 41234.560005 9 71950000060914", 2017, 06, 19)]


        public void Deve_criar_boleto_itau_112_com_codigo_de_barras_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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

        [TestCase(101.01, "223345", "BB000001A", "1", "112/00223345-8", "34191719500000101011120022334581234123456000", "34191.12002 22334.581232 41234.560005 1 71950000010101", 2017, 06, 19)]
        [TestCase(202.06, "223346", "BB000002B", "2", "112/00223346-6", "34192722500000202061120022334661234123456000", "34191.12002 22334.661232 41234.560005 2 72250000020206", 2017, 07, 19)]
        [TestCase(903.47, "223353", "BB000001I", "3", "112/00223353-2", "34193719500000903471120022335321234123456000", "34191.12002 22335.321232 41234.560005 3 71950000090347", 2017, 06, 19)]
        [TestCase(704.16, "223351", "BB000001G", "4", "112/00223351-6", "34194719500000704161120022335161234123456000", "34191.12002 22335.161232 41234.560005 4 71950000070416", 2017, 06, 19)]
        [TestCase(405.06, "223348", "BB000004D", "5", "112/00223348-2", "34195728700000405061120022334821234123456000", "34191.12002 22334.821232 41234.560005 5 72870000040506", 2017, 09, 19)]
        [TestCase(506.08, "223349", "BB000005E", "6", "112/00223349-0", "34196731700000506081120022334901234123456000", "34191.12002 22334.901232 41234.560005 6 73170000050608", 2017, 10, 19)]
        [TestCase(307.15, "223347", "BB000003C", "7", "112/00223347-4", "34197725600000307151120022334741234123456000", "34191.12002 22334.741232 41234.560005 7 72560000030715", 2017, 08, 19)]
        [TestCase(608.12, "223350", "BB000001F", "8", "112/00223350-8", "34198719500000608121120022335081234123456000", "34191.12002 22335.081232 41234.560005 8 71950000060812", 2017, 06, 19)]
        [TestCase(609.14, "223352", "BB000001F", "9", "112/00223352-4", "34199719500000609141120022335241234123456000", "34191.12002 22335.241232 41234.560005 9 71950000060914", 2017, 06, 19)]


        public void Deve_criar_boleto_itau_112_com_linha_digitavel_valida(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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

            Assert.That(boleto.CodigoBarra.LinhaDigitavel, Is.EqualTo(linhaDigitavel), "Linha digitável inválida");
        }
    }
}