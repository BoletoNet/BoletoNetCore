using System;
using System.IO;
using System.Text;

namespace BoletoNetCore.Pdf
{
    public class BoletoNetCorePdfProxy : BoletoNetCoreProxy
    {
        public override bool GerarBoletos(string nomeArquivo, ref string mensagemErro)
        {
            
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(nomeArquivo))
                {
                    mensagemErro = "Nome do arquivo não informado." + Environment.NewLine;
                    return false;
                }
                if (quantidadeBoletos == 0)
                {
                    mensagemErro = "Nenhum boleto encontrado." + Environment.NewLine;
                    return false;
                }
                var extensaoArquivo = nomeArquivo.Substring(nomeArquivo.Length - 3).ToUpper();
                if (extensaoArquivo != "HTM" && extensaoArquivo != "PDF")
                {
                    mensagemErro = "Tipo do arquivo inválido: HTM ou PDF" + Environment.NewLine;
                    return false;
                }
                var html = new StringBuilder();
                foreach (Boleto boletoTmp in boletos)
                {
                    BoletoBancario imprimeBoleto = new BoletoBancario
                    {
                        Boleto = boletoTmp,
                        OcultarInstrucoes = false,
                        MostrarComprovanteEntrega = true,
                        MostrarEnderecoBeneficiario = true
                    };
                    {
                        html.Append("<div style=\"page-break-after: always;\">");
                        html.Append(imprimeBoleto.MontaHtmlEmbedded());
                        html.Append("</div>");
                    }
                }
                switch (extensaoArquivo.ToUpper())
                {
                    case "HTM":
                        GerarArquivoTexto(html.ToString(), nomeArquivo);
                        break;
                    case "PDF":
                        GerarArquivoPDF(html.ToString(), nomeArquivo);
                        break;
                    default:
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;

            }
        }

        private void GerarArquivoPDF(string html, string nomeArquivo)
        {
#if NETSTANDARD2

            var pdf = new Wkhtmltopdf.NetCore.HtmlAsPdf().GetPDF(html);
            using (FileStream fs = new FileStream(nomeArquivo, FileMode.Create))
            {
                fs.Write(pdf, 0, pdf.Length);
                fs.Close();
            }
#else
            throw new NotImplementedException();
#endif

        }

    }
}
