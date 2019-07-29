using System;

namespace BoletoNetCore
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class CarteiraCodigoAttribute : Attribute
    {
        internal CarteiraCodigoAttribute(params string[] codigo)
        {
            Codigos = codigo;
        }

        internal string[] Codigos { get; }
    }
}