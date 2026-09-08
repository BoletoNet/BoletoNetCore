using System;
using System.Text;

namespace CpfCnpjComBr.BoletoNetCore
{
    /// <summary>
    /// Normalizacao e validacao de CPF e CNPJ (inclusive CNPJ alfanumerico).
    /// O CNPJ alfanumerico segue a regra da Receita Federal em vigor desde 2026:
    /// 12 posicoes base alfanumericas [0-9A-Z] mais 2 digitos verificadores numericos,
    /// com o valor de cada caractere obtido por (codigo ASCII menos 48).
    /// </summary>
    public static class Documento
    {
        /// <summary>
        /// Remove qualquer separador, deixa apenas [0-9A-Za-z] e converte para maiusculas.
        /// Nunca usar [^0-9] para nao destruir o CNPJ alfanumerico.
        /// </summary>
        public static string Normalizar(string documento)
        {
            if (string.IsNullOrEmpty(documento))
                return string.Empty;

            var sb = new StringBuilder(documento.Length);
            foreach (var c in documento)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z'))
                    sb.Append(c);
                else if (c >= 'a' && c <= 'z')
                    sb.Append(char.ToUpperInvariant(c));
            }
            return sb.ToString();
        }

        /// <summary>Indica se o documento normalizado tem forma de CPF (11 digitos numericos).</summary>
        public static bool EhCpf(string documentoNormalizado)
        {
            if (documentoNormalizado == null || documentoNormalizado.Length != 11)
                return false;
            foreach (var c in documentoNormalizado)
                if (c < '0' || c > '9')
                    return false;
            return true;
        }

        /// <summary>Indica se o documento normalizado tem forma de CNPJ (14 posicoes: 12 alfanumericas + 2 digitos).</summary>
        public static bool EhCnpj(string documentoNormalizado)
        {
            if (documentoNormalizado == null || documentoNormalizado.Length != 14)
                return false;
            for (var i = 0; i < 12; i++)
            {
                var c = documentoNormalizado[i];
                var alfanumerico = (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z');
                if (!alfanumerico)
                    return false;
            }
            return documentoNormalizado[12] >= '0' && documentoNormalizado[12] <= '9'
                && documentoNormalizado[13] >= '0' && documentoNormalizado[13] <= '9';
        }

        /// <summary>Valida o CPF pelos digitos verificadores.</summary>
        public static bool ValidarCpf(string documentoNormalizado)
        {
            if (!EhCpf(documentoNormalizado))
                return false;
            if (TodosCaracteresIguais(documentoNormalizado))
                return false;

            var d1 = CalcularDigitoCpf(documentoNormalizado, 9);
            var d2 = CalcularDigitoCpf(documentoNormalizado, 10);
            return documentoNormalizado[9] - '0' == d1 && documentoNormalizado[10] - '0' == d2;
        }

        /// <summary>Valida o CNPJ (numerico ou alfanumerico) pelos digitos verificadores.</summary>
        public static bool ValidarCnpj(string documentoNormalizado)
        {
            if (!EhCnpj(documentoNormalizado))
                return false;
            if (TodosCaracteresIguais(documentoNormalizado))
                return false;

            var d1 = CalcularDigitoCnpj(documentoNormalizado, 12);
            var d2 = CalcularDigitoCnpj(documentoNormalizado, 13);
            return documentoNormalizado[12] - '0' == d1 && documentoNormalizado[13] - '0' == d2;
        }

        /// <summary>
        /// Normaliza e valida o documento. Retorna o documento limpo (11 ou 14 caracteres),
        /// pronto para ser entregue ao setter de <c>Pagador.CPFCNPJ</c>.
        /// Lanca <see cref="ArgumentException"/> quando o documento nao e um CPF nem um CNPJ valido.
        /// </summary>
        public static string NormalizarEValidar(string documento)
        {
            var limpo = Normalizar(documento);
            if (ValidarCpf(limpo) || ValidarCnpj(limpo))
                return limpo;
            throw new ArgumentException(
                "Documento invalido: informe um CPF (11 digitos) ou CNPJ (14 posicoes) valido.",
                nameof(documento));
        }

        private static int CalcularDigitoCpf(string doc, int tamanhoBase)
        {
            var soma = 0;
            var peso = tamanhoBase + 1;
            for (var i = 0; i < tamanhoBase; i++)
                soma += (doc[i] - '0') * peso--;
            var resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }

        private static int CalcularDigitoCnpj(string doc, int tamanhoBase)
        {
            // Pesos ciclicos de 2 a 9, aplicados da direita para a esquerda da base.
            var soma = 0;
            var peso = 2;
            for (var i = tamanhoBase - 1; i >= 0; i--)
            {
                var valor = doc[i] - '0'; // ASCII menos 48: '0'->0 ... 'Z'->42
                soma += valor * peso;
                peso = peso == 9 ? 2 : peso + 1;
            }
            var resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }

        private static bool TodosCaracteresIguais(string doc)
        {
            for (var i = 1; i < doc.Length; i++)
                if (doc[i] != doc[0])
                    return false;
            return true;
        }
    }
}
