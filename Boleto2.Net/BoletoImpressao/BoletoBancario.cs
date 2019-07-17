using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
//Envio por email
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Web.UI;

[assembly: WebResource("Boleto2Net.BoletoImpressao.BoletoNet.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("Boleto2Net.Imagens.barra.gif", "image/gif")]

namespace Boleto2Net
{
    using System.Linq;

    [Serializable(),
    Designer(typeof(BoletoBancarioDesigner)),
    ToolboxBitmap(typeof(BoletoBancario)),
    ToolboxData("<{0}:BoletoBancario Runat=\"server\"></{0}:BoletoBancario>")]
    public class BoletoBancario : Control
    {
        String _vLocalLogoCedente = String.Empty;

        #region Variaveis

        private string _instrucoesHtml = string.Empty;
        private bool _mostrarCodigoCarteira = false;
        private bool _formatoCarne = false;
        #endregion Variaveis

        #region Propriedades

        /// <summary>
        /// Mostra o código da carteira
        /// </summary>
        [Browsable(true), Description("Mostra a descrição da carteira")]
        public bool MostrarCodigoCarteira
        {
            get { return _mostrarCodigoCarteira; }
            set { _mostrarCodigoCarteira = value; }
        }

        [Browsable(true), Description("Gera um relatório com os valores que deram origem ao boleto")]
        public bool ExibirDemonstrativo { get; set; }

        /// <summary>
        /// Mostra o código da carteira
        /// </summary>
        [Browsable(true), Description("Formata o boleto no layout de carnê")]
        public bool FormatoCarne
        {
            get { return _formatoCarne; }
            set { _formatoCarne = value; }
        }

        [Browsable(false)]
        public Boleto Boleto { get; set; }

        [Browsable(false)]
        public IBanco Banco { get; set; }

        #region Propriedades
        [Browsable(true), Description("Mostra o comprovante de entrega sem dados para marcar")]
        public bool MostrarComprovanteEntregaLivre
        {
            get { return Utils.ToBool(ViewState["1"]); }
            set { ViewState["1"] = value; }
        }

        [Browsable(true), Description("Mostra o comprovante de entrega")]
        public bool MostrarComprovanteEntrega
        {
            get { return Utils.ToBool(ViewState["2"]); }
            set { ViewState["2"] = value; }
        }

        [Browsable(true), Description("Oculta as intruções do boleto")]
        public bool OcultarEnderecoSacado
        {
            get { return Utils.ToBool(ViewState["3"]); }
            set { ViewState["3"] = value; }
        }

        [Browsable(true), Description("Oculta as intruções do boleto")]
        public bool OcultarInstrucoes
        {
            get { return Utils.ToBool(ViewState["4"]); }
            set { ViewState["4"] = value; }
        }

        [Browsable(true), Description("Oculta o recibo do sacado do boleto")]
        public bool OcultarReciboSacado
        {
            get { return Utils.ToBool(ViewState["5"]); }
            set { ViewState["5"] = value; }
        }

        [Browsable(true), Description("Gerar arquivo de remessa")]
        public bool GerarArquivoRemessa
        {
            get { return Utils.ToBool(ViewState["6"]); }
            set { ViewState["6"] = value; }
        }
        /// <summary> 
        /// Mostra o termo "Contra Apresentação" na data de vencimento do boleto
        /// </summary>
        public bool MostrarContraApresentacaoNaDataVencimento
        {
            get { return Utils.ToBool(ViewState["7"]); }
            set { ViewState["7"] = value; }
        }

        [Browsable(true), Description("Mostra o endereço do Cedente")]
        public bool MostrarEnderecoCedente
        {
            get { return Utils.ToBool(ViewState["8"]); }
            set { ViewState["8"] = value; }
        }
        #endregion Propriedades

        #endregion Propriedades

        public static string UrlLogo(int banco)
        {
            var page = System.Web.HttpContext.Current.CurrentHandler as Page;
            return page.ClientScript.GetWebResourceUrl(typeof(BoletoBancario), "Boleto2Net.Imagens." + Utils.FormatCode(banco.ToString(), 3) + ".jpg");
        }

        #region Override
        protected override void OnPreRender(EventArgs e)
        {
            var alias = "Boleto2Net.BoletoImpressao.BoletoNet.css";
            var csslink = "<link rel=\"stylesheet\" type=\"text/css\" href=\"" +
                Page.ClientScript.GetWebResourceUrl(typeof(BoletoBancario), alias) + "\" />";

            var include = new LiteralControl(csslink);
            Page.Header.Controls.Add(include);

            base.OnPreRender(e);
        }

        protected override void OnLoad(EventArgs e)
        {
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "Execution")]
        protected override void Render(HtmlTextWriter output)
        {
            var urlImagemLogo = Page.ClientScript.GetWebResourceUrl(typeof(BoletoBancario), "Boleto2Net.Imagens." + Utils.FormatCode(Boleto.Banco.Codigo.ToString(), 3) + ".jpg");
            var urlImagemBarra = Page.ClientScript.GetWebResourceUrl(typeof(BoletoBancario), "Boleto2Net.Imagens.barra.gif");
            //string urlImagemBarraInterna = Page.ClientScript.GetWebResourceUrl(typeof(BoletoBancario), "BoletoNet.Imagens.barrainterna.gif");
            //string urlImagemCorte = Page.ClientScript.GetWebResourceUrl(typeof(BoletoBancario), "BoletoNet.Imagens.corte.gif");
            //string urlImagemPonto = Page.ClientScript.GetWebResourceUrl(typeof(BoletoBancario), "BoletoNet.Imagens.ponto.gif");

            //Atribui os valores ao html do boleto bancário
            //output.Write(MontaHtml(urlImagemCorte, urlImagemLogo, urlImagemBarra, urlImagemPonto, urlImagemBarraInterna,
            //    "<img src=\"ImagemCodigoBarra.ashx?code=" + Boleto.CodigoBarra.Codigo + "\" alt=\"Código de Barras\" />"));
            output.Write(MontaHtml(urlImagemLogo, urlImagemBarra, "<img src=\"ImagemCodigoBarra.ashx?code=" + Boleto.CodigoBarra.CodigoDeBarras + "\" alt=\"Código de Barras\" />"));
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

                html.Append(Html.Instrucoes);
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

        private string GeraHtmlCarne(string telefone, string htmlBoleto)
        {
            var html = new StringBuilder();

            html.Append(Html.Carne);

            return html.ToString()
                .Replace("@TELEFONE", telefone)
                .Replace("#BOLETO#", htmlBoleto);
        }
        public string GeraHtmlReciboSacado()
        {
            try
            {
                var html = new StringBuilder();
                html.Append(Html.ReciboSacadoParte1);
                html.Append("<br />");
                html.Append(Html.ReciboSacadoParte2);
                html.Append(Html.ReciboSacadoParte3);
                if (MostrarEnderecoCedente)
                {
                    html.Append(Html.ReciboSacadoParte10);
                }
                html.Append(Html.ReciboSacadoParte4);
                html.Append(Html.ReciboSacadoParte5);
                html.Append(Html.ReciboSacadoParte6);
                html.Append(Html.ReciboSacadoParte7);
                html.Append(Html.ReciboSacadoParte8);
                return html.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a execução da transação.", ex);
            }
        }

        public string GeraHtmlReciboCedente()
        {
            try
            {
                var html = new StringBuilder();
                html.Append(Html.ReciboCedenteParte1);
                html.Append(Html.ReciboCedenteParte2);
                html.Append(Html.ReciboCedenteParte3);
                html.Append(Html.ReciboCedenteParte4);
                html.Append(Html.ReciboCedenteParte5);
                html.Append(Html.ReciboCedenteParte6);
                html.Append(Html.ReciboCedenteParte7);
                html.Append(Html.ReciboCedenteParte8);
                html.Append(Html.ReciboCedenteParte9);
                html.Append(Html.ReciboCedenteParte10);
                html.Append(Html.ReciboCedenteParte11);
                html.Append(Html.ReciboCedenteParte12);
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

                html.Append(Html.ComprovanteEntrega1);
                html.Append(Html.ComprovanteEntrega2);
                html.Append(Html.ComprovanteEntrega3);
                html.Append(Html.ComprovanteEntrega4);
                html.Append(Html.ComprovanteEntrega5);
                html.Append(Html.ComprovanteEntrega6);

                html.Append(MostrarComprovanteEntregaLivre ? Html.ComprovanteEntrega71 : Html.ComprovanteEntrega7);

                html.Append("<br />");
                return html.ToString();
            }
        }

        private string MontaHtml(string urlImagemLogo, string urlImagemBarra, string imagemCodigoBarras)
        {
            var html = new StringBuilder();
            var enderecoCedente = "";
            var enderecoCedenteCompacto = "";

            //Oculta o cabeçalho das instruções do boleto
            if (!OcultarInstrucoes)
                html.Append(GeraHtmlInstrucoes());

            if (ExibirDemonstrativo && Boleto.Demonstrativos.Any())
            {
                html.Append(Html.ReciboCedenteRelatorioValores);
                html.Append(Html.ReciboCedenteParte5);

                html.Append(Html.CabecalhoTabelaDemonstrativo);

                var grupoDemonstrativo = new StringBuilder();

                foreach (var relatorio in Boleto.Demonstrativos)
                {
                    var first = true;

                    foreach (var item in relatorio.Itens)
                    {
                        grupoDemonstrativo.Append(Html.GrupoDemonstrativo);

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
                        grupoDemonstrativo = grupoDemonstrativo.Replace("@VALORITEM", item.Valor.ToString("R$ ##,##0.00"));
                    }

                    grupoDemonstrativo.Append(Html.TotalDemonstrativo);
                    grupoDemonstrativo = grupoDemonstrativo.Replace(
                        "@VALORTOTALGRUPO",
                        relatorio.Itens.Sum(c => c.Valor).ToString("R$ ##,##0.00"));
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
                    if (OcultarReciboSacado)
                        html.Append(Html.ReciboSacadoParte8);
                }

                //Oculta o recibo do sacabo do boleto
                if (!OcultarReciboSacado)
                {
                    html.Append(GeraHtmlReciboSacado());

                    //Caso mostre o Endereço do Cedente
                    if (MostrarEnderecoCedente)
                    {
                        if (Boleto.Banco.Cedente.Endereco == null)
                            throw new ArgumentNullException("Endereço do Cedente");

                        enderecoCedente = string.Format("{0} - {1} - {2}/{3} - CEP: {4}",
                                                            Boleto.Banco.Cedente.Endereco.FormataLogradouro(0),
                                                            Boleto.Banco.Cedente.Endereco.Bairro,
                                                            Boleto.Banco.Cedente.Endereco.Cidade,
                                                            Boleto.Banco.Cedente.Endereco.UF,
                                                            Utils.FormataCEP(Boleto.Banco.Cedente.Endereco.CEP));
                        enderecoCedenteCompacto = string.Format("{0} - CEP: {1}",
                                                            Boleto.Banco.Cedente.Endereco.FormataLogradouro(25),
                                                            Utils.FormataCEP(Boleto.Banco.Cedente.Endereco.CEP));
                    }
                }
            }

            // Dados do Sacado
            var sacado = Boleto.Sacado.Nome;
            switch (Boleto.Sacado.TipoCPFCNPJ("A"))
            {
                case "F":
                    sacado += string.Format(" - CPF: " + Utils.FormataCPF(Boleto.Sacado.CPFCNPJ));
                    break;
                case "J":
                    sacado += string.Format(" - CNPJ: " + Utils.FormataCNPJ(Boleto.Sacado.CPFCNPJ));
                    break;
            }
            if (Boleto.Sacado.Observacoes != string.Empty)
                sacado += " - " + Boleto.Sacado.Observacoes;

            var enderecoSacado = string.Empty;
            if (!OcultarEnderecoSacado)
            {
                enderecoSacado = Boleto.Sacado.Endereco.FormataLogradouro(0) + "<br />" + string.Format("{0} - {1}/{2}", Boleto.Sacado.Endereco.Bairro, Boleto.Sacado.Endereco.Cidade, Boleto.Sacado.Endereco.UF);
                if (Boleto.Sacado.Endereco.CEP != String.Empty)
                    enderecoSacado += string.Format(" - CEP: {0}", Utils.FormataCEP(Boleto.Sacado.Endereco.CEP));
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
                html.Append(GeraHtmlReciboCedente());
            else
            {
                html.Append(GeraHtmlCarne("", GeraHtmlReciboCedente()));
            }

            var dataVencimento = Boleto.DataVencimento.ToString("dd/MM/yyyy");

            if (MostrarContraApresentacaoNaDataVencimento)
                dataVencimento = "Contra Apresentação";

            if (String.IsNullOrWhiteSpace(_vLocalLogoCedente))
                _vLocalLogoCedente = urlImagemLogo;

            return html
                .Replace("@CODIGOBANCO", Utils.FormatCode(Boleto.Banco.Codigo.ToString(), 3))
                .Replace("@DIGITOBANCO", Boleto.Banco.Digito.ToString())
                .Replace("@URLIMAGEMLOGO", urlImagemLogo)
                .Replace("@URLIMGCEDENTE", _vLocalLogoCedente)
                .Replace("@URLIMAGEMBARRA", urlImagemBarra)
                .Replace("@LINHADIGITAVEL", Boleto.CodigoBarra.LinhaDigitavel)
                .Replace("@LOCALPAGAMENTO", Boleto.Banco.Cedente.ContaBancaria.LocalPagamento)
                .Replace("@MENSAGEMFIXATOPOBOLETO", Boleto.Banco.Cedente.ContaBancaria.MensagemFixaTopoBoleto)
                .Replace("@MENSAGEMFIXASACADO", Boleto.Banco.Cedente.ContaBancaria.MensagemFixaSacado)
                .Replace("@DATAVENCIMENTO", dataVencimento)
                .Replace("@CEDENTE_BOLETO", !Boleto.Banco.Cedente.MostrarCNPJnoBoleto ? Boleto.Banco.Cedente.Nome : string.Format("{0} - {1}", Boleto.Banco.Cedente.Nome, Utils.FormataCNPJ(Boleto.Banco.Cedente.CPFCNPJ)))
                .Replace("@CEDENTE", Boleto.Banco.Cedente.Nome)
                .Replace("@DATADOCUMENTO", Boleto.DataEmissao.ToString("dd/MM/yyyy"))
                .Replace("@NUMERODOCUMENTO", Boleto.NumeroDocumento)
                .Replace("@ESPECIEDOCUMENTO", Boleto.EspecieDocumento.ToString())
                .Replace("@DATAPROCESSAMENTO", Boleto.DataProcessamento.ToString("dd/MM/yyyy"))
                .Replace("@NOSSONUMERO", Boleto.NossoNumeroFormatado)
                .Replace("@CARTEIRA", Boleto.CarteiraImpressaoBoleto)
                .Replace("@ESPECIE", Boleto.EspecieMoeda)
                .Replace("@QUANTIDADE", (Boleto.QuantidadeMoeda == 0 ? "" : Boleto.QuantidadeMoeda.ToString()))
                .Replace("@VALORDOCUMENTO", Boleto.ValorMoeda)
                .Replace("@=VALORDOCUMENTO", (Boleto.ValorTitulo == 0 ? "" : Boleto.ValorTitulo.ToString("R$ ##,##0.00")))
                .Replace("@DESCONTOS", (Boleto.ImprimirValoresAuxiliares == false || Boleto.ValorDesconto == 0 ? "" : Boleto.ValorDesconto.ToString("R$ ##,##0.00")))
                .Replace("@OUTRASDEDUCOES", (Boleto.ImprimirValoresAuxiliares == false || Boleto.ValorAbatimento == 0 ? "" : Boleto.ValorAbatimento.ToString("R$ ##,##0.00")))
                .Replace("@MORAMULTA", (Boleto.ImprimirValoresAuxiliares == false || Boleto.ValorMulta == 0 ? "" : Boleto.ValorMulta.ToString("R$ ##,##0.00")))
                .Replace("@OUTROSACRESCIMOS", (Boleto.ImprimirValoresAuxiliares == false || Boleto.ValorOutrasDespesas == 0 ? "" : Boleto.ValorOutrasDespesas.ToString("R$ ##,##0.00")))
                .Replace("@VALORCOBRADO", (Boleto.ImprimirValoresAuxiliares == false || Boleto.ValorPago == 0 ? "" : Boleto.ValorPago.ToString("R$ ##,##0.00")))
                .Replace("@AGENCIACONTA", Boleto.Banco.Cedente.CodigoFormatado)
                .Replace("@SACADO", sacado)
                .Replace("@ENDERECOSACADO", enderecoSacado)
                .Replace("@AVALISTA", avalista)
                .Replace("@AGENCIACODIGOCEDENTE", Boleto.Banco.Cedente.CodigoFormatado)
                .Replace("@CPFCNPJ", Boleto.Banco.Cedente.CPFCNPJ)
                .Replace("@AUTENTICACAOMECANICA", "")
                .Replace("@USODOBANCO", Boleto.UsoBanco)
                .Replace("@IMAGEMCODIGOBARRA", imagemCodigoBarras)
                .Replace("@ACEITE", Boleto.Aceite).ToString()
                .Replace("@ENDERECOCEDENTE_BOLETO", MostrarEnderecoCedente ? string.Format(" - {0}", enderecoCedenteCompacto) : "")
                .Replace("@ENDERECOCEDENTE", MostrarEnderecoCedente ? enderecoCedente : "")
                .Replace("@INSTRUCOES", Boleto.MensagemInstrucoesCaixa);
        }

        #endregion Html

        #region Geração do Html OffLine

        /// <summary>
        /// Função utilizada para gerar o html do boleto sem que o mesmo esteja dentro de uma página Web.
        /// </summary>
        /// <param name="srcLogo">Local apontado pela imagem de logo.</param>
        /// <param name="srcBarra">Local apontado pela imagem de barra.</param>
        /// <param name="srcCodigoBarra">Local apontado pela imagem do código de barras.</param>
        /// <returns>StringBuilder conténdo o código html do boleto bancário.</returns>
        protected StringBuilder HtmlOffLine(string textoNoComecoDoEmail, string srcLogo, string srcBarra, string srcCodigoBarra, bool usaCsspdf = false)
        {//protected StringBuilder HtmlOffLine(string srcCorte, string srcLogo, string srcBarra, string srcPonto, string srcBarraInterna, string srcCodigoBarra)
            OnLoad(EventArgs.Empty);

            var html = new StringBuilder();
            HtmlOfflineHeader(html, usaCsspdf);
            if (!string.IsNullOrEmpty(textoNoComecoDoEmail))
            {
                html.Append(textoNoComecoDoEmail);
            }
            html.Append(MontaHtml(srcLogo, srcBarra, "<img src=\"" + srcCodigoBarra + "\" alt=\"Código de Barras\" />"));
            HtmlOfflineFooter(html);
            return html;
        }




        /// <summary>
        /// Monta o Header de um email com pelo menos um boleto dentro.
        /// </summary>
        /// <param name="saida">StringBuilder onde o conteudo sera salvo.</param>
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
                var arquivoCss = usaCsspdf ? "Boleto2Net.BoletoImpressao.BoletoNetPDF.css" : "Boleto2Net.BoletoImpressao.BoletoNet.css";
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(arquivoCss);

                using (var sr = new StreamReader(stream))
                {
                    html.Append("<style>\n");
                    html.Append(sr.ReadToEnd());
                    html.Append("</style>\n");
                    sr.Close();
                    sr.Dispose();
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
            if (textoNoComecoDoEmail != null && textoNoComecoDoEmail != "")
            {
                corpoDoEmail.Append(textoNoComecoDoEmail);
            }
            foreach (var umBoleto in arrayDeBoletos)
            {
                if (umBoleto != null)
                {
                    LinkedResource lrImagemLogo;
                    LinkedResource lrImagemBarra;
                    LinkedResource lrImagemCodigoBarra;
                    umBoleto.GeraGraficosParaEmailOffLine(out lrImagemLogo, out lrImagemBarra, out lrImagemCodigoBarra);
                    var theOutput = umBoleto.MontaHtml(
                        "cid:" + lrImagemLogo.ContentId,
                        "cid:" + lrImagemBarra.ContentId,
                        "<img src=\"cid:" + lrImagemCodigoBarra.ContentId + "\" alt=\"Código de Barras\" />");

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
            LinkedResource lrImagemLogo;
            LinkedResource lrImagemBarra;
            LinkedResource lrImagemCodigoBarra;

            GeraGraficosParaEmailOffLine(out lrImagemLogo, out lrImagemBarra, out lrImagemCodigoBarra);
            var html = HtmlOffLine(textoNoComecoDoEmail, "cid:" + lrImagemLogo.ContentId, "cid:" + lrImagemBarra.ContentId, "cid:" + lrImagemCodigoBarra.ContentId);

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
            OnLoad(EventArgs.Empty);

            var randomSufix = new Random().Next().ToString(); // para podermos colocar no mesmo email varios boletos diferentes

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Boleto2Net.Imagens." + Utils.FormatCode(Boleto.Banco.Codigo.ToString(), 3) + ".jpg");
            lrImagemLogo = new LinkedResource(stream, MediaTypeNames.Image.Jpeg)
            {
                ContentId = "logo" + randomSufix
            };

            var ms = new MemoryStream(Utils.ConvertImageToByte(Html.barra));
            lrImagemBarra = new LinkedResource(ms, MediaTypeNames.Image.Gif)
            {
                ContentId = "barra" + randomSufix
            };

            var cb = new BarCode2of5i(Boleto.CodigoBarra.CodigoDeBarras, 1, 50, Boleto.CodigoBarra.CodigoDeBarras.Length);
            ms = new MemoryStream(Utils.ConvertImageToByte(cb.ToBitmap()));

            lrImagemCodigoBarra = new LinkedResource(ms, MediaTypeNames.Image.Gif)
            {
                ContentId = "codigobarra" + randomSufix
            };

        }


        /// <summary>
        /// Função utilizada para gravar em um arquivo local o conteúdo do boleto. Este arquivo pode ser aberto em um browser sem que o site esteja no ar.
        /// </summary>
        /// <param name="fileName">Path do arquivo que deve conter o código html.</param>
        public void MontaHtmlNoArquivoLocal(string fileName)
        {
            using (var f = new FileStream(fileName, FileMode.Create))
            {
                var w = new StreamWriter(f, Encoding.Default);
                w.Write(MontaHtml());
                w.Close();
                f.Close();
            }
        }

        /// <summary>
        /// Monta o Html do boleto bancário
        /// </summary>
        /// <returns>string</returns>
        public string MontaHtml()
        {
            return MontaHtml(null, null);
        }


        /// <summary>
        /// Monta o Html do boleto bancário
        /// </summary>
        /// <param name="fileName">Caminho do arquivo</param>
        /// <param name="fileName">Caminho do logo do cedente</param>
        /// <returns>Html do boleto gerado</returns>
        public string MontaHtml(string fileName, string logoCedente)
        {
            if (fileName == null)
                fileName = Path.GetTempPath();

            if (logoCedente != null)
                _vLocalLogoCedente = logoCedente;

            OnLoad(EventArgs.Empty);

            var fnLogo = fileName + @"BoletoNet" + Utils.FormatCode(Boleto.Banco.Codigo.ToString(), 3) + ".jpg";

            if (!File.Exists(fnLogo))
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Boleto2Net.Imagens." + Utils.FormatCode(Boleto.Banco.Codigo.ToString(), 3) + ".jpg");
                using (Stream file = File.Create(fnLogo))
                {
                    CopiarStream(stream, file);
                }
            }

            var fnBarra = fileName + @"BoletoNetBarra.gif";
            if (!File.Exists(fnBarra))
            {
                var imgConverter = new ImageConverter();
                var imgBuffer = (byte[])imgConverter.ConvertTo(Html.barra, typeof(byte[]));
                var ms = new MemoryStream(imgBuffer);

                using (Stream stream = File.Create(fnBarra))
                {
                    CopiarStream(ms, stream);
                    ms.Flush();
                    ms.Dispose();
                }
            }

            var fnCodigoBarras = Path.GetTempFileName();
            var cb = new BarCode2of5i(Boleto.CodigoBarra.CodigoDeBarras, 1, 50, Boleto.CodigoBarra.CodigoDeBarras.Length);
            cb.ToBitmap().Save(fnCodigoBarras);

            //return HtmlOffLine(fnCorte, fnLogo, fnBarra, fnPonto, fnBarraInterna, fnCodigoBarras).ToString();
            return HtmlOffLine(null, fnLogo, fnBarra, fnCodigoBarras).ToString();
        }

        /// <summary>
        /// Monta o Html do boleto bancário para View ASP.Net MVC
        /// <code>
        /// <para>Exemplo:</para>
        /// <para>public ActionResult VisualizarBoleto(string Id)</para>
        /// <para>{</para>
        /// <para>    BoletoBancario bb = new BoletoBancario();</para>
        /// <para>    //...</para>
        /// <para>    ViewBag.Boleto = bb.MontaHtml("/Content/Boletos/", "teste1");</para>
        /// <para>    return View();</para>
        /// <para>}</para>
        /// <para>//Na view</para>
        /// <para>@{Layout = null;}@Html.Raw(ViewBag.Boleto)</para>
        /// </code>
        /// </summary>
        /// <param name="Url">Pasta dos boletos. Exemplo MontaHtml("/Content/Boletos/", "000100")</param>
        /// <param name="fileName">Nome do arquivo para o boleto</param>
        /// <returns>Html do boleto gerado</returns>
        /// <desenvolvedor>Sandro Ribeiro</desenvolvedor>
        /// <criacao>16/11/2012</criacao>
        public string MontaHtml(string url, string fileName, bool useMapPathSecure = true)
        {
            //Variável para o caminho físico do servidor
            var pathServer = "";

            //Verifica se o usuário informou uma url válida
            if (url == null)
            {
                //Obriga o usuário a especificar uma url válida
                throw new ArgumentException("Você precisa informar uma pasta padrão.");
            }
            else
            {
                if (useMapPathSecure)
                {
                    //Verifica se o usuário usou barras no início e no final da url
                    if (url.Substring(url.Length - 1, 1) != "/")
                        url = url + "/";
                    if (url.Substring(0, 1) != "/")
                        url = url + "/";
                    //Mapeia o caminho físico dos arquivos
                    pathServer = MapPathSecure(string.Format("~{0}", url));
                }

                //Verifica se o caminho existe
                if (!Directory.Exists(pathServer))
                    throw new ArgumentException("A o caminho físico '{0}' não existe.", pathServer);
            }
            //Verifica o nome do arquivo
            if (fileName == null)
            {
                fileName = DateTime.Now.Ticks.ToString();
            }
            else
            {
                if (fileName == "")
                    fileName = DateTime.Now.Ticks.ToString();
            }

            //Mantive o padrão 
            OnLoad(EventArgs.Empty);

            //Prepara o arquivo da logo para ser salvo
            var fnLogo = pathServer + @"BoletoNet" + Utils.FormatCode(Boleto.Banco.Codigo.ToString(), 3) + ".jpg";
            //Prepara o arquivo da logo para ser usado no html
            var fnLogoUrl = url + @"BoletoNet" + Utils.FormatCode(Boleto.Banco.Codigo.ToString(), 3) + ".jpg";

            //Salvo a imagem apenas 1 vez com o código do banco
            if (!File.Exists(fnLogo))
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Boleto2Net.Imagens." + Utils.FormatCode(Boleto.Banco.Codigo.ToString(), 3) + ".jpg");
                using (Stream file = File.Create(fnLogo))
                {
                    CopiarStream(stream, file);
                }
            }

            //Prepara o arquivo da barra para ser salvo
            var fnBarra = pathServer + @"BoletoNetBarra.gif";
            //Prepara o arquivo da barra para ser usado no html
            var fnBarraUrl = url + @"BoletoNetBarra.gif";

            //Salvo a imagem apenas 1 vez
            if (!File.Exists(fnBarra))
            {
                var imgConverter = new ImageConverter();
                var imgBuffer = (byte[])imgConverter.ConvertTo(Html.barra, typeof(byte[]));
                var ms = new MemoryStream(imgBuffer);

                using (Stream stream = File.Create(fnBarra))
                {
                    CopiarStream(ms, stream);
                    ms.Flush();
                    ms.Dispose();
                }
            }

            //Prepara o arquivo do código de barras para ser salvo
            var fnCodigoBarras = string.Format("{0}{1}_codigoBarras.jpg", pathServer, fileName);
            //Prepara o arquivo do código de barras para ser usado no html
            var fnCodigoBarrasUrl = string.Format("{0}{1}_codigoBarras.jpg", url, fileName);

            var cb = new BarCode2of5i(Boleto.CodigoBarra.CodigoDeBarras, 1, 50, Boleto.CodigoBarra.CodigoDeBarras.Length);

            //Salva o arquivo conforme o fileName
            cb.ToBitmap().Save(fnCodigoBarras);

            //Retorna o Html para ser usado na view
            return HtmlOffLine(null, fnLogoUrl, fnBarraUrl, fnCodigoBarrasUrl).ToString();
        }

        /// <summary>
        /// Monta o Html do boleto bancário com as imagens embutidas no conteúdo, sem necessidade de links externos
        /// de acordo com o padrão http://en.wikipedia.org/wiki/Data_URI_scheme
        /// </summary>
        /// <param name="convertLinhaDigitavelToImage">Converte a Linha Digitável para imagem, com o objetivo de evitar malwares.</param>
        /// <param name="urlImagemLogoCedente">Url/Imagem Base64 da Logo do Cedente</param>
        /// <returns>Html do boleto gerado</returns>
        /// <desenvolvedor>Iuri André Stona</desenvolvedor>
        /// <criacao>23/01/2014</criacao>
        /// <alteracao>08/08/2014</alteracao>

        public string MontaHtmlEmbedded(bool convertLinhaDigitavelToImage = false, bool usaCsspdf = false, string urlImagemLogoCedente = null)
        {
            OnLoad(EventArgs.Empty);

            var assembly = Assembly.GetExecutingAssembly();

            var streamLogo = assembly.GetManifestResourceStream("Boleto2Net.Imagens." + Boleto.Banco.Codigo.ToString("000") + ".jpg");
            var base64Logo = Convert.ToBase64String(new BinaryReader(streamLogo).ReadBytes((int)streamLogo.Length));
            var fnLogo = string.Format("data:image/gif;base64,{0}", base64Logo);

            var streamBarra = assembly.GetManifestResourceStream("Boleto2Net.Imagens.barra.gif");
            var base64Barra = Convert.ToBase64String(new BinaryReader(streamBarra).ReadBytes((int)streamBarra.Length));
            var fnBarra = string.Format("data:image/gif;base64,{0}", base64Barra);

            var cb = new BarCode2of5i(Boleto.CodigoBarra.CodigoDeBarras, 1, 50, Boleto.CodigoBarra.CodigoDeBarras.Length);
            var base64CodigoBarras = Convert.ToBase64String(cb.ToByte());
            var fnCodigoBarras = string.Format("data:image/gif;base64,{0}", base64CodigoBarras);

            if (convertLinhaDigitavelToImage)
            {

                var linhaDigitavel = Boleto.CodigoBarra.LinhaDigitavel.Replace("  ", " ").Trim();

                var imagemLinha = Utils.DrawText(linhaDigitavel, new Font("Arial", 30, FontStyle.Bold), Color.Black, Color.White);
                var base64Linha = Convert.ToBase64String(Utils.ConvertImageToByte(imagemLinha));

                var fnLinha = string.Format("data:image/gif;base64,{0}", base64Linha);

                Boleto.CodigoBarra.LinhaDigitavel = @"<img style=""max-width:420px; margin-bottom: 2px"" src=" + fnLinha + " />";
            }

            if (!string.IsNullOrEmpty(urlImagemLogoCedente))
            {
                _vLocalLogoCedente = urlImagemLogoCedente;
            }

            var s = HtmlOffLine(null, fnLogo, fnBarra, fnCodigoBarras, usaCsspdf).ToString();

            if (convertLinhaDigitavelToImage)
            {
                s = s.Replace(".w500", "");
            }

            return s;
        }

        public byte[] MontaBytesPDF(bool convertLinhaDigitavelToImage = false, string urlImagemLogoCedente = null)
        {

            return (new NReco.PdfGenerator.HtmlToPdfConverter()).GeneratePdf(MontaHtmlEmbedded(convertLinhaDigitavelToImage, true, urlImagemLogoCedente));
        }
        #endregion Geração do Html OffLine

        public Image GeraImagemCodigoBarras(Boleto boleto)
        {
            var cb = new BarCode2of5i(boleto.CodigoBarra.CodigoDeBarras, 1, 50, boleto.CodigoBarra.CodigoDeBarras.Length);
            return cb.ToBitmap();
        }

        private void CopiarStream(Stream entrada, Stream saida)
        {
            var bytesLidos = 0;
            var imgBuffer = new byte[entrada.Length];

            while ((bytesLidos = entrada.Read(imgBuffer, 0, imgBuffer.Length)) > 0)
            {
                saida.Write(imgBuffer, 0, bytesLidos);
            }
        }
    }
}
