using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CpfCnpjComBr.BoletoNetCore
{
    /// <summary>
    /// Implementacao de <see cref="IPessoaLookup"/> baseada na API cpfcnpj.com.br.
    /// Usa os pacotes 3 (CPF) e 5 (CNPJ) por padrao. Nao adiciona estado mutavel;
    /// pode ser reutilizada e compartilhada entre threads com o mesmo <see cref="HttpClient"/>.
    ///
    /// Diferenciais da fonte: consulta em tempo real (D+0) direto nas bases oficiais,
    /// sem uso de bases vazadas, sob gestao certificada ISO/IEC 27001, ISO/IEC 27701
    /// e ISO 37301. Documentacao: https://www.cpfcnpj.com.br/dev/
    /// </summary>
    public sealed class CpfCnpjComBrLookup : IPessoaLookup, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly CpfCnpjComBrOptions _options;
        private readonly bool _httpClientProprio;

        /// <summary>
        /// Cria o cliente com um <see cref="HttpClient"/> injetado (recomendado via IHttpClientFactory).
        /// O <see cref="HttpClient"/> injetado nao e descartado por esta classe.
        /// </summary>
        public CpfCnpjComBrLookup(HttpClient httpClient, CpfCnpjComBrOptions options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _options.Validar();
        }

        /// <summary>
        /// Cria o cliente apenas com o token, usando um <see cref="HttpClient"/> proprio.
        /// Como esse HttpClient e criado internamente, reutilize a mesma instancia (ela e
        /// segura para varias consultas) e chame <see cref="Dispose"/> ao final; nao crie
        /// uma instancia por requisicao para evitar esgotamento de sockets.
        /// </summary>
        public CpfCnpjComBrLookup(string token)
            : this(new HttpClient(), new CpfCnpjComBrOptions { Token = token })
        {
            _httpClientProprio = true;
        }

        public async Task<PessoaResultado> ConsultarAsync(string documento, CancellationToken cancellationToken = default)
        {
            var limpo = Documento.NormalizarEValidar(documento);
            var ehCpf = Documento.EhCpf(limpo);
            var pacote = ehCpf ? _options.PacoteCpf : _options.PacoteCnpj;

            var url = MontarUrl(pacote, limpo);

            string corpo;
            HttpStatusCode statusHttp;
            try
            {
                using (var resposta = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false))
                {
                    statusHttp = resposta.StatusCode;
#if NET5_0_OR_GREATER
                    corpo = await resposta.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
                    corpo = await resposta.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif
                }
            }
            catch (HttpRequestException ex)
            {
                throw new CpfCnpjComBrException("Falha ao consultar a API cpfcnpj.com.br.", ex);
            }
            catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                // Cancelamento sem pedido do chamador significa estouro do timeout do HttpClient.
                throw new CpfCnpjComBrException("Tempo limite excedido ao consultar a API cpfcnpj.com.br.", ex);
            }

            return Interpretar(corpo, statusHttp, limpo, ehCpf);
        }

        public void Dispose()
        {
            if (_httpClientProprio)
                _httpClient.Dispose();
        }

        private string MontarUrl(int pacote, string documento)
        {
            var baseUrl = _options.BaseUrl.TrimEnd('/');
            var token = Uri.EscapeDataString(_options.Token);
            var doc = Uri.EscapeDataString(documento);
            return string.Concat(baseUrl, "/", token, "/",
                pacote.ToString(CultureInfo.InvariantCulture), "/", doc);
        }

        private static PessoaResultado Interpretar(string corpo, HttpStatusCode statusHttp, string documento, bool ehCpf)
        {
            JsonDocument json;
            try
            {
                json = JsonDocument.Parse(corpo);
            }
            catch (JsonException ex)
            {
                // Corpo nao-JSON com HTTP de erro (401/403/5xx) costuma indicar
                // token invalido, limite atingido ou indisponibilidade: exponha o status.
                var codigo = (int)statusHttp;
                var mensagem = statusHttp == HttpStatusCode.OK
                    ? "Resposta da API cpfcnpj.com.br nao e um JSON valido."
                    : $"API cpfcnpj.com.br respondeu HTTP {codigo} sem um corpo JSON valido.";
                throw new CpfCnpjComBrException(mensagem, ex);
            }

            using (json)
            {
                var raiz = json.RootElement;

                if (LerInt(raiz, "status") != 1)
                {
                    var mensagem = LerString(raiz, "erro");
                    if (string.IsNullOrEmpty(mensagem))
                        mensagem = "Consulta nao retornou dados.";
                    throw new CpfCnpjComBrException(mensagem, LerIntNulavel(raiz, "erroCodigo"));
                }

                return ehCpf
                    ? InterpretarCpf(raiz, documento)
                    : InterpretarCnpj(raiz, documento);
            }
        }

        private static PessoaResultado InterpretarCpf(JsonElement raiz, string documento)
        {
            return new PessoaResultado
            {
                Documento = documento,
                EhPessoaFisica = true,
                Nome = LerString(raiz, "nome"),
                Logradouro = LerString(raiz, "endereco"),
                Numero = LerString(raiz, "numero"),
                Complemento = LerString(raiz, "complemento"),
                Bairro = LerString(raiz, "bairro"),
                Cidade = LerString(raiz, "cidade"),
                Uf = LerString(raiz, "uf"),
                Cep = SomenteDigitos(LerString(raiz, "cep")),
            };
        }

        private static PessoaResultado InterpretarCnpj(JsonElement raiz, string documento)
        {
            var endereco = raiz.TryGetProperty("matrizEndereco", out var e) && e.ValueKind == JsonValueKind.Object
                ? e
                : default;

            return new PessoaResultado
            {
                Documento = documento,
                EhPessoaFisica = false,
                Nome = LerString(raiz, "razao"),
                Logradouro = MontarLogradouro(LerString(endereco, "tipo"), LerString(endereco, "logradouro")),
                Numero = LerString(endereco, "numero"),
                Complemento = LerString(endereco, "complemento"),
                Bairro = LerString(endereco, "bairro"),
                Cidade = LerString(endereco, "cidade"),
                Uf = LerString(endereco, "uf"),
                Cep = SomenteDigitos(LerString(endereco, "cep")),
            };
        }

        // Abreviacoes comuns de tipo de logradouro mapeadas para sua forma canonica,
        // para comparar "R" e "Rua" (ou "Av" e "Avenida") como equivalentes.
        private static readonly Dictionary<string, string> AbreviacoesLogradouro =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "R", "RUA" }, { "RUA", "RUA" },
                { "AV", "AVENIDA" }, { "AVE", "AVENIDA" }, { "AVENIDA", "AVENIDA" },
                { "TV", "TRAVESSA" }, { "TRAV", "TRAVESSA" }, { "TRAVESSA", "TRAVESSA" },
                { "PC", "PRACA" }, { "PCA", "PRACA" }, { "PRACA", "PRACA" },
                { "AL", "ALAMEDA" }, { "ALAMEDA", "ALAMEDA" },
                { "ROD", "RODOVIA" }, { "RODOVIA", "RODOVIA" },
                { "EST", "ESTRADA" }, { "ESTRADA", "ESTRADA" },
                { "LGO", "LARGO" }, { "LARGO", "LARGO" },
            };

        /// <summary>
        /// Concatena tipo e logradouro respeitando abreviacoes ("R" vs "Rua"): so prefixa
        /// o tipo quando o logradouro ainda nao comeca por ele (comparando a forma canonica,
        /// para nao duplicar o prefixo quando a API ja traz o tipo por extenso).
        /// </summary>
        internal static string MontarLogradouro(string tipo, string logradouro)
        {
            tipo = (tipo ?? string.Empty).Trim();
            logradouro = (logradouro ?? string.Empty).Trim();

            if (tipo.Length == 0)
                return logradouro;
            if (logradouro.Length == 0)
                return tipo;

            var espaco = logradouro.IndexOf(' ');
            var primeiroToken = espaco < 0 ? logradouro : logradouro.Substring(0, espaco);

            if (Canonizar(primeiroToken) == Canonizar(tipo))
                return logradouro;

            return tipo + " " + logradouro;
        }

        private static string Canonizar(string token)
        {
            token = token.TrimEnd('.');
            return AbreviacoesLogradouro.TryGetValue(token, out var canonico)
                ? canonico
                : token.ToUpperInvariant();
        }

        private static string SomenteDigitos(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return string.Empty;
            var sb = new StringBuilder(valor.Length);
            foreach (var c in valor)
                if (c >= '0' && c <= '9')
                    sb.Append(c);
            return sb.ToString();
        }

        private static string LerString(JsonElement objeto, string propriedade)
        {
            if (objeto.ValueKind != JsonValueKind.Object)
                return string.Empty;
            if (objeto.TryGetProperty(propriedade, out var valor) && valor.ValueKind == JsonValueKind.String)
                return valor.GetString() ?? string.Empty;
            return string.Empty;
        }

        private static int LerInt(JsonElement objeto, string propriedade)
        {
            return LerIntNulavel(objeto, propriedade) ?? 0;
        }

        private static int? LerIntNulavel(JsonElement objeto, string propriedade)
        {
            if (objeto.ValueKind != JsonValueKind.Object)
                return null;
            if (!objeto.TryGetProperty(propriedade, out var valor))
                return null;
            if (valor.ValueKind == JsonValueKind.Number && valor.TryGetInt32(out var n))
                return n;
            if (valor.ValueKind == JsonValueKind.String && int.TryParse(valor.GetString(), out var s))
                return s;
            return null;
        }
    }
}
