using System;
using System.Collections.Generic;
using System.Text;

namespace BoletoNetCore.QuestPdf
{
    internal static class StringHelper
    {
        public static string ToDateStr(this DateTime data)
        {
            return data.ToString("dd/MM/yyyy");
        }

        public static bool IsEmpty(this String str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string StrNumber(this string str)
        {
            return str == null ? string.Empty : string.Join("", System.Text.RegularExpressions.Regex.Split(str ?? "", @"[^\d]"));
        }

        public static bool IsCpfOrCnpj(this string cpfcnpj)
        {
            return (cpfcnpj.IsCpf() || cpfcnpj.IsCnpj());
        }

        public static string FormatarCep(this string cep)
        {
            cep = cep.StrNumber();
            if (cep.Length != 8)
                return cep;

            return $"{cep.Substring(0, 2)}{cep.Substring(2, 3)}-{cep.Substring(5, 3)}";
        }

        public static string MascararCpfCnpj(this string cpf)
        {
            cpf = cpf.StrNumber();
            if (cpf.IsCnpj())
                return $"{cpf.Substring(0, 2)}.{cpf.Substring(2, 3)}.{cpf.Substring(5, 3)}/{cpf.Substring(8, 4)}-{cpf.Substring(12, 2)}";
            else if (cpf.IsCpf())
                return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
            else
                return cpf;
        }

        public static bool IsCnpj(this string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj.StrNumber()))
                return false;

            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;
            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cnpj.EndsWith(digito);
        }

        public static bool IsCpf(this string cpf)
        {
            if (cpf.IsEmpty())
                return false;

            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;
            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;
            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }
    }
}
