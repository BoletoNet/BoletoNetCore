using System;
using System.Collections.Generic;
//Envio por email
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using SkiaSharp;

namespace BoletoNetCore
{
    using System.Globalization;
    using System.Linq;

    [Serializable()]
    public class BoletoBancario
    {
        #region Variaveis

        string _vLocalLogoBeneficiario = string.Empty;

        private readonly CultureInfo _culture = CultureInfo.GetCultureInfo("pt-BR");

        private bool _formatoCarne;
        private bool _mostrarCodigoCarteira;

        #endregion Variaveis

        #region Propriedades

        /// <summary>
        /// Mostra o código da carteira
        /// </summary>
        public bool MostrarCodigoCarteira
        {
            get => _mostrarCodigoCarteira;
            set => _mostrarCodigoCarteira = value;
        }

        public bool ExibirDemonstrativo { get; set; }

        /// <summary>
        /// Mostra o código da carteira
        /// </summary>
        public bool FormatoCarne
        {
            get => _formatoCarne;
            set => _formatoCarne = value;
        }

        public Boleto Boleto { get; set; }

        public IBanco Banco { get; set; }

        #region Propriedades

        public bool MostrarComprovanteEntregaLivre { get; set; }

        public bool MostrarComprovanteEntrega { get; set; }

        public bool OcultarEnderecoPagador { get; set; }

        public bool OcultarInstrucoes { get; set; }

        public bool OcultarReciboPagador { get; set; }

        public bool GerarArquivoRemessa { get; set; }

        /// <summary> 
        /// Mostra o termo "Contra Apresentação" na data de vencimento do boleto
        /// </summary>
        public bool MostrarContraApresentacaoNaDataVencimento { get; set; }

        public bool MostrarEnderecoBeneficiario { get; set; }

        #endregion Propriedades

        #endregion Propriedades

        #region Override

        private string GetResourceImage(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var str = new BinaryReader(assembly.GetManifestResourceStream(resourcePath)))
            {
                return Convert.ToBase64String(str.ReadBytes((int)str.BaseStream.Length));
            }
        }

        private string GetResourceHypertext(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var str = new StreamReader(assembly.GetManifestResourceStream(resourcePath)))
            {
                return str.ReadToEnd();
            }
        }

        private string GetCodBarraCode(string code)
        {
            return Convert.ToBase64String(new BarCode2of5i(code, 1, 50, code.Length).ToByte());
        }

        #endregion Override

        #region Html

        public string GeraHtmlInstrucoes()
        {
            try
            {
                var html = new StringBuilder();

                var titulo = "Instruções de Impressão";
                var instrucoes = "Imprimir em impressora jato de tinta (ink jet) ou laser em qualidade normal. (Não use modo econômico).<br>Utilize folha A4 (210 x 297 mm) ou Carta (216 x 279 mm) - Corte na linha indicada<br>";

                var htmlInstrucao = GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.Instrucoes.html");
                html.Append(htmlInstrucao); //HTML.instrucoes
                html.Append("<br />");

                return html.ToString()
                    .Replace("@TITULO", titulo)
                    .Replace("@INSTRUCAO", instrucoes);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a execução da transação.", ex);
            }
        }

        private string GeraHtmlPix(string pixStr, int tamanhoImagem = 200)
        {
            var html = GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.Pix.html")
                .Replace("@PIXSTRING", pixStr)
                .Replace("@PIXIMAGEMTAMANHO", tamanhoImagem.ToString());

            return html;
        }

        private string GeraHtmlPixInstrucoes(string pixStr)
        {
            var html = GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.PixInstrucoes.html")
                .Replace("@PIXINSTRUCOESSTRING", pixStr);

            return html;
        }

        private string GeraHtmlCarne(string telefone, string htmlBoleto)
        {
            var html = new StringBuilder();

            html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.Carne.html"));

            return html.ToString()
                .Replace("@TELEFONE", telefone)
                .Replace("#BOLETO#", htmlBoleto);
        }

        public string GeraHtmlReciboPagador()
        {
            try
            {
                var html = new StringBuilder();
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboPagadorParte1.html"));
                html.Append("<br />");
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboPagadorParte2.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboPagadorParte3.html"));
                if (MostrarEnderecoBeneficiario)
                {
                    html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboPagadorParte10.html"));
                }

                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboPagadorParte4.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboPagadorParte5.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboPagadorParte6.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboPagadorParte7.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboPagadorParte8.html"));
                return html.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a execução da transação.", ex);
            }
        }

        public string GeraHtmlReciboBeneficiario()
        {
            try
            {
                var html = new StringBuilder();
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboBeneficiarioParte1.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboBeneficiarioParte2.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboBeneficiarioParte3.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboBeneficiarioParte4.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboBeneficiarioParte5.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboBeneficiarioParte6.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboBeneficiarioParte7.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboBeneficiarioParte8.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboBeneficiarioParte9.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboBeneficiarioParte10.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboBeneficiarioParte11.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboBeneficiarioParte12.html"));
                return html.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro na execução da transação.", ex);
            }
        }

        public string HtmlComprovanteEntrega
        {
            get
            {
                var html = new StringBuilder();

                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ComprovanteEntrega1.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ComprovanteEntrega2.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ComprovanteEntrega3.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ComprovanteEntrega4.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ComprovanteEntrega5.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ComprovanteEntrega6.html"));

                html.Append(MostrarComprovanteEntregaLivre
                    ? GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ComprovanteEntrega71.html")
                    : GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ComprovanteEntrega7.html"));

                html.Append("<br />");
                return html.ToString();
            }
        }

        private string MontaHtml(string urlImagemLogo, string urlImagemBarra, string imagemCodigoBarras, string pixStr = null, int pixTamanhoImagem = 200, bool pixInstrucoes = false)
        {
            var html = new StringBuilder();
            var enderecoBeneficiario = "";
            var enderecoBeneficiarioCompacto = "";

            //Oculta o cabeçalho das instruções do boleto
            if (!OcultarInstrucoes)
                html.Append(GeraHtmlInstrucoes());

            if (!string.IsNullOrWhiteSpace(pixStr))
            {
                html.Append(GeraHtmlPix(pixStr, pixTamanhoImagem));
            }

            if (ExibirDemonstrativo && Boleto.Demonstrativos.Any())
            {
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboBeneficiarioRelatorioValores.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboBeneficiarioParte5.html"));
                html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.CabecalhoTabelaDemonstrativo.html"));

                var grupoDemonstrativo = new StringBuilder();

                foreach (var relatorio in Boleto.Demonstrativos)
                {
                    var first = true;

                    foreach (var item in relatorio.Itens)
                    {
                        grupoDemonstrativo.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.GrupoDemonstrativo.html"));

                        if (first)
                        {
                            grupoDemonstrativo = grupoDemonstrativo.Replace("@DESCRICAOGRUPO", relatorio.Descricao);

                            first = false;
                        }
                        else
                        {
                            grupoDemonstrativo = grupoDemonstrativo.Replace("@DESCRICAOGRUPO", string.Empty);
                        }

                        grupoDemonstrativo = grupoDemonstrativo.Replace("@DESCRICAOITEM", item.Descricao);
                        grupoDemonstrativo = grupoDemonstrativo.Replace("@REFERENCIAITEM", item.Referencia);
                        grupoDemonstrativo = grupoDemonstrativo.Replace("@VALORITEM", item.Valor.ToString("C", _culture));
                    }

                    grupoDemonstrativo.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.TotalDemonstrativo.html"));
                    grupoDemonstrativo = grupoDemonstrativo.Replace("@VALORTOTALGRUPO", relatorio.Itens.Sum(c => c.Valor).ToString("C", _culture));
                }

                html = html.Replace("@ITENSDEMONSTRATIVO", grupoDemonstrativo.ToString());
            }

            if (!FormatoCarne)
            {
                //Mostra o comprovante de entrega
                if (MostrarComprovanteEntrega | MostrarComprovanteEntregaLivre)
                {
                    html.Append(HtmlComprovanteEntrega);
                    //Html da linha pontilhada
                    if (OcultarReciboPagador)
                        html.Append(GetResourceHypertext("BoletoNetCore.BoletoImpressao.Parts.ReciboPagadorParte8.html"));
                }

                //Oculta o recibo do sacabo do boleto
                if (!OcultarReciboPagador)
                {
                    html.Append(GeraHtmlReciboPagador());

                    //Caso mostre o Endereço do Beneficiário
                    if (MostrarEnderecoBeneficiario)
                    {
                        if (Boleto.Banco.Beneficiario.Endereco == null)
                            throw new ArgumentNullException("Endereço do Beneficiário");

                        enderecoBeneficiario = string.Format("{0} - {1} - {2}/{3} - CEP: {4}",
                            Boleto.Banco.Beneficiario.Endereco.FormataLogradouro(0),
                            Boleto.Banco.Beneficiario.Endereco.Bairro,
                            Boleto.Banco.Beneficiario.Endereco.Cidade,
                            Boleto.Banco.Beneficiario.Endereco.UF,
                            Utils.FormataCEP(Boleto.Banco.Beneficiario.Endereco.CEP));
                        enderecoBeneficiarioCompacto = string.Format("{0} - CEP: {1}",
                            Boleto.Banco.Beneficiario.Endereco.FormataLogradouro(25),
                            Utils.FormataCEP(Boleto.Banco.Beneficiario.Endereco.CEP));
                    }
                }
            }

            // Dados do Pagador
            var pagador = Boleto.Pagador.Nome;
            switch (Boleto.Pagador.TipoCPFCNPJ("A"))
            {
                case "F":
                    pagador += string.Format(" - CPF: " + Utils.FormataCPF(Boleto.Pagador.CPFCNPJ));
                    break;
                case "J":
                    pagador += string.Format(" - CNPJ: " + Utils.FormataCNPJ(Boleto.Pagador.CPFCNPJ));
                    break;
            }

            if (Boleto.Pagador.Observacoes != string.Empty)
                pagador += " - " + Boleto.Pagador.Observacoes;

            var enderecoPagador = string.Empty;
            if (!OcultarEnderecoPagador)
            {
                enderecoPagador = Boleto.Pagador.Endereco.FormataLogradouro(0) + "<br />" + $"{Boleto.Pagador.Endereco.Bairro} - {Boleto.Pagador.Endereco.Cidade}/{Boleto.Pagador.Endereco.UF}";
                if (Boleto.Pagador.Endereco.CEP != String.Empty)
                    enderecoPagador += $" - CEP: {Utils.FormataCEP(Boleto.Pagador.Endereco.CEP)}";
            }

            // Dados do Avalista
            var avalista = string.Empty;
            if (Boleto.Avalista.Nome != string.Empty)
            {
                avalista = Boleto.Avalista.Nome;
                switch (Boleto.Avalista.TipoCPFCNPJ("A"))
                {
                    case "F":
                        avalista += string.Format(" - CPF: " + Utils.FormataCPF(Boleto.Avalista.CPFCNPJ));
                        break;
                    case "J":
                        avalista += string.Format(" - CNPJ: " + Utils.FormataCNPJ(Boleto.Avalista.CPFCNPJ));
                        break;
                }

                if (Boleto.Avalista.Observacoes != string.Empty)
                    avalista += " - " + Boleto.Avalista.Observacoes;
            }


            if (!FormatoCarne)
                html.Append(GeraHtmlReciboBeneficiario());
            else
            {
                html.Append(GeraHtmlCarne("", GeraHtmlReciboBeneficiario()));
            }

            var dataVencimento = Boleto.DataVencimento.ToString("dd/MM/yyyy");

            if (MostrarContraApresentacaoNaDataVencimento)
                dataVencimento = "Contra Apresentação";

            if (string.IsNullOrWhiteSpace(_vLocalLogoBeneficiario))
                _vLocalLogoBeneficiario = urlImagemLogo;

            html.Replace("@PIXINSTRUCOES",
                pixInstrucoes && !string.IsNullOrWhiteSpace(pixStr) ? GeraHtmlPixInstrucoes(pixStr) : string.Empty);

            return html
                .Replace("@CODIGOBANCO", Utils.FormatCode(Boleto.Banco.Codigo.ToString(), 3))
                .Replace("@DIGITOBANCO", Boleto.Banco.Digito)
                .Replace("@URLIMAGEMLOGO", urlImagemLogo)
                .Replace("@URLIMGBENEFICIARIO", _vLocalLogoBeneficiario)
                .Replace("@URLIMAGEMBARRA", urlImagemBarra)
                .Replace("@LINHADIGITAVEL", Boleto.CodigoBarra.LinhaDigitavel)
                .Replace("@LOCALPAGAMENTO", Boleto.Banco.Beneficiario.ContaBancaria.LocalPagamento)
                .Replace("@MENSAGEMFIXATOPOBOLETO", Boleto.Banco.Beneficiario.ContaBancaria.MensagemFixaTopoBoleto)
                .Replace("@MENSAGEMFIXAPAGADOR", Boleto.Banco.Beneficiario.ContaBancaria.MensagemFixaPagador)
                .Replace("@DATAVENCIMENTO", dataVencimento)
                .Replace("@BENEFICIARIO_BOLETO", !Boleto.Banco.Beneficiario.MostrarCNPJnoBoleto ? Boleto.Banco.Beneficiario.Nome : $"{Boleto.Banco.Beneficiario.Nome} - {Utils.FormataCNPJ(Boleto.Banco.Beneficiario.CPFCNPJ)}")
                .Replace("@BENEFICIARIO", !Boleto.Banco.Beneficiario.MostrarCNPJnoBoleto ? Boleto.Banco.Beneficiario.Nome : $"{Boleto.Banco.Beneficiario.Nome} - {Utils.FormataCNPJ(Boleto.Banco.Beneficiario.CPFCNPJ)}")
                .Replace("@DATADOCUMENTO", Boleto.DataEmissao.ToString("dd/MM/yyyy"))
                .Replace("@NUMERODOCUMENTO", Boleto.NumeroDocumento)
                .Replace("@ESPECIEDOCUMENTO", Boleto.EspecieDocumento.ToString())
                .Replace("@DATAPROCESSAMENTO", Boleto.DataProcessamento.ToString("dd/MM/yyyy"))
                .Replace("@NOSSONUMERO", Boleto.NossoNumeroFormatado)
                .Replace("@CARTEIRA", Boleto.CarteiraImpressaoBoleto)
                .Replace("@ESPECIE", Boleto.EspecieMoeda)
                .Replace("@QUANTIDADE", Boleto.QuantidadeMoeda == 0 ? "" : Boleto.QuantidadeMoeda.ToString())
                .Replace("@VALORDOCUMENTO", Boleto.ValorMoeda)
                .Replace("@=VALORDOCUMENTO", Boleto.ValorTitulo == 0 ? "" : Boleto.ValorTitulo.ToString("C", _culture))
                .Replace("@DESCONTOS", !Boleto.ImprimirValoresAuxiliares || Boleto.ValorDesconto == 0 ? "" : Boleto.ValorDesconto.ToString("C", _culture))
                .Replace("@OUTRASDEDUCOES", !Boleto.ImprimirValoresAuxiliares || Boleto.ValorAbatimento == 0 ? "" : Boleto.ValorAbatimento.ToString("C", _culture))
                .Replace("@MORAMULTA", !Boleto.ImprimirValoresAuxiliares || Boleto.ValorMulta == 0 ? "" : Boleto.ValorMulta.ToString("C", _culture))
                .Replace("@OUTROSACRESCIMOS", !Boleto.ImprimirValoresAuxiliares || Boleto.ValorOutrasDespesas == 0 ? "" : Boleto.ValorOutrasDespesas.ToString("C", _culture))
                .Replace("@VALORCOBRADO", !Boleto.ImprimirValoresAuxiliares || Boleto.ValorPago == 0 ? "" : Boleto.ValorPago.ToString("C", _culture))
                .Replace("@AGENCIACONTA", Boleto.Banco.Beneficiario.CodigoFormatado)
                .Replace("@PAGADOR", pagador)
                .Replace("@ENDERECOPAGADOR", enderecoPagador)
                .Replace("@AVALISTA", avalista)
                .Replace("@AGENCIACODIGOBENEFICIARIO", Boleto.Banco.Beneficiario.CodigoFormatado)
                .Replace("@CPFCNPJ", Utils.FormataCPFCPPJ(Boleto.Banco.Beneficiario.CPFCNPJ))
                .Replace("@AUTENTICACAOMECANICA", "")
                .Replace("@USODOBANCO", Boleto.UsoBanco)
                .Replace("@IMAGEMCODIGOBARRA", imagemCodigoBarras)
                .Replace("@ACEITE", Boleto.Aceite).ToString()
                .Replace("@ENDERECOBENEFICIARIO_BOLETO", MostrarEnderecoBeneficiario ? $" - {enderecoBeneficiarioCompacto}" : "")
                .Replace("@ENDERECOBENEFICIARIO", MostrarEnderecoBeneficiario ? enderecoBeneficiario : "")
                .Replace("@INSTRUCOES", Boleto.MensagemInstrucoesCaixaFormatado.Replace(Environment.NewLine, "<br/>"))
                .Replace("@PARCELAS", Boleto.ParcelaInformativo != string.Empty ? ("Parcela: " + Boleto.ParcelaInformativo) : "");
        }

        #endregion Html

        #region Geração do Html OffLine

        /// <summary>
        /// Função utilizada para gerar o html do boleto sem que o mesmo esteja dentro de uma página Web.
        /// </summary>
        /// <param name="textoNoComecoDoEmail">O texto a ser incluído no início do e-mail.</param>
        /// <param name="srcLogo">Local apontado pela imagem de logo.</param>
        /// <param name="srcBarra">Local apontado pela imagem de barra.</param>
        /// <param name="srcCodigoBarra">Local apontado pela imagem do código de barras.</param>
        /// <param name="usaCsspdf">Indica se os estilos CSS para renderização em PDF devem ser aplicados.</param>
        /// <param name="pixStr">A string de pagamento PIX a ser incluída no HTML, se aplicável.</param>
        /// <param name="pixTamanhoImagem">O tamanho da imagem PIX em pixels.</param>
        /// <param name="pixInstrucoes">Indica se o QrCode de pagamento PIX deve ser incluído nas instruções.</param>
        /// <returns>StringBuilder conténdo o código html do boleto bancário.</returns>
        protected StringBuilder HtmlOffLine(string textoNoComecoDoEmail, string srcLogo, string srcBarra, string srcCodigoBarra, bool usaCsspdf = false, string pixStr = null, int pixTamanhoImagem = 200, bool pixInstrucoes = false)
        {
            var html = new StringBuilder();
            HtmlOfflineHeader(html, usaCsspdf);
            if (!string.IsNullOrEmpty(textoNoComecoDoEmail))
            {
                html.Append(textoNoComecoDoEmail);
            }

            html.Append(MontaHtml(srcLogo, srcBarra, srcCodigoBarra, pixStr, pixTamanhoImagem, pixInstrucoes));
            HtmlOfflineFooter(html);
            return html;
        }

        /// <summary>
        /// Monta o Header de um email com pelo menos um boleto dentro.
        /// </summary>
        /// <param name="html">StringBuilder usado para construir o conteúdo HTML.</param>
        /// <param name="usaCsspdf">Indica se os estilos CSS para renderização em PDF devem ser aplicados.</param>
        protected static void HtmlOfflineHeader(StringBuilder html, bool usaCsspdf = false)
        {
            html.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n");
            html.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">\n");
            html.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\n");
            html.Append("<meta charset=\"utf-8\"/>\n");
            html.Append("<head>");
            html.Append("    <title>Boleto.Net</title>\n");

            #region Css

            {
                var arquivoCss = usaCsspdf ? "BoletoNetCore.BoletoImpressao.BoletoNetPDF.css" : "BoletoNetCore.BoletoImpressao.BoletoNet.css";
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(arquivoCss);

                using (var sr = new StreamReader(stream))
                {
                    html.Append("<style>\n");
                    html.Append(sr.ReadToEnd());
                    html.Append("</style>\n");
                    sr.Close();
                }
            }

            #endregion Css

            html.Append("     </head>\n");
            html.Append("<body>\n");
        }

        /// <summary>
        /// Monta o Footer de um email com pelo menos um boleto dentro.
        /// </summary>
        /// <param name="saida">StringBuilder onde o conteudo sera salvo.</param>
        protected static void HtmlOfflineFooter(StringBuilder saida)
        {
            saida.Append("</body>\n");
            saida.Append("</html>\n");
        }

        /// <summary>
        /// Junta varios boletos em uma unica AlternateView, para todos serem mandados juntos no mesmo email
        /// </summary>
        /// <param name="arrayDeBoletos">Array contendo os boletos a serem mesclados</param>
        /// <returns></returns>
        public static AlternateView GeraHtmlDeVariosBoletosParaEmail(BoletoBancario[] arrayDeBoletos)
        {
            return GeraHtmlDeVariosBoletosParaEmail(null, arrayDeBoletos);
        }

        /// <summary>
        /// Junta varios boletos em uma unica AlternateView, para todos serem mandados juntos no mesmo email
        /// </summary>
        /// <param name="textoNoComecoDoEmail">Texto em HTML a ser adicionado no comeco do email</param>
        /// <param name="arrayDeBoletos">Array contendo os boletos a serem mesclados</param>
        /// <returns>AlternateView com os dados de todos os boleto.</returns>
        public static AlternateView GeraHtmlDeVariosBoletosParaEmail(string textoNoComecoDoEmail, BoletoBancario[] arrayDeBoletos)
        {
            var corpoDoEmail = new StringBuilder();

            var linkedResources = new List<LinkedResource>();
            HtmlOfflineHeader(corpoDoEmail);
            if (!string.IsNullOrEmpty(textoNoComecoDoEmail))
            {
                corpoDoEmail.Append(textoNoComecoDoEmail);
            }

            foreach (var umBoleto in arrayDeBoletos)
            {
                if (umBoleto != null)
                {
                    umBoleto.GeraGraficosParaEmailOffLine(out LinkedResource lrImagemLogo,
                        out LinkedResource lrImagemBarra,
                        out LinkedResource lrImagemCodigoBarra);
                    var theOutput = umBoleto.MontaHtml(
                        $"cid:{lrImagemLogo.ContentId}",
                        $"cid:{lrImagemBarra.ContentId}",
                        $"cid:{lrImagemCodigoBarra.ContentId}");

                    corpoDoEmail.Append(theOutput);

                    linkedResources.Add(lrImagemLogo);
                    linkedResources.Add(lrImagemBarra);
                    linkedResources.Add(lrImagemCodigoBarra);
                }
            }

            HtmlOfflineFooter(corpoDoEmail);

            var av = AlternateView.CreateAlternateViewFromString(corpoDoEmail.ToString(), Encoding.Default, "text/html");
            foreach (var theResource in linkedResources)
            {
                av.LinkedResources.Add(theResource);
            }

            return av;
        }

        /// <summary>
        /// Função utilizada gerar o AlternateView necessário para enviar um boleto bancário por e-mail.
        /// </summary>
        /// <returns>AlternateView com os dados do boleto.</returns>
        public AlternateView HtmlBoletoParaEnvioEmail()
        {
            return HtmlBoletoParaEnvioEmail(null);
        }

        /// <summary>
        /// Função utilizada gerar o AlternateView necessário para enviar um boleto bancário por e-mail.
        /// </summary>
        /// <param name="textoNoComecoDoEmail">Texto (em HTML) a ser incluido no começo do Email.</param>
        /// <returns>AlternateView com os dados do boleto.</returns>
        public AlternateView HtmlBoletoParaEnvioEmail(string textoNoComecoDoEmail)
        {
            GeraGraficosParaEmailOffLine(out LinkedResource lrImagemLogo, out LinkedResource lrImagemBarra, out LinkedResource lrImagemCodigoBarra);
            var html = HtmlOffLine(textoNoComecoDoEmail, $"cid:{lrImagemLogo.ContentId}", $"cid:{lrImagemBarra.ContentId}", $"cid:{lrImagemCodigoBarra.ContentId}");

            var av = AlternateView.CreateAlternateViewFromString(html.ToString(), Encoding.Default, "text/html");

            av.LinkedResources.Add(lrImagemLogo);
            av.LinkedResources.Add(lrImagemBarra);
            av.LinkedResources.Add(lrImagemCodigoBarra);
            return av;
        }

        /// <summary>
        /// Gera as tres imagens necessárias para o Boleto
        /// </summary>
        /// <param name="lrImagemLogo">O Logo do Banco</param>
        /// <param name="lrImagemBarra">A Barra Horizontal</param>
        /// <param name="lrImagemCodigoBarra">O Código de Barras</param>
        void GeraGraficosParaEmailOffLine(out LinkedResource lrImagemLogo, out LinkedResource lrImagemBarra, out LinkedResource lrImagemCodigoBarra)
        {
            var randomSufix = new Random().Next().ToString(); // para podermos colocar no mesmo email varios boletos diferentes

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"BoletoNetCore.Imagens.{Utils.FormatCode(Boleto.Banco.Codigo.ToString(), 3)}.jpg");
            lrImagemLogo = new LinkedResource(stream, MediaTypeNames.Image.Jpeg)
            {
                ContentId = $"logo{randomSufix}"
            };

            var assembly = Assembly.GetExecutingAssembly();
            var ms = assembly.GetManifestResourceStream("BoletoNetCore.Imagens.barra.jpg");
            lrImagemBarra = new LinkedResource(ms, MediaTypeNames.Image.Jpeg)
            {
                ContentId = $"barra{randomSufix}"
            };

            var cb = new BarCode2of5i(Boleto.CodigoBarra.CodigoDeBarras, 1, 50,
                Boleto.CodigoBarra.CodigoDeBarras.Length);
            ms = new MemoryStream(Utils.ConvertImageToByte(cb.ToBitmap()));

            lrImagemCodigoBarra = new LinkedResource(ms, MediaTypeNames.Image.Gif)
            {
                ContentId = $"codigobarra{randomSufix}"
            };
        }

        /// <summary>
        /// Monta o Html do boleto bancário com as imagens embutidas no conteúdo, sem necessidade de links externos
        /// de acordo com o padrão http://en.wikipedia.org/wiki/Data_URI_scheme
        /// 
        /// Alterado por Olavo Rocha @ Exodus para utilizar arquivos dentro da própria DLL para .net core 2.2 
        /// </summary>
        /// <param name="convertLinhaDigitavelToImage">Indica se a linha de cobrança deve ser convertida em uma imagem para evitar exploração por malware.</param>
        /// <param name="usaCsspdf">Determina se os estilos CSS específicos para renderização em formato PDF devem ser aplicados.</param>
        /// <param name="urlImagemLogoBeneficiario">Especifica a URL ou a imagem codificada em Base64 do logotipo do beneficiário a ser exibida no comprovante bancário.</param>
        /// <param name="pixString">A string de pagamento PIX a ser incorporada no HTML, se aplicável.</param>
        /// <param name="pixTamanhoImagem">O tamanho da imagem do código QR PIX, em pixels.</param>
        /// <param name="pixInstrucoes">Indica se o QrCode de pagamento PIX deve ser incluído nas instruções.</param>
        /// <returns>Uma string representando o HTML gerado para o comprovante bancário.</returns>
        public string MontaHtmlEmbedded(bool convertLinhaDigitavelToImage = false, bool usaCsspdf = false, string urlImagemLogoBeneficiario = null, string pixString = null, int pixTamanhoImagem = 200, bool pixInstrucoes = false)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var imageLogo = $"BoletoNetCore.Imagens.{Boleto.Banco.Codigo:000}.jpg";
            var streamLogo = assembly.GetManifestResourceStream(imageLogo);
            var base64Logo = Convert.ToBase64String(new BinaryReader(streamLogo).ReadBytes((int)streamLogo.Length));
            var fnLogo = $"data:image/jpg;base64,{base64Logo}";

            var streamBarra = assembly.GetManifestResourceStream("BoletoNetCore.Imagens.barra.jpg");
            var base64Barra = Convert.ToBase64String(new BinaryReader(streamBarra).ReadBytes((int)streamBarra.Length));
            var fnBarra = $"data:image/jpg;base64,{base64Barra}";

            var cb = new BarCode2of5i(Boleto.CodigoBarra.CodigoDeBarras, 1, 50, Boleto.CodigoBarra.CodigoDeBarras.Length);
            var base64CodigoBarras = Convert.ToBase64String(cb.ToByte());
            var fnCodigoBarras = $"data:image/gif;base64,{base64CodigoBarras}";

            if (convertLinhaDigitavelToImage)
            {
                var linhaDigitavel = Boleto.CodigoBarra.LinhaDigitavel.Replace("  ", " ").Trim();

                var font = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal,
                    SKFontStyleSlant.Upright);
                var imagemLinha = Utils.DrawText(linhaDigitavel, 40, font, SKColors.Black, SKColors.White);
                var base64Linha = Convert.ToBase64String(Utils.ConvertImageToByte(imagemLinha));

                Boleto.CodigoBarra.LinhaDigitavel = $@"<img style=""max-width:420px; margin-bottom: 2px"" src=""data:image/gif;base64,{base64Linha}"" />";
            }

            if (!string.IsNullOrEmpty(urlImagemLogoBeneficiario))
            {
                _vLocalLogoBeneficiario = urlImagemLogoBeneficiario;
            }

            var s = HtmlOffLine(null, fnLogo, fnBarra, fnCodigoBarras, usaCsspdf, pixString, pixTamanhoImagem, pixInstrucoes).ToString();

            if (convertLinhaDigitavelToImage)
            {
                s = s.Replace(".w500", "");
            }

            return s;
        }

        #endregion Geração do Html OffLine

        public SKBitmap GeraImagemCodigoBarras(Boleto boleto)
        {
            var cb = new BarCode2of5i(boleto.CodigoBarra.CodigoDeBarras, 1, 50, boleto.CodigoBarra.CodigoDeBarras.Length);
            return cb.ToBitmap();
        }

        private void CopiarStream(Stream entrada, Stream saida)
        {
            int bytesLidos;
            var imgBuffer = new byte[entrada.Length];

            while ((bytesLidos = entrada.Read(imgBuffer, 0, imgBuffer.Length)) > 0)
            {
                saida.Write(imgBuffer, 0, bytesLidos);
            }
        }
    }
}