using System;
using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using static System.String;

namespace BoletoNetCore
{
    partial class BancoItau : IBancoOnlineRest
    {
        /// <summary>
        /// TODO: Necessário verificar quais os métodos necessários
        /// </summary>
        /// <returns></returns>
        public string GerarToken()
        {
            throw new NotImplementedException();
        }

        public void RegistrarBoleto(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException();
        }
    }
}


