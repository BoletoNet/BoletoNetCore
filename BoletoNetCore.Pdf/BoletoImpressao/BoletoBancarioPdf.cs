using System;

namespace BoletoNetCore.Pdf.BoletoImpressao
{
    public class BoletoBancarioPdf : BoletoBancario
    {
        public byte[] MontaBytesPDF(bool convertLinhaDigitavelToImage = false, string urlImagemLogoBeneficiario = null)
        {
#if NETSTANDARD2
            return (new Wkhtmltopdf.NetCore.HtmlAsPdf().GetPDF(MontaHtmlEmbedded(convertLinhaDigitavelToImage, true, urlImagemLogoBeneficiario)));
#else
            throw new NotImplementedException();
#endif


        }

        public byte[] MontaBytesPDFLegacy(bool convertLinhaDigitavelToImage = false, string urlImagemLogoBeneficiario = null)
        {
            return (new NReco.PdfGenerator.HtmlToPdfConverter()).GeneratePdf(MontaHtmlEmbedded(convertLinhaDigitavelToImage, true, urlImagemLogoBeneficiario));
        }
    }
}
