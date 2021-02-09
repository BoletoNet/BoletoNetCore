using BoletoNetCore.Exceptions;
using System;
using System.Collections.Generic;

namespace BoletoNetCore
{
    partial class BancoFebraban<T> 
    {
        public virtual void LerHeaderRetornoCNAB400(string registro)
        {
            try
            {
                if (registro.Substring(0, 9) != "02RETORNO")
                    throw new Exception("O arquivo não é do tipo \"02RETORNO\"");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler HEADER do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

    }
}