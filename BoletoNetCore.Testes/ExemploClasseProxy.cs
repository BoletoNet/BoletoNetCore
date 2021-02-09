using System;
using System.IO;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Outros Testes")]
    public class BoletoNetCoreExemploClasseProxy
    {
        [Test]
        public void BoletoNetCore_ExemploProxy()
        {
            var mensagemErro = "";
            var retorno = false;
            var classeProxy = new BoletoNetCoreProxy();

            // Define os dados do Beneficiário, Conta Bancária e Carteira de Cobrança
            retorno = classeProxy.SetupCobranca("12.123.123/1234-46", "Beneficiario Teste Classe Proxy",
                                                  "Av Testador", "12", "sala 30", "Centro", "Cidade", "SP", "11223-445", "Observacoes do Beneficiário",
                                                   237, "1234", "X", "", "123456", "X",
                                                   "1213141", "0", "", "09", "",
                                                   (int)TipoCarteira.CarteiraCobrancaSimples, (int)TipoFormaCadastramento.ComRegistro, (int)TipoImpressaoBoleto.Empresa, (int)TipoDocumento.Escritural,
                                                   ref mensagemErro);
            Assert.AreEqual(true, retorno, "SetupCobrança: " +mensagemErro);

            // Cria um novo Boleto Bancario na coleção de Boletos Bancarios
            retorno = classeProxy.NovoBoleto(ref mensagemErro);
            Assert.AreEqual(true, retorno, "NovoBoleto: " + mensagemErro);

            // Define os dados do Pagador
            retorno = classeProxy.DefinirPagador("123.456.789-09", "Pagador Teste Classe Proxy", "Rua Testando", "456", "casa 123", "Vila Central", "Cidade", "SP", "56789012", "Observação do Pagador", ref mensagemErro);
            Assert.AreEqual(true, retorno, "DefinirPagador: " + mensagemErro);

            // Define os dados do Boleto
            retorno = classeProxy.DefinirBoleto("DM", "DP123456AZ", "445566", DateTime.Now.AddDays(-3), DateTime.Now, DateTime.Now.AddDays(+30), (decimal)123456.78, "CHAVEPRIMARIABANCO=12345!", "N", ref mensagemErro);
            Assert.AreEqual(true, retorno, "DefinirBoleto: " + mensagemErro);

            // Define multa (2%)
            retorno = classeProxy.DefinirMulta(classeProxy.boleto.DataVencimento, classeProxy.boleto.ValorTitulo * (decimal)0.02, (decimal)0.02, ref mensagemErro);
            Assert.AreEqual(true, retorno, "DefinirMulta: " + mensagemErro);

            // Define juros (6% ao mês)
            retorno = classeProxy.DefinirJuros(classeProxy.boleto.DataVencimento, classeProxy.boleto.ValorTitulo * (decimal)(0.06/30), (decimal)(0.06 / 30), ref mensagemErro);
            Assert.AreEqual(true, retorno, "DefinirJuros: " + mensagemErro);

            // Define desconto (5 dias antes do vencimento, R$ 10 de desconto)
            retorno = classeProxy.DefinirDesconto(classeProxy.boleto.DataVencimento.AddDays(-5), (decimal)10.00, ref mensagemErro);
            Assert.AreEqual(true, retorno, "DefinirDesconto: " + mensagemErro);

            // Define instruções de cobrança para o arquivo remessa e para ser impresso no boleto
            retorno = classeProxy.DefinirInstrucoes("Mensagem para ser impressa no boleto","Mensagem para o arquivo remessa", "01", "02", "03", "04", "05", "06", true, ref mensagemErro);
            Assert.AreEqual(true, retorno, "DefinirInstrucoes: " + mensagemErro);

            // Fecha o boleto atual, valida os dados, etc.
            retorno = classeProxy.FecharBoleto(ref mensagemErro);
            Assert.AreEqual(true, retorno, "FecharBoleto: " + mensagemErro);

            // Repita os métodos acima para adicionar novos boletos, quantos necessários: NovoBoleto, DefinirPagador, DefinirBoleto, FecharBoleto, etc.
            // Após preencher a coleção com os boletos, siga com os exemplos abaixo...

            // Verifica se existe a pasta temporaria para receber os arquivos do teste:
            var nomePasta = Path.GetTempPath() + "BoletoNetCore\\";
            if (Directory.Exists(nomePasta) == false)
                Directory.CreateDirectory(nomePasta);

            // Para gerar o arquivo remessa, após preencher a coleção de Boletos:
            classeProxy.GerarRemessa((int)TipoArquivo.CNAB400, nomePasta + @"ClasseProxy_Cnab400.Rem", 1, ref mensagemErro);
            Assert.AreEqual(true, retorno, "GerarRemessa: " + mensagemErro);

            // Para gerar o arquivo PDF
            classeProxy.GerarBoletos(nomePasta + @"ClasseProxy_Boleto.Pdf", ref mensagemErro);
            Assert.AreEqual(true, retorno, "GerarBoletos: " + mensagemErro);

        }

    }

}
