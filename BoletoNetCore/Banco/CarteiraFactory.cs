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

        #region #173339 - Remover no Pull Request (se aprovado)

        /// <summary>
        /// Retorna um array de carteiras implementadas para este banco.
        /// </summary>
        /// <param name="predicate">
        /// Função opcional para filtrar as carteiras retornadas.
        /// Se não for informada, retorna todas as carteiras implementadas.
        /// </param>
        /// <returns>
        /// Array de strings contendo os códigos das carteiras implementadas.
        /// </returns>
        internal static IEnumerable<(string Carteira, string VariacaoCarteira)> ObterCarteiras(Func<(string Carteira, string VariacaoCarteira), bool> predicate = null)
        {
            predicate = predicate ?? (s => true); // Se não for informado, assume que todas as carteiras são válidas.

            foreach(var carteira in Carteiras.Select(s => transform(s.Key)).Where(w => predicate(w)))
            {
                yield return carteira;
            }

            (string Carteira, string VariacaoCarteira) transform(string carteira)
            {
                var split = carteira.Split('/');
                return split.Length == 1 ? (split[0], string.Empty) : (split[0], split[1]);
            }

        }

        #endregion
    }
}
