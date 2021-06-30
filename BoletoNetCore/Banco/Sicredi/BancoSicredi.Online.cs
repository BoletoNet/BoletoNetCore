using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BoletoNetCore
{
    partial class BancoSicredi : IBancoOnlineRest
    {
        #region HttpClient
        private HttpClient _httpClient;
        private HttpClient httpClient
        {
            get
            {
                if (this._httpClient == null)
                {
                    this._httpClient = new HttpClient();
                    this._httpClient.BaseAddress = new Uri("https://cobrancaonline.sicredi.com.br/sicredi-cobranca-ws-ecomm-api/ecomm/v1/boleto/");
                }

                return this._httpClient;
            }
        }
        #endregion

        public string ChaveMaster { get; set; }
        public string Token { get; set; }

        public async Task<string> GerarToken()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "autenticacao");
            request.Headers.Add("token", this.ChaveMaster);

            var response = await this.httpClient.SendAsync(request);
            var ret = await response.Content.ReadFromJsonAsync<ChaveTransacaoSicrediApi>();
            this.Token = ret.ChaveTransacao;
            return ret.ChaveTransacao;
        }

        public async Task RegistrarBoleto(Boleto boleto)
        {
            var emissao = new EmissaoBoletoSicrediApi();
            emissao.Agencia = boleto.Banco.Beneficiario.ContaBancaria.Agencia;
            emissao.Posto = boleto.Banco.Beneficiario.ContaBancaria.DigitoAgencia;
            emissao.Cedente = boleto.Banco.Beneficiario.Codigo;
            emissao.NossoNumero = boleto.NossoNumeroFormatado;
            emissao.TipoPessoa = boleto.Pagador.TipoCPFCNPJ("0");
            emissao.CpfCnpj = boleto.Pagador.CPFCNPJ;
            emissao.Nome = boleto.Pagador.Nome;
            emissao.Endereco = boleto.Pagador.Endereco.FormataLogradouro(0);
            emissao.Cidade = boleto.Pagador.Endereco.Cidade;
            emissao.Uf = boleto.Pagador.Endereco.UF;
            emissao.Cep = boleto.Pagador.Endereco.CEP;
            emissao.Telefone = ""; // todo: sicredi exige o telefone
            emissao.Email = "";
            emissao.EspecieDocumento = "A"; // todo: A - Duplicata Mercantil
            emissao.SeuNumero = boleto.NumeroDocumento;
            emissao.DataVencimento = boleto.DataVencimento.ToString("dd/MM/yyyy");
            emissao.Valor = boleto.ValorTitulo;
            emissao.TipoDesconto = "A"; // todo: 
            emissao.ValorDesconto1 = boleto.ValorDesconto;
            emissao.DataDesconto1 = boleto.DataDesconto.ToString("dd/MM/yyyy");
            emissao.TipoJuros = "A"; // todo
            emissao.Juros = boleto.ValorJurosDia;
            emissao.Multas = boleto.ValorMulta;
            emissao.DescontoAntecipado = 0; // todo
            emissao.Informativo = ""; // todo
            emissao.Mensagem = boleto.MensagemInstrucoesCaixaFormatado;
            emissao.NumDiasNegativacaoAuto = boleto.DiasProtesto;

            var request = new HttpRequestMessage(HttpMethod.Post, "emissao");
            request.Headers.Add("token", this.Token);
            request.Content = JsonContent.Create(emissao);
            //var emitido = await this.httpClient.SendAsync(request);
            await this.httpClient.SendAsync(request);
        }
    }

    class ChaveTransacaoSicrediApi
    {
        public string ChaveTransacao { get; set; }
        public DateTime dataExpiracao { get; set; }
    }

    class RetornoConsultaBoletoSicrediApi
    {
        public string SeuNumero { get; set; }
        public string NossoNumero { get; set; }
        public string NomePagador { get; set; }
        public string Valor { get; set; }
        public string ValorLiquidado { get; set; }
        public string DataEmissao { get; set; }
        public string DataVencimento { get; set; }
        public string DataLiquidacao { get; set; }
        public string Situacao { get; set; }
    }

    class BoletoEmitidoSicrediApi
    {
        public string LinhaDigitável { get; set; }
        public string CodigoBanco { get; set; }
        public string NomeBeneficiario { get; set; }
        public string EnderecoBeneficiario { get; set; }
        public string CpfCnpjBeneficiario { get; set; }
        public string CooperativaBeneficiario { get; set; }
        public string PostoBeneficiario { get; set; }
        public string CodigoBeneficiario { get; set; }
        public DateTime DataDocumento { get; set; }
        public string SeuNumero { get; set; }
        public string EspecieDocumento { get; set; }
        public string Aceite { get; set; }
        public DateTime DataProcessamento { get; set; }
        public string NossoNumero { get; set; }
        public string Especie { get; set; }
        public decimal ValorDocumento { get; set; }
        public DateTime DataVencimento { get; set; }
        public string NomePagador { get; set; }
        public string CpfCnpjPagador { get; set; }
        public string EnderecoPagador { get; set; }
        public DateTime DataLimiteDesconto { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal JurosMulta { get; set; }
        public string Instrucao { get; set; }
        public string Informativo { get; set; }
        public string CodigoBarra { get; set; }
    }

    class EmissaoBoletoSicrediApi
    {
        public string Agencia { get; set; }
        public string Posto { get; set; }
        public string Cedente { get; set; }
        public string NossoNumero { get; set; }
        public string CodigoPagador { get; set; }
        /// <summary>
        /// 1 fisica - 2 juridica
        /// </summary>
        public string TipoPessoa { get; set; }
        public string CpfCnpj { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public string Cep { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string EspecieDocumento { get; set; }
        public string CodigoSacadorAvalista { get; set; }
        public string SeuNumero { get; set; }
        public string DataVencimento { get; set; }
        public decimal Valor { get; set; }
        /// <summary>
        /// A valor / B percentual
        /// </summary>
        public string TipoDesconto { get; set; }
        public decimal ValorDesconto1 { get; set; }
        public string DataDesconto1 { get; set; }
        public decimal ValorDesconto2 { get; set; }
        public string DataDesconto2 { get; set; }
        public decimal ValorDesconto3 { get; set; }
        public string DataDesconto3 { get; set; }
        /// <summary>
        /// A valor / B percentual
        /// </summary>
        public string TipoJuros { get; set; }
        public decimal Juros { get; set; }
        public decimal Multas { get; set; }
        public decimal DescontoAntecipado { get; set; }
        public string Informativo { get; set; }
        public string Mensagem { get; set; }
        public string CodigoMensagem { get; set; }
        public int NumDiasNegativacaoAuto { get; set; }
    }
}