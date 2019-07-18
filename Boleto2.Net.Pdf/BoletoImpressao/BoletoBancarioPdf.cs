using Boleto2Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Boleto2.Net.Pdf.BoletoImpressao
{
    public class BoletoBancarioPdf : BoletoBancario
    {
        public byte[] MontaBytesPDF(bool convertLinhaDigitavelToImage = false, string urlImagemLogoCedente = null)
        {
#if NETSTANDARD2
            return (new Wkhtmltopdf.NetCore.HtmlAsPdf().GetPDF(MontaHtmlEmbedded(convertLinhaDigitavelToImage, true, urlImagemLogoCedente)));
#else
            throw new NotImplementedException();
#endif


        }
    }
}
