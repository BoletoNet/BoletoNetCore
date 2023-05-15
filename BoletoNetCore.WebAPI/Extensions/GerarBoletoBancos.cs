using BoletoNetCore.WebAPI.Models;
using System.Net.NetworkInformation;

namespace BoletoNetCore.WebAPI.Extensions
{
    public class GerarBoletoBancos
    {
        readonly IBanco _banco;

        public GerarBoletoBancos(IBanco banco)
        {
            _banco = banco;
        }

        public string RetornarHtmlBoleto(DadosBoleto dadosBoleto)
        {
            // 1º Beneficiarios = Quem recebe o pagamento
            Beneficiario beneficiario = new Beneficiario()
            {
                CPFCNPJ = dadosBoleto.BeneficiarioResponse.CPFCNPJ,
                Nome = dadosBoleto.BeneficiarioResponse.Nome,
                ContaBancaria = new ContaBancaria()
                {
                    Agencia = dadosBoleto.BeneficiarioResponse.ContaBancariaResponse.Agencia,
                    Conta = dadosBoleto.BeneficiarioResponse.ContaBancariaResponse.Conta,
                    CarteiraPadrao = dadosBoleto.BeneficiarioResponse.ContaBancariaResponse.CarteiraPadrao,
                    TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                    TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                    TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
                }
            };

            _banco.Beneficiario = beneficiario;
            _banco.FormataBeneficiario();

            var boleto = GerarBoleto(_banco, dadosBoleto);
            var boletoBancario = new BoletoBancario();
            boletoBancario.Boleto = boleto;

            return boletoBancario.MontaHtmlEmbedded();
        }

        public static Boleto GerarBoleto(IBanco iBanco, DadosBoleto dadosBoleto)
        {
            try
            {
                var boleto = new Boleto(iBanco)
                {
                    Pagador = GerarPagador(dadosBoleto),
                    DataEmissao = dadosBoleto.DataEmissao,
                    DataProcessamento = dadosBoleto.DataProcessamento,
                    DataVencimento = dadosBoleto.DataVencimento,
                    ValorTitulo = dadosBoleto.ValorTitulo,
                    NossoNumero = dadosBoleto.NossoNumero,
                    NumeroDocumento = dadosBoleto.NumeroDocumento,
                    EspecieDocumento = TipoEspecieDocumento.DS,
                    ImprimirValoresAuxiliares = true,
                };

                //  Para teste não é preciso validar os dados, pois com dados falso nunca vai gerar um boleto que dê para pagar
                //boleto.ValidarDados();
                return boleto;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static Pagador GerarPagador(DadosBoleto dadosBoleto)
        {
            return new Pagador
            {
                Nome = dadosBoleto.PagadorResponse.Nome,
                CPFCNPJ = dadosBoleto.PagadorResponse.CPFCNPJ,
                Observacoes = dadosBoleto.PagadorResponse.Observacoes,
                Endereco = new Endereco
                {
                    LogradouroEndereco = dadosBoleto.PagadorResponse.EnderecoResponse.Logradouro,
                    LogradouroNumero = dadosBoleto.PagadorResponse.EnderecoResponse.Numero,
                    Bairro = dadosBoleto.PagadorResponse.EnderecoResponse.Bairro,
                    Cidade = dadosBoleto.PagadorResponse.EnderecoResponse.Cidade,
                    UF = dadosBoleto.PagadorResponse.EnderecoResponse.Estado,
                    CEP = dadosBoleto.PagadorResponse.EnderecoResponse.CEP,
                }
            };
        }
    }
}
