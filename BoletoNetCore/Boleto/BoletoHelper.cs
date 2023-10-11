using System;
using System.Collections.Generic;
using System.IO;

namespace BoletoNetCore
{
    /// <summary>
    /// Classe Auxiliar com métodos de extensão para executar comandos a partir de um Boleto ou de uma lista de boletos
    /// </summary>
    public static class BoletoHelper
    {
        /// <summary>
        /// Recupera um arquivo HTML com encoding do banco
        /// </summary>
        /// <param name="boleto"></param>
        /// <returns></returns>
        public static byte[] ImprimirHtml(this Boleto boleto)
        {
            throw new NotImplementedException();
        }

        public static Stream GerarRemessa(this Boletos boletos, int numArquivoRemessa, TipoArquivo tipoArquivo = TipoArquivo.CNAB240 )
        {
            ArquivoRemessa rem = new ArquivoRemessa(boletos.Banco, tipoArquivo, numArquivoRemessa);
            MemoryStream ms = new MemoryStream(2048);
            rem.GerarArquivoRemessa(boletos, ms, false);
            ms.Position = 0;
            return ms as Stream;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boletos"></param>
        /// <returns></returns>
        public static byte[] ImprimirLoteHtml(this Boletos boletos)
        {
            throw new NotImplementedException();
        }

    }
}
