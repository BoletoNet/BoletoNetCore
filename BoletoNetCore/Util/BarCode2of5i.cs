using System;
using SkiaSharp;

namespace BoletoNetCore
{
    public class BarCode2of5i : BarCodeBase
    {
        #region variables
        private readonly string[] _cPattern = new string[100];
        private const string START = "0000";
        private const string STOP = "1000";
        private SKBitmap _bitmap;
        private SKCanvas _canvas;
        #endregion

        #region Constructor
        public BarCode2of5i()
        {
        }

        /// <summary>
        /// Code 2 of 5 intrelaced Constructor
        /// </summary>
        /// <param name="code">The string that contents the numeric code</param>
        /// <param name="barWidth">The Width of each bar</param>
        /// <param name="height">The Height of each bar</param>
        public BarCode2of5i(string code, int barWidth, int height)
        {
            Code = code;
            Height = height;
            Width = barWidth;
        }
        /// <summary>
        /// Code 2 of 5 intrelaced Constructor
        /// </summary>
        /// <param name="code">The string that contents the numeric code</param>
        /// <param name="barWidth">The Width of each bar</param>
        /// <param name="height">The Height of each bar</param>
        /// <param name="digits">Number of digits of code</param>
        public BarCode2of5i(string code, int barWidth, int height, int digits)
        {
            Code = code;
            Height = height;
            Width = barWidth;
            Digits = digits;
        }
        #endregion

        private void FillPatern()
        {
            int f;
            string strTemp;

            if (_cPattern[0] == null)
            {
                _cPattern[0] = "00110";
                _cPattern[1] = "10001";
                _cPattern[2] = "01001";
                _cPattern[3] = "11000";
                _cPattern[4] = "00101";
                _cPattern[5] = "10100";
                _cPattern[6] = "01100";
                _cPattern[7] = "00011";
                _cPattern[8] = "10010";
                _cPattern[9] = "01010";
                //Create a draw pattern for each char from 0 to 99
                for (int f1 = 9; f1 >= 0; f1--)
                {
                    for (int f2 = 9; f2 >= 0; f2--)
                    {
                        f = f1 * 10 + f2;
                        var builder = new System.Text.StringBuilder();
                        for (int i = 0; i < 5; i++)
                        {
                            builder.Append(_cPattern[f1][i] + _cPattern[f2][i].ToString());
                        }
                        strTemp = builder.ToString();
                        _cPattern[f] = strTemp;
                    }
                }
            }
        }

        /// <summary>
        /// Generate the Bitmap of Barcode.
        /// </summary>
        /// <returns>Return Bitmap Image</returns>
        public SKBitmap ToBitmap()
        {
            XPos = 0;
            YPos = 0;

            if (Digits == 0)
            {
                Digits = Code.Length;
            }

            if (Digits % 2 > 0) Digits++;

            while (Code.Length < Digits || Code.Length % 2 > 0)
            {
                Code = "0" + Code;
            }

            int width = (2 * Full + 3 * Thin) * (Digits) + 7 * Thin + Full;

            _bitmap = new SKBitmap(width, Height);
            _canvas = new SKCanvas(_bitmap);

            //Start Pattern
            DrawPattern(ref _canvas, START);

            //Draw code
            FillPatern();
            while (Code.Length > 0)
            {
                var i = Convert.ToInt32(Code.Substring(0, 2));
                if (Code.Length > 2)
                    Code = Code.Substring(2, Code.Length - 2);
                else
                    Code = "";
                DrawPattern(ref _canvas, _cPattern[i]);
            }

            //Stop Patern
            DrawPattern(ref _canvas, STOP);

            return _bitmap;
        }

        /// <summary>
        /// Returns the byte array of Barcode
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] ToByte()
        {
            using (SKData encoded = ToBitmap().Encode(SKEncodedImageFormat.Jpeg, 100))
            {
                return encoded.ToArray();
            }
        }
    }
}
