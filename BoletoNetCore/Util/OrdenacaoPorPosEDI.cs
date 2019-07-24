using System.Collections.Generic;

namespace BoletoNetCore
{
    //Classes básicas para manipulação de registros para geração/interpretação de EDI

    /// <summary>
    /// Classe para ordenação pela propriedade Posição no Registro EDI
    /// </summary>
    internal class OrdenacaoPorPosEDI : IComparer<TCampoRegistroEDI>
    {
        public int Compare(TCampoRegistroEDI x, TCampoRegistroEDI y)
        {
            return x.OrdemNoRegistroEDI.CompareTo(y.OrdemNoRegistroEDI);
        }
    }
}
