using System;
using System.Net;
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

        #region Chaves de Acesso Api

        // Chave Master que deve ser gerada pelo portal do sicredi
        // Menu Cobrança, Sub Menu Lateral Código de Acesso / Gerar
        public string ChaveApi { get; set; }

        // Chave de Transação valida por 24 horas
        // Segundo o manual, não é permitido gerar uma nova chave de transação antes da atual estar expirada.
        // Caso seja necessário gerar uma chave de transação antes, é necessário criar uma nova chave master, o que invalida a anterior.
        public string Token { get; set; }

        #endregion

        public async Task<string> GerarToken()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "autenticacao");
            request.Headers.Add("token", this.ChaveApi);

            var response = await this.httpClient.SendAsync(request);
            await this.CheckHttpResponseError(response);
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
            emissao.NossoNumero = boleto.NossoNumero+boleto.NossoNumeroDV;
            emissao.TipoPessoa = boleto.Pagador.TipoCPFCNPJ("0");
            emissao.CpfCnpj = boleto.Pagador.CPFCNPJ;
            emissao.Nome = boleto.Pagador.Nome;
            emissao.Endereco = boleto.Pagador.Endereco.FormataLogradouro(0);
            emissao.Cidade = boleto.Pagador.Endereco.Cidade;
            emissao.Uf = boleto.Pagador.Endereco.UF;
            emissao.Cep = boleto.Pagador.Endereco.CEP;

            // todo
            emissao.CodigoPagador = string.Empty;

            // manual: "Opcional. Será obrigatório se o código do pagador não for informado"
            emissao.Telefone = boleto.Pagador.Telefone;

            emissao.Email = "";
            emissao.EspecieDocumento = this.AjustaEspecieCnab400(boleto.EspecieDocumento);
            emissao.SeuNumero = boleto.NumeroDocumento;
            emissao.DataVencimento = boleto.DataVencimento.ToString("dd/MM/yyyy");
            emissao.Valor = boleto.ValorTitulo;
            emissao.TipoDesconto = "A"; // todo: 

            if (boleto.ValorDesconto != 0)
            {
                emissao.ValorDesconto1 = boleto.ValorDesconto;
                emissao.DataDesconto1 = boleto.DataDesconto.ToString("dd/MM/yyyy");
            }

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
            var response = await this.httpClient.SendAsync(request);
            await this.CheckHttpResponseError(response);

            // todo: verificar a necessidade de preencher dados do boleto com o retorno do sicredi
            var boletoEmitido = await response.Content.ReadFromJsonAsync<BoletoEmitidoSicrediApi>();
            boletoEmitido.LinhaDigitável.ToString();
            boletoEmitido.CodigoBarra.ToString();
        }

        private async Task CheckHttpResponseError(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return;

            if (response.StatusCode == HttpStatusCode.BadRequest || (response.StatusCode == HttpStatusCode.NotFound && response.Content.Headers.ContentType.MediaType == "application/json"))
            {
                var bad = await response.Content.ReadFromJsonAsync<BadRequestSicrediApi>();
                throw new Exception(string.Format("{0} {1}", bad.Parametro, bad.Mensagem).Trim());
            }
            else
                throw new Exception(string.Format("Erro desconhecido: {0}", response.StatusCode));
        }

        public async Task ConsultarStatus(Boleto boleto)
        {
            var agencia = boleto.Banco.Beneficiario.ContaBancaria.Agencia;
            var posto = boleto.Banco.Beneficiario.ContaBancaria.DigitoAgencia;
            var cedente = boleto.Banco.Beneficiario.Codigo;
            var nossoNumero = boleto.NossoNumero+boleto.NossoNumeroDV;

            // existem outros parametros no manual para consulta de multiplos boletos
            var url = string.Format("consulta?agencia={0}&cedente={1}&posto={2}&nossoNumero={3}", agencia, cedente, posto, nossoNumero);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("token", this.Token);
            var response = await this.httpClient.SendAsync(request);
            await this.CheckHttpResponseError(response);
            var ret = await response.Content.ReadFromJsonAsync<RetornoConsultaBoletoSicrediApi[]>();

            // todo: verificar quais dados necessarios para preencher boleto
            ret[0].Situacao.ToString();
        }
    }

    #region Classes Auxiliares (json) Sicredi

    class InstrucaoSicrediApi
    {
        public string Agencia { get; set; }
        public string Posto { get; set; }
        public string Cedente { get; set; }
        public string NossoNumero { get; set; }

        /*
            PEDIDO_BAIXA
            CONCESSAO_ABATIMENTO
            CANCELAMENTO_ABATIMENTO_CONCEDIDO
            ALTERACAO_VENCIMENTO
            ALTERACAO_SEU_NUMERO
            PEDIDO_PROTESTO
            SUSTAR_PROTESTO_BAIXAR _TITULO
            SUSTAR_PROTESTO_MANTER_CARTEIRA
            ALTERACAO_OUTROS_DADOS
        */
        public string InstrucaoComando { get; set; }

        /*
            DESCONTO
            JUROS_DIA
            DESCONTO_DIA_ANTECIPACAO
            DATA_LIMITE_CONCESSAO_DESCONTO
            CANCELAMENTO_PROTESTO _AUTOMATICO
            CANCELAMENTO_NEGATIVACAO_AUTOMATICA
        */
        public string ComplementoInstrucao { get; set; }
    }

    class BadRequestSicrediApi
    {
        public string Codigo { get; set; }
        public string Mensagem { get; set; }
        public string Parametro { get; set; }
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

    #endregion
}