using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BoletoNetCore.Exceptions;
using static System.String;

namespace BoletoNetCore
{
    partial class BancoItau : IBancoOnlineRest
    {
        public string ChaveApi { get; set; }
        public string Token { get; set; }

        public Task ConsultarStatus(Boleto boleto)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO: Necessário verificar quais os métodos necessários
        /// </summary>
        /// <returns></returns>
        public Task<string> GerarToken()
        {
            throw new NotImplementedException();
        }

        public Task RegistrarBoleto(Boleto boleto)
        {
            throw new NotImplementedException();
        }
    }
}


