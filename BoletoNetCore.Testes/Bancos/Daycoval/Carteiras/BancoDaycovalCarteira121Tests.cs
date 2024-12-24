using System;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Banco Daycoval Carteira 121")]
    public class BancoDaycovalCarteira121
    {
        readonly IBanco _banco;
        public BancoDaycovalCarteira121()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "0001",
                DigitoAgencia = "9",
                Conta = "1213165",
                DigitoConta = "7",
                CarteiraPadrao = "121",
                OperacaoConta = "1934436",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.BancoDaycoval);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("9192132", "", "190600919213200", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Daycoval_121_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoDaycovalCarteira121), 3, true, "?", 84580106);
        }
        //                                                                                                                        70790.00118 21194.346306 08458.010868 9 10250000100000
        [TestCase(141.50,  "84580106", "000001A", "8", "00019/121/0084580106-0", "70798993200000141500001121193443600845801060", "70790.00118 21193.443609 08458.010603 8 99320000014150", 2024,12, 16)]
        [TestCase(2711.12, "84580107", "000002A", "2", "00019/121/0084580107-8", "70792996300002711120001121193443600845801078", "70790.00118 21193.443609 08458.010785 2 99630000271112", 2025, 01, 16)]
        [TestCase(645.39,  "84580108", "000001B", "4", "00019/121/0084580108-6", "70794104200000645390001121193443600845801086", "70790.00118 21193.443609 08458.010868 4 10420000064539", 2025, 4, 05)]
        public void Deve_criar_boleto_daycoval_01_com_digito_verificador_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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


        [TestCase(141.50, "84580106", "000001A", "8", "00019/121/0084580106-0", "70798993200000141500001121193443600845801060", "70790.00118 21193.443609 08458.010603 8 99320000014150", 2024, 12, 16)]
        [TestCase(2711.12, "84580107", "000002A", "2", "00019/121/0084580107-8", "70792996300002711120001121193443600845801078", "70790.00118 21193.443609 08458.010785 2 99630000271112", 2025, 01, 16)]
        [TestCase(645.39, "84580108", "000001B", "4", "00019/121/0084580108-6", "70794104200000645390001121193443600845801086", "70790.00118 21193.443609 08458.010868 4 10420000064539", 2025, 4, 05)]
        public void Deve_criar_boleto_daycoval_01_com_nosso_numero_formatado_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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


        [TestCase(141.50, "84580106", "000001A", "8", "00019/121/0084580106-0", "70798993200000141500001121193443600845801060", "70790.00118 21193.443609 08458.010603 8 99320000014150", 2024, 12, 16)]
        [TestCase(2711.12, "84580107", "000002A", "2", "00019/121/0084580107-8", "70792996300002711120001121193443600845801078", "70790.00118 21193.443609 08458.010785 2 99630000271112", 2025, 01, 16)]
        [TestCase(645.39, "84580108", "000001B", "4", "00019/121/0084580108-6", "70794104200000645390001121193443600845801086", "70790.00118 21193.443609 08458.010868 4 10420000064539", 2025, 4, 05)]
        public void Deve_criar_boleto_daycoval_01_com_codigo_de_barras_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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


        [TestCase(141.50, "84580106", "000001A", "8", "00019/121/0084580106-0", "70798993200000141500001121193443600845801060", "70790.00118 21193.443609 08458.010603 8 99320000014150", 2024, 12, 16)]
        [TestCase(2711.12, "84580107", "000002A", "2", "00019/121/0084580107-8", "70792996300002711120001121193443600845801078", "70790.00118 21193.443609 08458.010785 2 99630000271112", 2025, 01, 16)]
        [TestCase(645.39, "84580108", "000001B", "4", "00019/121/0084580108-6", "70794104200000645390001121193443600845801086", "70790.00118 21193.443609 08458.010868 4 10420000064539", 2025, 4, 05)]
        public void Deve_criar_boleto_daycoval_01_com_linha_digitavel_valida(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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