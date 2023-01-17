using NUnit.Framework;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    public class GeradorImagensTest
    {
        private const string _codigoBarras = "75696012340120105432102101020103645670000012350";

        [Test]
        public void Teste_DrawText()
        {
            var font = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            var imagem = Utils.DrawText(_codigoBarras, 40, font, SKColors.Black, SKColors.White);
            var bytes = Utils.ConvertImageToByte(imagem);
            SalvarAbrirImagemJpeg(bytes);
        }

        [Test]
        public void Teste_CodigoBarras()
        {
            var cb = new BarCode2of5i(_codigoBarras, 1, 50, _codigoBarras.Length);
            var bytes = cb.ToByte();
            SalvarAbrirImagemJpeg(bytes);
        }

        private static void SalvarAbrirImagemJpeg(byte[] bytes)
        {
            if (Debugger.IsAttached)
            {
                var fileName = Path.GetTempPath() + Guid.NewGuid() + ".jpeg";
                File.WriteAllBytes(fileName, bytes);
                Process.Start(@"cmd.exe ", @"/c " + fileName);
            }
        }
    }
}
