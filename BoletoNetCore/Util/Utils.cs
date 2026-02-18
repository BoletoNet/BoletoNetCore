using SkiaSharp;
using System;
using System.Globalization;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace BoletoNetCore
{
    public static class Utils
    {
        internal static long DateDiff(DateInterval Interval, System.DateTime StartDate, System.DateTime EndDate)
        {
            long lngDateDiffValue = 0;
            System.TimeSpan TS = new System.TimeSpan(EndDate.Ticks - StartDate.Ticks);
            switch (Interval)
            {
                case DateInterval.Day:
                    lngDateDiffValue = (long)TS.Days;
                    break;
                case DateInterval.Hour:
                    lngDateDiffValue = (long)TS.TotalHours;
                    break;
                case DateInterval.Minute:
                    lngDateDiffValue = (long)TS.TotalMinutes;
                    break;
                case DateInterval.Month:
                    lngDateDiffValue = (long)(TS.Days / 30);
                    break;
                case DateInterval.Quarter:
                    lngDateDiffValue = (long)((TS.Days / 30) / 3);
                    break;
                case DateInterval.Second:
                    lngDateDiffValue = (long)TS.TotalSeconds;
                    break;
                case DateInterval.Week:
                    lngDateDiffValue = (long)(TS.Days / 7);
                    break;
                case DateInterval.Year:
                    lngDateDiffValue = (long)(TS.Days / 365);
                    break;
            }
            return (lngDateDiffValue);
        }

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
        /// Formata o n�mero do CPF 92074286520 para 920.742.865-20
        /// </summary>
        /// <param name="cpf">Sequencia num�rica de 11 d�gitos. Exemplo: 00000000000</param>
        /// <returns>CPF formatado</returns>
        internal static string FormataCPF(string cpf)
        {
            try
            {
                if (cpf == null || cpf.Length != 11)
                    return cpf;
                
                // Otimizado: usa array ao invés de múltiplos Substring
                char[] result = new char[14];
                cpf.CopyTo(0, result, 0, 3);      // 000
                result[3] = '.';
                cpf.CopyTo(3, result, 4, 3);      // 000
                result[7] = '.';
                cpf.CopyTo(6, result, 8, 3);      // 000
                result[11] = '-';
                cpf.CopyTo(9, result, 12, 2);     // 00
                
                return new string(result);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Formata o CNPJ. Exemplo 00.316.449/0001-63
        /// </summary>
        /// <param name="cnpj">Sequencia num�rica de 14 d�gitos. Exemplo: 00000000000000</param>
        /// <returns>CNPJ formatado</returns>
        internal static string FormataCNPJ(string cnpj)
        {
            try
            {
                if (cnpj == null || cnpj.Length != 14)
                    return cnpj;
                
                // Otimizado: usa array ao invés de múltiplos Substring
                char[] result = new char[18];
                cnpj.CopyTo(0, result, 0, 2);         // 00
                result[2] = '.';
                cnpj.CopyTo(2, result, 3, 3);         // 000
                result[6] = '.';
                cnpj.CopyTo(5, result, 7, 3);         // 000
                result[10] = '/';
                cnpj.CopyTo(8, result, 11, 4);        // 0000
                result[15] = '-';
                cnpj.CopyTo(12, result, 16, 2);       // 00
                
                return new string(result);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Formato o CEP em 00000-000
        /// </summary>
        /// <param name="cep">Sequencia num�rica de 8 d�gitos. Exemplo: 00000000</param>
        /// <returns>CEP formatado</returns>
        internal static string FormataCEP(string cep)
        {
            try
            {
                if (cep == null || cep.Length != 8)
                    return cep;
                
                // Otimizado: usa array ao invés de múltiplos Substring
                char[] result = new char[9];
                cep.CopyTo(0, result, 0, 5);         // 00000
                result[5] = '-';
                cep.CopyTo(5, result, 6, 3);         // 000
                
                return new string(result);
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
                strline = strline.Replace("�", "a");
                strline = strline.Replace('�', 'A');
                strline = strline.Replace('�', 'a');
                strline = strline.Replace('�', 'A');
                strline = strline.Replace('�', 'a');
                strline = strline.Replace('�', 'A');
                strline = strline.Replace('�', 'a');
                strline = strline.Replace('�', 'A');
                strline = strline.Replace('�', 'c');
                strline = strline.Replace('�', 'C');
                strline = strline.Replace('�', 'e');
                strline = strline.Replace('�', 'E');
                strline = strline.Replace('�', 'E');
                strline = strline.Replace('�', 'e');
                strline = strline.Replace('�', 'o');
                strline = strline.Replace('�', 'O');
                strline = strline.Replace('�', 'o');
                strline = strline.Replace('�', 'O');
                strline = strline.Replace('�', 'o');
                strline = strline.Replace('�', 'O');
                strline = strline.Replace('�', 'u');
                strline = strline.Replace('�', 'U');
                strline = strline.Replace('�', 'u');
                strline = strline.Replace('�', 'U');
                strline = strline.Replace('�', 'i');
                strline = strline.Replace('�', 'I');
                strline = strline.Replace('�', 'a');
                strline = strline.Replace('�', 'o');
                strline = strline.Replace('�', 'o');
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
        public static byte[] ConvertImageToByte(SKBitmap image)
        {
            if (image == null)
            {
                return null;
            }

            if (image.GetType().ToString() == "SkiaSharp.SKBitmap")
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

        public static SKBitmap DrawText(string text, float textSizeFont, SKTypeface font, SKColor textColor, SKColor backColor)
        {
            const int pixelsAdicionalAltura = 12;//folga de altura (dividido em acima e abaixo [Ex: 12/2 = 6 em cima, 6 em baixo])

            SKBitmap img;
            using (var textPaint = new SKPaint(font.ToFont(textSizeFont)))
            {
                textPaint.Color = textColor;//cor texto
                textPaint.IsAntialias = true;//melhora nas bordas da imagem (mais bonito)

                //verifica a altura e largura do texto
                var bounds = new SKRect();
                textPaint.MeasureText(text, ref bounds);

                //cria uma imagem com altura e largura do texto corretos + folga de altura(acima e abaixo)
                img = new SKBitmap((int)bounds.Right, (int)bounds.Height + pixelsAdicionalAltura);

                //gera um canvas para desenhar
                using (var canvas = new SKCanvas(img))
                {
                    canvas.Clear(backColor);//define cor de tras
                    canvas.DrawText(text, 0, -bounds.Top + (pixelsAdicionalAltura/2), textPaint);//escreve o texto
                    canvas.Save();//salva o canvas (aplicando no img)
                }
            }
            return img;
        }
    }
}
