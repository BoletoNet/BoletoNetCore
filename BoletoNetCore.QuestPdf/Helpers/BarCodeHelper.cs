using Barcoder.Code128;
using Barcoder.Qr;
using Barcoder.Renderer.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BoletoNetCore.QuestPdf
{
    internal static class BarCodeHelper
    {
        public static byte[] GerarCodBarras128(this string codbar, int? heigthPng = null)
        {
            if (string.IsNullOrWhiteSpace(codbar))
                throw new Exception("Código de barras não informado");

            var bar = Code128Encoder.Encode(codbar);
            var render = new ImageRenderer(barHeightFor1DBarcode: heigthPng ?? 25);
            using (var ms = new MemoryStream())
            {
                render.Render(bar, ms);
                return ms.ToArray();
            }
        }

    }
}
