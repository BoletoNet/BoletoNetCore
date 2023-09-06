using BoletoNetCore.Exceptions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Banco")]
    internal class BancoTests
    {
        private readonly IBanco _banco;
        private readonly Boleto _boleto;
        public BancoTests()
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

            _boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(2016, 9, 2),
                ValorTitulo = 2924.11M,
                NossoNumero = "445",
                NumeroDocumento = "BB874A",
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

        }

        [Test]
        public void Deve_retornar_exception_ao_criar_um_banco_invalido()
            => Assert.Throws<BoletoNetCoreException>(() => Banco.Instancia((Bancos)999));

        [Test]
        public void Deve_retornar_mensagem_na_exception_ao_criar_um_banco_invalido()
        {
            try
            {
                Banco.Instancia((Bancos)999);
            }
            catch (BoletoNetCoreException ex)
            {
                Assert.AreEqual("Banco não implementando: 999", ex.Message);
            }
           
        }

        #region Valida a criação dos bancos válidos
        
        [Test]
        public void Deve_criar_um_banco_santander_valido()
           => Assert.NotNull(Banco.Instancia(Bancos.Santander));

        [Test]
        public void Deve_criar_um_banco_banrisul_valido()
           => Assert.NotNull(Banco.Instancia(Bancos.Banrisul));

        [Test]
        public void Deve_criar_um_banco_sicredi_valido()
           => Assert.NotNull(Banco.Instancia(Bancos.Sicredi));

        [Test]
        public void Deve_criar_um_banco_sicoob_valido()
           => Assert.NotNull(Banco.Instancia(Bancos.Sicoob));

        [Test]
        public void Deve_criar_um_banco_safra_valido()
           => Assert.NotNull(Banco.Instancia(Bancos.Safra));

        [Test]
        public void Deve_criar_um_banco_itau_valido()
           => Assert.NotNull(Banco.Instancia(Bancos.Itau));

        [Test]
        public void Deve_criar_um_banco_do_brasil_valido()
           => Assert.NotNull(Banco.Instancia(Bancos.BancoDoBrasil));

        [Test]
        public void Deve_criar_um_banco_do_nordeste_valido()
           => Assert.NotNull(Banco.Instancia(Bancos.BancoDoNordeste));

        [Test]
        public void Deve_criar_um_banco_bradesco_valido()
           => Assert.NotNull(Banco.Instancia(Bancos.Bradesco));

        [Test]
        public void Deve_criar_um_banco_caixa_valido()
           => Assert.NotNull(Banco.Instancia(Bancos.Caixa));

        [Test]
        public void Deve_criar_um_banco_cecred_valido()
           => Assert.NotNull(Banco.Instancia(Bancos.Cecred));

        [Test]
        public void Deve_criar_um_banco_uniprime_norte_pr_valido()
           => Assert.NotNull(Banco.Instancia(Bancos.UniprimeNortePR));

        [Test]
        public void Deve_criar_um_banco_inter_valido()
           => Assert.NotNull(Banco.Instancia(Bancos.BancoInter));

        #endregion

        [Test]
        public void Deve_formatar_um_codigo_de_barras_valido()
        {

            _boleto.Banco.FormataNossoNumero(_boleto);

            Banco.FormataCodigoBarra(_boleto);

            Assert.AreEqual(_boleto.CodigoBarra.CodigoDeBarras, "03397690500002924119123456700000000044560101");

        }

        [Test]
        public void Deve_formatar_mensagem_instrucao_com_a_mensagem_informada()
        { 
            _boleto.MensagemInstrucoesCaixa = "Teste";
            _boleto.ImprimirMensagemInstrucao = true;

            Banco.FormataMensagemInstrucao(_boleto);

            Assert.True(_boleto.MensagemInstrucoesCaixaFormatado.Contains("Teste")); 
        }


        [Test]
        public void Deve_formatar_linha_digitavel()
        {
            _boleto.MensagemInstrucoesCaixa = "Teste";
            _boleto.ImprimirMensagemInstrucao = true;

            _boleto.Banco.ValidaBoleto(_boleto);
            _boleto.Banco.FormataNossoNumero(_boleto);
            BoletoNetCore.Banco.FormataCodigoBarra(_boleto);

            Banco.FormataLinhaDigitavel(_boleto);

            Assert.AreEqual(_boleto.CodigoBarra.LinhaDigitavel, "03399.12347 56700.000005 00445.601016 7 69050000292411");
        }

    }
}
