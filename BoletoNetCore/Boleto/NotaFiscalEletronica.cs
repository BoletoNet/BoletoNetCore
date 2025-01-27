using System;

namespace BoletoNetCore
{
    //Essa classe vai receber os dados da nota fiscal eletrônica do boleto em questão
    //exigência do Banco Daycoval
    public class NotaFiscalEletronica
    {
        public string Numero { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataEmissao { get; set; }
        public string ChaveAcesso { get; set; }

    }
}
