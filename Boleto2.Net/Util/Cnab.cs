using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boleto2Net
{
    public static class Cnab
    {
        // Considerando o arquivo CNAB padrão 240, acredita-se que os códigos de retorno são os mesmos.
        // Neste caso, utilizar sempre essa classe, para evitar duplicar esse código em cada banco implementado.
        public static string OcorrenciaCnab240(string codigo)
        {
            switch (codigo)
            {
                case "01":
                    return "Solicitação de Impressão de Títulos Confirmada";
                case "02":
                    return "Entrada Confirmada";
                case "03":
                    return "Entrada Rejeitada";
                case "04":
                    return "Transferência de Carteira/Entrada";
                case "05":
                    return "Transferência de Carteira/Baixa";
                case "06":
                    return "Liquidação";
                case "07":
                    return "Confirmação do Recebimento da Instrução de Desconto";
                case "08":
                    return "Confirmação do Recebimento do Cancelamento do Desconto";
                case "09":
                    return "Baixa";
                case "12":
                    return "Confirmação Recebimento Instrução de Abatimento";
                case "13":
                    return "Confirmação Recebimento Instrução de Cancelamento Abatimento";
                case "14":
                    return "Confirmação Recebimento Instrução Alteração de Vencimento";
                case "17":
                    return "Liquidação Após Baixa ou Liquidação Título Não Registrado";
                case "19":
                    return "Confirmação Recebimento Instrução de Protesto";
                case "20":
                    return "Confirmação Recebimento Instrução Sustação de Protesto";
                case "23":
                    return "Remessa a Cartório";
                case "24":
                    return "Retirada de Cartório";
                case "25":
                    return "Protestado e Baixado (Baixa por Ter Sido Protestado)";
                case "26":
                    return "Instrução Rejeitada";
                case "27":
                    return "Confrmação do Pedido de Alteração de Outros Dados";
                case "28":
                    return "Débito de Tarifas/Custas";
                case "30":
                    return "Alteração de Dados Rejeitada";
                case "33":
                    return "Confirmação da Alteração dos Dados do Rateio de Crédito";
                case "34":
                    return "Confirmação do Cancelamento dos Dados do Rateio de Crédito";
                case "35":
                    return "Confirmação de Inclusão Banco de Pagador";
                case "36":
                    return "Confirmação de Alteração Banco de Pagador";
                case "37":
                    return "Confirmação de Exclusão Banco de Pagador";
                case "38":
                    return "Emissão de Boletos de Banco de Pagador";
                case "39":
                    return "Manutenção de Pagador Rejeitada";
                case "40":
                    return "Entrada de Título via Banco de Pagador Rejeitada";
                case "41":
                    return "Manutenção de Banco de Pagador Rejeitada";
                case "44":
                    return "Estorno de Baixa / Liquidação";
                case "45":
                    return "Alteração de Dados";
                default:
                    return "";
            }
        }
    }
}
