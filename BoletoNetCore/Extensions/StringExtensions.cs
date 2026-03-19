using System;

namespace BoletoNetCore.Extensions
{
    public static class StringExtensions
    {
        public static string Right(this string value, int length)
        {
            if (String.IsNullOrEmpty(value))
                return string.Empty;
            return value.Length <= length ? value : value.AsSpan(value.Length - length).ToString();
        }

        internal static ReadOnlySpan<char> RightSpan(this string value, int length)
        {
            if (string.IsNullOrEmpty(value))
                return ReadOnlySpan<char>.Empty;
            return value.Length <= length ? value.AsSpan() : value.AsSpan(value.Length - length);
        }
        public static string Left(this string value, int length)
        {
            if (String.IsNullOrEmpty(value))
                return string.Empty;
            return value.Length <= length ? value : value.AsSpan(0, length).ToString();
        }

        internal static ReadOnlySpan<char> LeftSpan(this string value, int length)
        {
            if (string.IsNullOrEmpty(value))
                return ReadOnlySpan<char>.Empty;
            return value.Length <= length ? value.AsSpan() : value.AsSpan(0, length);
        }

        public static string MidVB(this string str, int start, int length)
        {
            return str.Mid(--start,length);
        }

        public static string Mid(this string str, int startIndex, int length)
        {
            if (str.Length <= 0 || startIndex >= str.Length) return string.Empty;
            if (startIndex + length > str.Length)
            {
                length = str.Length - startIndex;
            }
            return str.AsSpan(startIndex, length).ToString();
        }

        internal static ReadOnlySpan<char> MidSpan(this string str, int startIndex, int length)
        {
            if (str.Length <= 0 || startIndex >= str.Length) return ReadOnlySpan<char>.Empty;
            if (startIndex + length > str.Length)
            {
                length = str.Length - startIndex;
            }
            return str.AsSpan(startIndex, length);
        }

        public static string CalcularDVCaixa(this string texto)
        {
            return CalcularDVCaixa(texto.AsSpan());
        }

        public static string CalcularDVCaixa(this ReadOnlySpan<char> texto)
        {
            int pesoMaximo = 9, soma = 0, peso = 2;
            for (var i = texto.Length - 1; i >= 0; i--)
            {
                int digito = texto[i] - '0';
                soma += digito * peso;
                peso = peso == pesoMaximo ? 2 : peso + 1;
            }
            var resto = soma % 11;
            return resto <= 1 ? "0" : (11 - resto).ToString();
        }

        public static string CalcularDVSantander(this string texto)
        {
            return CalcularDVSantander(texto.AsSpan());
        }

        public static string CalcularDVSantander(this ReadOnlySpan<char> texto)
        {
            int pesoMaximo = 9, soma = 0, peso = 2;
            for (var i = texto.Length - 1; i >= 0; i--)
            {
                int digito = texto[i] - '0';
                soma += digito * peso;
                peso = peso == pesoMaximo ? 2 : peso + 1;
            }
            var resto = soma % 11;
            return resto <= 1 ? "0" : (11 - resto).ToString();
        }

        private static ReadOnlySpan<byte> FatorMultiplicacaoSicoob => new byte[] 
            { 3, 1, 9, 7, 3, 1, 9, 7, 3, 1, 9, 7, 3, 1, 9, 7, 3, 1, 9, 7, 3 };

        public static string CalcularDVSicoob(this string texto)
        {
            return CalcularDVSicoob(texto.AsSpan());
        }

        public static string CalcularDVSicoob(this ReadOnlySpan<char> texto)
        {
            if (texto.Length != 21)
                throw new ArgumentException("Texto deve ter 21 caracteres", nameof(texto));
            
            ReadOnlySpan<byte> fatores = FatorMultiplicacaoSicoob;
            int soma = 0;
            
            for (int i = 0; i < 21; i++)
            {
                soma += (texto[i] - '0') * fatores[i];
            }
            
            int resto = soma % 11;
            return resto <= 1 ? "0" : (11 - resto).ToString();
        }

        public static string CalcularDVBradesco(this string texto)
        {
            return CalcularDVBradesco(texto.AsSpan());
        }

        public static string CalcularDVBradesco(this ReadOnlySpan<char> texto)
        {
            int pesoMaximo = 7, soma = 0, peso = 2;
            for (var i = texto.Length - 1; i >= 0; i--)
            {
                int digito = texto[i] - '0';
                soma += digito * peso;
                peso = peso == pesoMaximo ? 2 : peso + 1;
            }
            var resto = soma % 11;
            switch (resto)
            {
                case 0:
                    return "0";
                case 1:
                    return "P";
                default:
                    return (11 - resto).ToString();
            }
        }

        public static string CalcularDVItau(this string texto)
        {
            return CalcularDVItau(texto.AsSpan());
        }

        public static string CalcularDVItau(this ReadOnlySpan<char> texto)
        {
            int soma = 0, peso = 2;
            for (var i = texto.Length - 1; i >= 0; i--)
            {
                int digito = texto[i] - '0';
                int digTmp = digito * peso;
                if (digTmp > 9)
                    digTmp = (digTmp / 10) + (digTmp % 10);

                soma += digTmp;
                peso = peso == 2 ? 1 : peso + 1;
            }
            var resto = soma % 10;
            return resto == 0 ? "0" : (10 - resto).ToString();
        }

        public static string CalcularDVSafra(this string texto)
        {
            return CalcularDVSafra(texto.AsSpan());
        }

        public static string CalcularDVSafra(this ReadOnlySpan<char> texto)
        {
            int pesoMaximo = 9, soma = 0, peso = 2;
            for (var i = texto.Length - 1; i >= 0; i--)
            {
                int digito = texto[i] - '0';
                soma += digito * peso;
                peso = peso == pesoMaximo ? 2 : peso + 1;
            }
            var resto = soma % 11;
            switch (resto)
            {
                case 0:
                    return "1";
                case 1:
                    return "0";
                default:
                    return (11 - resto).ToString();
            }
        }

        public static string CalcularDVUniprimeNortePR(this string texto)
        {
            return CalcularDVUniprimeNortePR(texto.AsSpan());
        }

        public static string CalcularDVUniprimeNortePR(this ReadOnlySpan<char> texto)
        {
            int pesoMaximo = 7, soma = 0, peso = 2;
            for (var i = texto.Length - 1; i >= 0; i--)
            {
                int digito = texto[i] - '0';
                soma += digito * peso;
                peso = peso == pesoMaximo ? 2 : peso + 1;
            }
            var moduloFinal = soma % 11;
            
            if (moduloFinal == 1)
                return "P";

            int digitoFinal = moduloFinal < 2 ? 0 : 11 - moduloFinal;
            return digitoFinal.ToString();
        }

        public static string CalcularDVBancoInter(this string texto)
        {
            return CalcularDVBancoInter(texto.AsSpan());
        }

        public static string CalcularDVBancoInter(this ReadOnlySpan<char> texto)
        {
            int soma = 0;
            for (var i = 0; i < texto.Length; i++)
            {
                int numero = texto[i] - '0';
                int peso = i % 2 == 0 ? 2 : 1;
                int parcial = numero * peso;
                
                if (parcial < 10)
                {
                    soma += parcial;
                }
                else
                {
                    // Soma os dígitos do resultado (ex: 14 -> 1 + 4)
                    soma += (parcial / 10) + (parcial % 10);
                }
            }
            int moduloFinal = soma % 10;
            int digitoFinal = moduloFinal > 0 ? 10 - moduloFinal : 0;
            return digitoFinal.ToString();
        }
        
        public static string CalcularDVBancoBTGPactual(this string nossoNumero)
        {
            return CalcularDVBancoBTGPactual(nossoNumero.AsSpan());
        }

        public static string CalcularDVBancoBTGPactual(this ReadOnlySpan<char> nossoNumero)
        {
            int soma = 0;
            int multiplicador = 2;

            // Percorrer o número de trás para frente
            for (int i = nossoNumero.Length - 1; i >= 0; i--)
            {
                // Multiplicar cada dígito pela sequência crescente de 2 a 9
                int numero = nossoNumero[i] - '0';
                soma += numero * multiplicador;

                // Atualizar o multiplicador (2 a 9 e depois volta para 2)
                multiplicador++;
                if (multiplicador > 9)
                {
                    multiplicador = 2;
                }
            }

            // Aplicar o Módulo 11
            int resto = soma % 11;
            int digitoVerificador = (resto == 0 || resto == 1 || resto == 10) ? 1 : 11 - resto;

            return digitoVerificador.ToString();
        }
        public static string CalcularDVDaycoval(this string texto)
        {
            return CalcularDVDaycoval(texto.AsSpan());
        }

        public static string CalcularDVDaycoval(this ReadOnlySpan<char> texto)
        {
            int soma = 0, peso = 2;
            for (var i = texto.Length - 1; i >= 0; i--)
            {
                int digito = texto[i] - '0';
                int digTmp = digito * peso;
                if (digTmp > 9)
                    digTmp = (digTmp / 10) + (digTmp % 10);

                soma += digTmp;
                peso = peso == 2 ? 1 : peso + 1;
            }
            var resto = soma % 10;
            return resto == 0 ? "0" : (10 - resto).ToString();
        }

    }


}