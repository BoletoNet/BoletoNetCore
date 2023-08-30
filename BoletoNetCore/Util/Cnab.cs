using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoletoNetCore
{
    public static class Cnab
    {
        // Considerando o arquivo CNAB padrão 240, acredita-se que os códigos de retorno são os mesmos.
        // Neste caso, utilizar sempre essa classe, para evitar duplicar esse código em cada banco implementado.
        public static string MovimentoRetornoCnab240(string codigo)
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
                case "11":
                    return "Título em Carteira (em ser)";
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
                case "BD":
                    return "Ocorrências para o Retorno";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Recupera a Lista dos Motivos de Ocorrência na Cobrança conforme C047
        /// </summary>
        /// <remarks> Poderão ser
        /// informados até cinco ocorrências distintas, incidente sobre o título
        /// </remarks>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public static IEnumerable<string> MotivoOcorrenciaCnab240(string codigo, string codigoMovimentoRetorno)
        {
            //define qual o domínio que será utilizado, conforme C047
            var funcaoDominio = new string[] { "02", "03", "26", "30" }.Contains(codigoMovimentoRetorno) ? DominioMotivoOcorrencia_A :
                          new string[] { "28" }.Contains(codigoMovimentoRetorno) ? DominioMotivoOcorrencia_B :
                          new string[] { "06", "09", "17" }.Contains(codigoMovimentoRetorno) ? DominioMotivoOcorrencia_C : null;

            //retorna uma lista vazia caso ele não encontre um domínio de motivos de ocorrência
            if (funcaoDominio == null) return Enumerable.Empty<string>();

            //classifica os motivos e inclui na lista de resultados
            List<string> motivos = new List<string>(5);
            for (int posicao = 0; posicao < codigo.Length - 1; posicao = posicao + 2) {
                var entrada = codigo.Substring(posicao, 2);
                if (entrada.Equals("00")) continue;
                motivos.Add(funcaoDominio(entrada)); //inclui o motivo a partir 
            }
            return motivos;
        }

        /// <summary>
        /// Códigos de rejeições de '01' a '95' associados aos códigos de movimento '02', '03',
        /// '26' e '30' (Descrição C044)
        /// </summary>
        public static Func<string, string> DominioMotivoOcorrencia_A { get; set; } =
            (q) =>
            {
                switch (q)
                {
                    case "01": return "Código do Banco Inválido"                                                                              ;
                    case "02": return "Código do Registro Detalhe Inválido"                                                                   ;
                    case "03": return "Código do Segmento Inválido"                                                                           ;
                    case "04": return "Código de Movimento Não Permitido para Carteira"                                                       ;
                    case "05": return "Código de Movimento Inválido"                                                                          ;
                    case "06": return "Tipo / Número de Inscrição do Beneficiário Inválidos"                                                  ;
                    case "07": return "Agência / Conta / DV Inválido"                                                                         ;
                    case "08": return "Nosso Número Inválido"                                                                                 ;
                    case "09": return "Nosso Número Duplicado"                                                                                ;
                    case "10": return "Carteira Inválida"                                                                                     ;
                    case "11": return "Forma de Cadastramento do Título Inválido"                                                             ;
                    case "12": return "Tipo de Documento Inválido"                                                                            ;
                    case "13": return "Identificação da Emissão do Boleto de Pagamento Inválida"                                              ;
                    case "14": return "Identificação da Distribuição do Boleto de Pagamento Inválida"                                         ;
                    case "15": return "Características da Cobrança Incompatíveis"                                                             ;
                    case "16": return "Data de Vencimento Inválida"                                                                           ;
                    case "17": return "Data de Vencimento Anterior a Data de Emissão"                                                         ;
                    case "18": return "Vencimento Fora do Prazo de Operação"                                                                  ;
                    case "19": return "Título a Cargo de Bancos Correspondentes com Vencimento Inferior a XX Dias"                            ;
                    case "20": return "Valor do Título Inválido"                                                                              ;
                    case "21": return "Espécie do Título Inválida"                                                                            ;
                    case "22": return "Espécie do Título Não Permitida para a Carteira"                                                       ;
                    case "23": return "Aceite Inválido"                                                                                       ;
                    case "24": return "Data da Emissão Inválida"                                                                              ;
                    case "25": return "Data da Emissão Posterior a Data de Entrada"                                                           ;
                    case "26": return "Código de Juros de Mora Inválido"                                                                      ;
                    case "27": return "Valor / Taxa de Juros de Mora Inválido"                                                                ;
                    case "28": return "Código do Desconto Inválido"                                                                           ;
                    case "29": return "Valor do Desconto Maior ou Igual ao Valor do Título"                                                   ;
                    case "30": return "Desconto a Conceder Não Confere"                                                                       ;
                    case "31": return "Concessão de Desconto -Já Existe Desconto Anterior"                                                    ;
                    case "32": return "Valor do IOF Inválido"                                                                                 ;
                    case "33": return "Valor do Abatimento Inválido"                                                                          ;
                    case "34": return "Valor do Abatimento Maior ou Igual ao Valor do Título"                                                 ;
                    case "35": return "Valor a Conceder Não Confere"                                                                          ;
                    case "36": return "Concessão de Abatimento -Já Existe Abatimento Anterior"                                                ;
                    case "37": return "Código para Protesto Inválido"                                                                         ;
                    case "38": return "Prazo para Protesto Inválido"                                                                          ;
                    case "39": return "Pedido de Protesto Não Permitido para o Título"                                                        ;
                    case "40": return "Título com Ordem de Protesto Emitida"                                                                  ;
                    case "41": return "Pedido de Cancelamento/ Sustação para Títulos sem Instrução de Protesto"                               ;
                    case "42": return "Código para Baixa/ Devolução Inválido"                                                                 ;
                    case "43": return "Prazo para Baixa/ Devolução Inválido"                                                                  ;
                    case "44": return "Código da Moeda Inválido"                                                                              ;
                    case "45": return "Nome do Pagador Não Informado"                                                                         ;
                    case "46": return "Tipo / Número de Inscrição do Pagador Inválidos"                                                       ;
                    case "47": return "Endereço do Pagador Não Informado"                                                                     ;
                    case "48": return "CEP Inválido"                                                                                        ;
                    case "49": return "CEP Sem Praça de Cobrança(Não Localizado)"                                                             ;
                    case "50": return "CEP Referente a um Banco Correspondente"                                                               ;
                    case "51": return "CEP incompatível com a Unidade da Federação"                                                           ;
                    case "52": return "Unidade da Federação Inválida"                                                                         ;
                    case "53": return "Tipo / Número de Inscrição do Pagador / Avalista Inválidos"                                            ;
                    case "54": return "Pagador / Avalista Não Informado"                                                                      ;
                    case "55": return "Nosso número no Banco Correspondente Não Informado"                                                    ;
                    case "56": return "Código do Banco Correspondente Não Informado"                                                          ;
                    case "57": return "Código da Multa Inválido"                                                                              ;
                    case "58": return "Data da Multa Inválida"                                                                                ;
                    case "59": return "Valor / Percentual da Multa Inválido"                                                                  ;
                    case "60": return "Movimento para Título Não Cadastrado"                                                                  ;
                    case "61": return "Alteração da Agência Cobradora / DV Inválida"                                                          ;
                    case "62": return "Tipo de Impressão Inválido"                                                                            ;
                    case "63": return "Entrada para Título já Cadastrado"                                                                     ;
                    case "64": return "Número da Linha Inválido"                                                                              ;
                    case "65": return "Código do Banco para Débito Inválido"                                                                  ;
                    case "66": return "Agência / Conta / DV para Débito Inválido"                                                             ;
                    case "67": return "Dados para Débito incompatível com a Identificação da Emissão do Boleto de Pagamento"                  ;
                    case "68": return "Débito Automático Agendado"                                                                            ;
                    case "69": return "Débito Não Agendado -Erro nos Dados da Remessa"                                                        ;
                    case "70": return "Débito Não Agendado -Pagador Não Consta do Cadastro de Autorizante"                                    ;
                    case "71": return "Débito Não Agendado -Beneficiário Não Autorizado pelo Pagador"                                         ;
                    case "72": return "Débito Não Agendado -Beneficiário Não Participa da Modalidade Débito Automático"                       ;
                    case "73": return "Débito Não Agendado -Código de Moeda Diferente de Real(R$)"                                            ;
                    case "74": return "Débito Não Agendado -Data Vencimento Inválida"                                                         ;
                    case "75": return "Débito Não Agendado, Conforme seu Pedido, Título Não Registrado"                                       ;
                    case "76": return "Débito Não Agendado, Tipo/ Num.Inscrição do Debitado, Inválido"                                        ;
                    case "77": return "Transferência para Desconto Não Permitida para a Carteira do Título"                                   ;
                    case "78": return "Data Inferior ou Igual ao Vencimento para Débito Automático"                                           ;
                    case "79": return "Data Juros de Mora Inválido"                                                                           ;
                    case "80": return "Data do Desconto Inválida"                                                                             ;
                    case "81": return "Tentativas de Débito Esgotadas - Baixado"                                                              ;
                    case "82": return "Tentativas de Débito Esgotadas - Pendente"                                                             ;
                    case "83": return "Limite Excedido"                                                                                       ;
                    case "84": return "Número Autorização Inexistente"                                                                        ;
                    case "85": return "Título com Pagamento Vinculado"                                                                        ;
                    case "86": return "Seu Número Inválido"                                                                                   ;
                    case "87": return "e-mail / SMS enviado"                                                                                ;
                    case "88": return "e-mail Lido"                                                                                      ;
                    case "89": return "e-mail / SMS devolvido - endereço de e-mail ou número do celular incorreto"                          ;
                    case "90": return "e-mail devolvido - caixa postal cheia"                                                               ;
                    case "91": return "e-mail / número do celular do Pagador não informado"                                                 ;
                    case "92": return "Pagador optante por Boleto de Pagamento Eletrônico -e - mail não enviado"                              ;
                    case "93": return "Código para emissão de Boleto de Pagamento não permite envio de e - mail"                              ;
                    case "94": return "Código da Carteira inválido para envio e-mail."                                                        ;
                    case "95": return "Contrato não permite o envio de e-mail"                                                                ;
                    case "96": return "Número de contrato inválido"                                                                           ;
                    case "97": return "Rejeição da alteração do prazo limite de recebimento(a data deve ser informada no campo 28.3.p)"       ;
                    case "98": return "Rejeição de dispensa de prazo limite de recebimento"                                                   ;
                    case "99": return "Rejeição da alteração do número do título dado pelo Beneficiário"                                      ;
                    case "A1": return "Rejeição da alteração do número controle do participante"                                              ;
                    case "A2": return "Rejeição da alteração dos dados do Pagador"                                                            ;
                    case "A3": return "Rejeição da alteração dos dados do Pagador / avalista"                                                 ;
                    case "A4": return "Pagador DDA"                                                                                        ;
                    case "A5": return "Registro Rejeitado – Título já Liquidado"                                                              ;
                    case "A6": return "Código do Convenente Inválido ou Encerrado"                                                            ;
                    case "A7": return "Título já se encontra na situação Pretendida"                                                          ;
                    case "A8": return "Valor do Abatimento inválido para cancelamento"                                                        ;
                    case "A9": return "Não autoriza pagamento parcial"                                                                        ;
                    case "B1": return "Autoriza recebimento parcial"                                                                          ;
                    case "B2": return "Valor Nominal do Título Conflitante"                                                                   ;
                    case "B3": return "Tipo de Pagamento Inválido"                                                                            ;
                    case "B4": return "Valor Máximo / Percentual Inválido"                                                                    ;
                    case "B5": return "Valor Mínimo / Percentual Inválido"                                                                    ;
                    default: return "";
                }
            };

        /// <summary>
        /// Códigos de tarifas / custas de '01' a '20' associados ao código de movimento '28'
        /// (Descrição C044)
        /// </summary>
        public static Func<string, string> DominioMotivoOcorrencia_B { get; private set; } = (q) =>
        {
            switch (q)
            {
                case "01": return "Tarifa de Extrato de Posição"                                        ;
                case "02": return "Tarifa de Manutenção de Título Vencido"                              ;
                case "03": return "Tarifa de Sustação"                                                  ;
                case "04": return "Tarifa de Protesto"                                                  ;
                case "05": return "Tarifa de Outras Instruções"                                         ;
                case "06": return "Tarifa de Outras Ocorrências"                                        ;
                case "07": return "Tarifa de Envio de Duplicata ao Pagador"                             ;
                case "08": return "Custas de Protesto"                                                  ;
                case "09": return "Custas de Sustação de Protesto"                                      ;
                case "10": return "Custas de Cartório Distribuidor"                                     ;
                case "11": return "Custas de Edital"                                                    ;
                case "12": return "Tarifa Sobre Devolução de Título Vencido"                            ;
                case "13": return "Tarifa Sobre Registro Cobrada na Baixa / Liquidação"                 ;
                case "14": return "Tarifa Sobre Reapresentação Automática"                              ;
                case "15": return "Tarifa Sobre Rateio de Crédito"                                      ;
                case "16": return "Tarifa Sobre Informações Via Fax"                                    ;
                case "17": return "Tarifa Sobre Prorrogação de Vencimento"                              ;
                case "18": return "Tarifa Sobre Alteração de Abatimento/ Desconto"                      ;
                case "19": return "Tarifa Sobre Arquivo mensal(Em Ser)"                                 ;
                case "20": return "Tarifa Sobre Emissão de Boleto de Pagamento Pré - Emitido pelo Banco";
                case "A9": return "Tarifa de manutenção de título vencido"                              ;
                case "B1": return "Tarifa de baixa da carteira"                                         ;
                case "B3": return "Tarifa de registro de entrada do título"                             ;
                case "F5": return "Tarifa de entrada na rede"                                           ;
                case "S4": return "Tarifa de Inclusão Negativação"                                      ;
                case "S5": return "Tarifa de Exclusão Negativação"                                      ;
                default: return "";
            }
        };

        /// <summary>
        /// Códigos de liquidação / baixa de '01' a '15' associados aos códigos de movimento
        /// '06', '09' e '17' (Descrição C044)
        /// </summary>
        public static Func<string, string> DominioMotivoOcorrencia_C { get; private set; } = (q) =>
        {
            switch (q)
            {
                case "01": return "Por Saldo"                                               ;
                case "02": return "Por Conta"                                               ;
                case "03": return "Liquidação no Guichê de Caixa em Dinheiro"               ;
                case "04": return "Compensação Eletrônica"                                  ;
                case "05": return "Compensação Convencional"                                ;
                case "06": return "Por Meio Eletrônico"                                     ;
                case "07": return "Após Feriado Local"                                      ;
                case "08": return "Em Cartório"                                             ;
                case "30": return "Liquidação no Guichê de Caixa em Cheque"                 ;
                case "31": return "Liquidação em banco correspondente"                      ;
                case "32": return "Liquidação Terminal de Auto - Atendimento"               ;
                case "33": return "Liquidação na Internet(Home banking)"                    ;
                case "34": return "Liquidado Office Banking"                                ;
                case "35": return "Liquidado Correspondente em Dinheiro"                    ;
                case "36": return "Liquidado Correspondente em Cheque"                      ;
                case "37": return "Liquidado por meio de Central de Atendimento(Telefone)"  ;
                case "09": return "Comandada Banco"                                         ;
                case "10": return "Comandada Cliente Arquivo"                               ;
                case "11": return "Comandada Cliente On-line"                               ;
                case "12": return "Decurso Prazo - Cliente"                                 ;
                case "13": return "Decurso Prazo - Banco"                                   ;
                case "14": return "Protestado"                                              ;
                case "15": return "Título Excluído"                                         ;
                default: return "";
            }
        };

    }
}
