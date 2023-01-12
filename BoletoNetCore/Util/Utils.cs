using SkiaSharp;
using System;
using System.Globalization;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace BoletoNetCore
{
    static class Utils
    {
        internal static string FormatCode(string text, int length) => text.PadLeft(length, '0');

        internal static bool ToBool(object value)
        {
            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                return false;
            }
        }

        internal static int ToInt32(string value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }

        internal static long ToInt64(string value)
        {
            try
            {
                return Convert.ToInt64(value);
            }
            catch
            {
                return 0;
            }
        }

        internal static string ToString(object value)
        {
            try
            {
                return Convert.ToString(value).Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        internal static DateTime ToDateTime(object value)
        {
            try
            {
                return Convert.ToDateTime(value, CultureInfo.GetCultureInfo("pt-BR"));
            }
            catch
            {
                return new DateTime(1, 1, 1);
            }
        }

        public static T ToEnum<T>(string value, bool ignoreCase, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            if (Enum.TryParse(value, ignoreCase, out T result))
                return result;

            return defaultValue;
        }

        /// <summary>
        /// Formata o CPF ou CNPJ do Beneficiario ou do Pagador no formato: 000.000.000-00, 00.000.000/0001-00 respectivamente.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string FormataCPFCPPJ(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            switch (value.Trim().Length)
            {
                case 11:
                    return FormataCPF(value);
                case 14:
                    return FormataCNPJ(value);
                default:
                    return value;
            }
        }

        /// <summary>
        /// Formata o número do CPF 92074286520 para 920.742.865-20
        /// </summary>
        /// <param name="cpf">Sequencia numérica de 11 dígitos. Exemplo: 00000000000</param>
        /// <returns>CPF formatado</returns>
        internal static string FormataCPF(string cpf)
        {
            try
            {
                return cpf != null && cpf.Length == 11 ? $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}" : cpf;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Formata o CNPJ. Exemplo 00.316.449/0001-63
        /// </summary>
        /// <param name="cnpj">Sequencia numérica de 14 dígitos. Exemplo: 00000000000000</param>
        /// <returns>CNPJ formatado</returns>
        internal static string FormataCNPJ(string cnpj)
        {
            try
            {
                return cnpj != null && cnpj.Length == 14 ? $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}" : cnpj;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Formato o CEP em 00000-000
        /// </summary>
        /// <param name="cep">Sequencia numérica de 8 dígitos. Exemplo: 00000000</param>
        /// <returns>CEP formatado</returns>
        internal static string FormataCEP(string cep)
        {
            try
            {
                return cep != null && cep.Length == 8 ? $"{cep.Substring(0, 2)}{cep.Substring(2, 3)}-{cep.Substring(5, 3)}" : cep;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Formata o campo de acordo com o tipo e o tamanho 
        /// </summary>        
        public static string FitStringLength(this string sringToBeFit, int maxLength, char fitChar)
            => sringToBeFit.Length > maxLength ? sringToBeFit.Substring(0, maxLength) : sringToBeFit.PadLeft(maxLength, fitChar);

        public static string SubstituiCaracteresEspeciais(string strline)
        {
            try
            {
                strline = strline.Replace("ã", "a");
                strline = strline.Replace('Ã', 'A');
                strline = strline.Replace('â', 'a');
                strline = strline.Replace('Â', 'A');
                strline = strline.Replace('á', 'a');
                strline = strline.Replace('Á', 'A');
                strline = strline.Replace('à', 'a');
                strline = strline.Replace('À', 'A');
                strline = strline.Replace('ç', 'c');
                strline = strline.Replace('Ç', 'C');
                strline = strline.Replace('é', 'e');
                strline = strline.Replace('É', 'E');
                strline = strline.Replace('Ê', 'E');
                strline = strline.Replace('ê', 'e');
                strline = strline.Replace('õ', 'o');
                strline = strline.Replace('Õ', 'O');
                strline = strline.Replace('ó', 'o');
                strline = strline.Replace('Ó', 'O');
                strline = strline.Replace('ô', 'o');
                strline = strline.Replace('Ô', 'O');
                strline = strline.Replace('ú', 'u');
                strline = strline.Replace('Ú', 'U');
                strline = strline.Replace('ü', 'u');
                strline = strline.Replace('Ü', 'U');
                strline = strline.Replace('í', 'i');
                strline = strline.Replace('Í', 'I');
                strline = strline.Replace('ª', 'a');
                strline = strline.Replace('º', 'o');
                strline = strline.Replace('°', 'o');
                strline = strline.Replace('&', 'e');
                return strline;
            }
            catch (Exception ex)
            {
                Exception tmpEx = new Exception("Erro ao formatar string.", ex);
                throw tmpEx;
            }
        }

        /// <summary>
        /// Converte uma imagem em array de bytes.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ConvertImageToByte(SKImage image)
        {
            if (image == null)
            {
                return null;
            }

            if (image.GetType().ToString() == "System.Drawing.Bitmap")
            {
                using (SKData encoded = image.Encode(SKEncodedImageFormat.Jpeg, 100))
                {
                    return encoded.ToArray();
                }
            }
            else
            {
                throw new NotImplementedException("ConvertImageToByte invalid type " + image.GetType().ToString());
            }
        }

        internal static SKBitmap DrawText(string text, float textSizeFont, SKTypeface font, SKColor textColor, SKColor backColor)
        {
            //first, create a dummy bitmap just to get a graphics object
            var img = new SKBitmap(1, 1);
            using (var textPaint = new SKPaint(font.ToFont(textSizeFont)))
            {
                //measure the string to see how big the image needs to be
                var bounds = new SKRect();
                textPaint.MeasureText(text, ref bounds);

                //create a new image of the right size
                //new Bitmap((int)textSize.Width - Convert.ToInt32(font.Size * 1.5), (int)textSize.Height, PixelFormat.Format24bppRgb);
                img.Dispose();
                img = new SKBitmap((int)bounds.Right, (int)bounds.Height);

                using (var canvas = new SKCanvas(img))
                {
                    canvas.Clear(backColor);
                    canvas.DrawText(text, 0, -bounds.Top, textPaint);
                    canvas.Save();
                }
            }
            return img;
        }
    }
}
