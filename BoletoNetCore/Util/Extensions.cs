using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Threading;

namespace BoletoNetCore.Util
{
    public static class Extensions
    {
        public static string Left(this string str, int length)
        {
            str = (str ?? string.Empty);
            return str.Substring(0, Math.Min(length, str.Length));
        }
        public static string Right(this string str, int length)
        {
            str = (str ?? string.Empty);
            return (str.Length >= length)
                ? str.Substring(str.Length - length, length)
                : str;
        }

        public static string OnlyNumber(this string str)
        {
            return str == null ? string.Empty : string.Join("", System.Text.RegularExpressions.Regex.Split(str ?? "", @"[^\d]"));
        }
    }
}
