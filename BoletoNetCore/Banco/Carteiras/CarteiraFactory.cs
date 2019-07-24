using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BoletoNetCore.Exceptions;

namespace BoletoNetCore
{
    internal static class CarteiraFactory<T>
        where T : IBanco
    {
        private static readonly Dictionary<string, Lazy<ICarteira<T>>> Carteiras;
        private static readonly Type CarteiraType = typeof(ICarteira<T>);

        static CarteiraFactory()
        {
            const string propName = nameof(BancoBrasilCarteira11.Instance);
            string[] ObterCodigos(Type type) => type.GetCustomAttributes(false).OfType<CarteiraCodigoAttribute>().First().Codigos;
            Lazy<ICarteira<T>> ObterInstancia(Type type) => (Lazy<ICarteira<T>>)type.GetProperty(propName, BindingFlags.NonPublic | BindingFlags.Static).GetValue(null, null);

            Carteiras = typeof(CarteiraFactory<>).Assembly.GetTypes()
                .Where(type => CarteiraType.IsAssignableFrom(type))
                .SelectMany(ObterCodigos, (cod, tipo) => Tuple.Create(tipo, cod))
                .ToDictionary(t => t.Item1, t => ObterInstancia(t.Item2));
        }

        internal static ICarteira<T> ObterCarteira(string identificacao)
            => Carteiras.ContainsKey(identificacao) ? Carteiras[identificacao].Value : throw BoletoNetCoreException.CarteiraNaoImplementada(identificacao);

        public static bool CarteiraEstaImplementada(string identificacao)
            => Carteiras.ContainsKey(identificacao);
    }
}
