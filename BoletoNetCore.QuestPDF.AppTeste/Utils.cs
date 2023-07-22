using System;
using System.IO;
using System.Text;

namespace BoletoNetCore.QuestPDF.AppTeste
{
    internal sealed class Utils
    {
        private static int _contador = 1;

        private static int _proximoNossoNumero = 1;

        internal static Beneficiario GerarBeneficiario(string codigoBeneficiario, string digitoCodigoBeneficiario, string codigoTransmissao, ContaBancaria contaBancaria)
        {
            return new Beneficiario
            {
                CPFCNPJ = "86.875.666/0001-09",
                Nome = "Beneficiario Teste",
                Codigo = codigoBeneficiario,
                CodigoDV = digitoCodigoBeneficiario,
                CodigoTransmissao = codigoTransmissao,
                Endereco = new Endereco
                {
                    LogradouroEndereco = "Rua Teste do Beneficiário",
                    LogradouroNumero = "789",
                    LogradouroComplemento = "Cj 333",
                    Bairro = "Bairro",
                    Cidade = "Cidade",
                    UF = "SP",
                    CEP = "65432987"
                },
                ContaBancaria = contaBancaria
            };
        }

        internal static Pagador GerarPagador()
        {
            if (_contador % 2 == 0)
                return new Pagador
                {
                    CPFCNPJ = "443.316.101-28",
                    Nome = "Pagador Teste PF",
                    Observacoes = "Matricula 678/9",
                    Endereco = new Endereco
                    {
                        LogradouroEndereco = "Rua Testando",
                        LogradouroNumero = "456",
                        Bairro = "Bairro",
                        Cidade = "Cidade",
                        UF = "SP",
                        CEP = "56789012"
                    }
                };
            return new Pagador
            {
                CPFCNPJ = "71.738.978/0001-01",
                Nome = "Pagador Teste PJ",
                Observacoes = "Matricula 123/4",
                Endereco = new Endereco
                {
                    LogradouroEndereco = "Avenida Testando",
                    LogradouroNumero = "123",
                    Bairro = "Bairro",
                    Cidade = "Cidade",
                    UF = "SP",
                    CEP = "12345678"
                }
            };
        }

        internal static Boletos GerarBoletos(IBanco banco, int quantidadeBoletos, string aceite, int NossoNumeroInicial)
        {
            var boletos = new Boletos
            {
                Banco = banco
            };
            for (var i = 1; i <= quantidadeBoletos; i++)
                boletos.Add(GerarBoleto(banco, i, aceite, NossoNumeroInicial));
            return boletos;
        }

        internal static Boleto GerarBoleto(IBanco banco, int i, string aceite, int NossoNumeroInicial)
        {
            if (aceite == "?")
                aceite = _contador % 2 == 0 ? "N" : "A";

            var boleto = new Boleto(banco)
            {
                Pagador = GerarPagador(),
                DataEmissao = DateTime.Now.AddDays(-3),
                DataProcessamento = DateTime.Now,
                DataVencimento = DateTime.Now.AddMonths(i),
                ValorTitulo = (decimal)100 * i,
                NossoNumero = NossoNumeroInicial == 0 ? "" : (NossoNumeroInicial + _proximoNossoNumero).ToString(),
                NumeroDocumento = "BB" + _proximoNossoNumero.ToString("D6") + (char)(64 + i),
                EspecieDocumento = TipoEspecieDocumento.DM,
                Aceite = aceite,
                CodigoInstrucao1 = "11",
                CodigoInstrucao2 = "22",
                DataDesconto = DateTime.Now.AddMonths(i),
                ValorDesconto = (decimal)(100 * i * 0.10),
                DataDesconto2 = DateTime.Now.AddMonths(i).AddDays(2),
                ValorDesconto2 = (decimal)(100 * i * 0.12),
                DataDesconto3 = DateTime.Now.AddMonths(i).AddDays(3),
                ValorDesconto3 = (decimal)(100 * i * 0.13),
                DataMulta = DateTime.Now.AddMonths(i),
                PercentualMulta = (decimal)2.00,
                ValorMulta = (decimal)(100 * i * (2.00 / 100)),
                DataJuros = DateTime.Now.AddMonths(i),
                PercentualJurosDia = (decimal)0.2,
                ValorJurosDia = (decimal)(100 * i * (0.2 / 100)),
                AvisoDebitoAutomaticoContaCorrente = "2",
                MensagemArquivoRemessa = "Mensagem para o arquivo remessa",
                NumeroControleParticipante = "CHAVEPRIMARIA" + _proximoNossoNumero
            };
            // Mensagem - Instruções do Caixa
            boleto.ImprimirValoresAuxiliares = true;

            boleto.ValidarDados();
            _contador++;
            _proximoNossoNumero++;
            return boleto;
        }

    }
}