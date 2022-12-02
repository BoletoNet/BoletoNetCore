using BoletoNetCore.Exceptions;
using System;
using System.Collections.Generic;

namespace BoletoNetCore
{
    partial class BancoFebraban<T>
    {
        ///
        /// Como o retorno CNAB400 não tem todos os dados na linha do header
        /// Utiliza o primeiro registro de titulo para pegar mais dados do beneficiario
        ///
        public virtual void CompletarHeaderRetornoCNAB400(string registro)
        {
            //implementar cada um especificamente! Pois muda o modelo de cada banco...
        }

        public virtual void LerHeaderRetornoCNAB400(string registro)
        {
            try
            {
                if (registro.Substring(0, 9) != "02RETORNO")
                    throw new Exception("O arquivo não é do tipo \"02RETORNO\"");

                this.Beneficiario = new Beneficiario();
                this.Beneficiario.Codigo = registro.Substring(26, 20);
                this.Beneficiario.Nome = registro.Substring(46, 30);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler HEADER do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

    }
}