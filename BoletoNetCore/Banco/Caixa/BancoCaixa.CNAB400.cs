using System;
using static System.String;

namespace BoletoNetCore
{
    partial class BancoCaixa : IBancoCNAB400
    {
        #region Remessa

        public string GerarHeaderRemessaCNAB400(ref int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0003, 007, 0, "REMESSA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0010, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0012, 015, 0, "COBRANCA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 004, 0, Beneficiario.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0031, 007, 0, Beneficiario.Codigo, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0038, 009, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 030, 0, Beneficiario.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0077, 003, 0, "104", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0080, 015, 0, "CEF", ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0095, 006, 0, DateTime.Now, ' ');

                if (Beneficiario.Codigo.Length == 7)
                {
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0101, 003, 0, "007", ' ');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0104, 286, 0, Empty, ' ');
                }
                else
                {
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0101, 289, 0, Empty, ' ');
                }

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0390, 005, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');

                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB400.", ex);
            }
        }

        public string GerarDetalheRemessaCNAB400(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 002, 0, boleto.Banco.Beneficiario.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 014, 0, boleto.Banco.Beneficiario.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 003, 0, Empty, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0021, 007, 0, boleto.Banco.Beneficiario.Codigo, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0028, 001, 0, "2", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0029, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0030, 002, 0, Empty, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0032, 025, 0, boleto.NumeroControleParticipante, ' ');

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0057, 017, 0, boleto.NossoNumero, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0074, 002, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0076, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0077, 030, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0107, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, boleto.CodigoMovimentoRetorno, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0111, 010, 0, boleto.NumeroDocumento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0121, 006, 0, boleto.DataVencimento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 013, 2, boleto.ValorTitulo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 003, 0, "104", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0148, 002, 0, (int)boleto.EspecieDocumento, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0150, 001, 0, boleto.Aceite, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0151, 006, 0, boleto.DataEmissao, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0157, 002, 0, boleto.CodigoInstrucao1, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0159, 002, 0, boleto.CodigoInstrucao2, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0161, 013, 2, boleto.ValorJurosDia, '0');

                if (boleto.ValorDesconto == 0)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0174, 006, 0, "0", '0'); // Sem Desconto
                else
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0174, 006, 0, boleto.DataDesconto, '0'); // Com Desconto

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0180, 013, 2, boleto.ValorDesconto, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0193, 013, 2, boleto.ValorIOF, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0206, 013, 2, boleto.ValorAbatimento, '0');

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0219, 002, 0, boleto.Pagador.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 014, 0, boleto.Pagador.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0235, 040, 0, boleto.Pagador.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0275, 040, 0, boleto.Pagador.Endereco.FormataLogradouro(40), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0315, 012, 0, boleto.Pagador.Endereco.Bairro, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0327, 008, 0, boleto.Pagador.Endereco.CEP.Replace("-", ""), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0335, 015, 0, boleto.Pagador.Endereco.Cidade, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0350, 002, 0, boleto.Pagador.Endereco.UF, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0352, 006, 0, boleto.DataMulta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0358, 010, 2, boleto.ValorMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0368, 022, 0, boleto.Avalista.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0390, 002, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0392, 002, 0, boleto.DiasLimiteRecebimento.HasValue ? boleto.DiasLimiteRecebimento.Value.ToString("00") : "99", '0'); // Caso não for informado, irá definir o máximo de dias "99".
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0394, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');

                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400 - Registro 1.", ex);
            }
        }

        public string GerarTrailerRemessaCNAB400(int numeroRegistroGeral, decimal valorBoletoGeral, int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples, int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada, int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada, int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "9", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 393, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do registro TRAILER do arquivo de REMESSA.", ex);
            }
        }

        #endregion

        #region Retorno

        public override void LerHeaderRetornoCNAB400(string registro)
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

        public void LerDetalheRetornoCNAB400Segmento1(ref Boleto boleto, string registro)
        {
            try
            {
                //Nº Controle do Participante
                boleto.NumeroControleParticipante = registro.Substring(31, 6);

                //Carteira
                boleto.Carteira = registro.Substring(106, 2);
                boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;

                //Identificação do Título no Banco
                boleto.NossoNumero = registro.Substring(56, 17);
                boleto.NossoNumeroDV = registro.Substring(93, 1); //DV
                boleto.NossoNumeroFormatado = $"{boleto.Carteira}/{boleto.NossoNumero}-{boleto.NossoNumeroDV}";

                //Identificação de Ocorrência
                boleto.CodigoMovimentoRetorno = registro.Substring(108, 2);
                boleto.CodigoMotivoOcorrencia = registro.Substring(79, 2);
                boleto.DescricaoMovimentoRetorno = DescricaoOcorrenciaCnab400(boleto.CodigoMovimentoRetorno, boleto.CodigoMotivoOcorrencia);

                //Número do Documento
                boleto.NumeroDocumento = registro.Substring(116, 10);
                boleto.EspecieDocumento = AjustaEspecieCnab400(registro.Substring(173, 2));

                //Valores do Título
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(152, 13)) / 100;
                boleto.ValorTarifas = Convert.ToDecimal(registro.Substring(175, 13)) / 100;
                boleto.ValorOutrasDespesas = 0; // Convert.ToDecimal(registro.Substring(188, 13)) / 100;
                boleto.ValorIOF = Convert.ToDecimal(registro.Substring(214, 13)) / 100;
                boleto.ValorAbatimento = Convert.ToDecimal(registro.Substring(227, 13)) / 100;
                boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(240, 13)) / 100;
                boleto.ValorPagoCredito = Convert.ToDecimal(registro.Substring(253, 13)) / 100;
                boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(266, 13)) / 100;
                boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(279, 13)) / 100;

                //Data Ocorrência no Banco
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));

                //Data Vencimento do Título
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(146, 6)).ToString("##-##-##"));

                // Data do Crédito
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(293, 6)).ToString("##-##-##"));

                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

        public void LerDetalheRetornoCNAB400Segmento7(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException();
        }

        public void LerTrailerRetornoCNAB400(string registro)
        {
        }

        private string DescricaoOcorrenciaCnab400(string codigo, string codigoRejeicao)
        {
            switch (codigo)
            {
                case "01":
                    return "Entrada Confirmada";
                case "02":
                    return "Baixa Manual Confirmada";
                case "03":
                    return "Abatimento Concedido";
                case "04":
                    return "Abatimento Cancelado";
                case "05":
                    return "Vencimento Alterado";
                case "06":
                    return "Uso da Empresa Alterado";
                case "07":
                    return "Prazo de Protesto Alterado";
                case "08":
                    return "Prazo de Devolução Alterado";
                case "09":
                    return "Alteração Confirmada";
                case "10":
                    return "Alteração com reemissão de boleto confirmada";
                case "11":
                    return "Alteração da opção de Protesto para Devolução Confirmada";
                case "12":
                    return "Alteração da opção de Devolução para Protesto Confirmada";
                case "20":
                    return "Em Ser";
                case "21":
                    return "Liquidação";
                case "22":
                    return "Liquidação em Cartório";
                case "23":
                    return "Baixa por Devolução";
                case "25":
                    return "Baixa por Protesto";
                case "26":
                    return "Título enviado para Cartório";
                case "27":
                    return "Sustação de Protesto";
                case "28":
                    return "Estorno de Protesto";
                case "29":
                    return "Estorno de Sustação de Protesto";
                case "30":
                    return "Alteração de Título";
                case "31":
                    return "Tarifa sobre Título Vencido";
                case "32":
                    return "Outras Tarifas de Alteração";
                case "33":
                    return "Estorno de Baixa / Liquidação";
                case "34":
                    return "Tarifas Diversas";
                case "35":
                    return "Liquidação On-line";
                case "36":
                    return "Estorno de Liquidação On-line";
                case "37":
                    return "Transferência para a cobrança simples";
                case "38":
                    return "Transferência para a cobrança descontada";
                case "51":
                    return "Reconhecido pelo pagador";
                case "52":
                    return "Não reconhecido pelo pagador";
                case "53":
                    return "Recusado no DDA";
                case "A4":
                    return "Pagador DDA";

                case "99":
                    return DescricaoRejeicaoCnab400(codigoRejeicao);
                default:
                    return "";
            }
        }

        private string DescricaoRejeicaoCnab400(string codigo)
        {
            switch (codigo)
            {
                case "01":
                    return "Movimento sem Beneficiário Correspondente";
                case "02":
                    return "Movimento sem Título Correspondente";
                case "08":
                    return "Movimento para título já com movimentação no dia";
                case "09":
                    return "Nosso Número não pertence ao Beneficiário";
                case "10":
                    return "Inclusão de título já existente na base";
                case "12":
                    return "Movimento duplicado";
                case "13":
                    return "Entrada Inválida para Cobrança Caucionada(Beneficiário não possui conta Caução)";
                case "20":
                    return "CEP do Pagador não encontrado(não foi possível a determinação da Agência Cobradora para o título)";
                case "21":
                    return "Agência cobradora não encontrada (agência designada para cobradora não cadastrada no sistema)";
                case "22":
                    return "Agência Beneficiário não encontrada (Agência do Beneficiário não cadastrada no sistema)";
                case "26":
                    return "Data de vencimento inválida";
                case "44":
                    return "CEP do pagador inválido";
                case "45":
                    return "Data de Vencimento com prazo superior ao limite";
                case "49":
                    return "Movimento inválido para título Baixado / Liquidado";
                case "50":
                    return "Movimento inválido para título enviado a Cartório";
                case "54":
                    return "Faixa de CEP da Agência Cobradora não abrange CEP do Pagador";
                case "55":
                    return "Título já com opção de Devolução";
                case "56":
                    return "Processo de Protesto em andamento";
                case "57":
                    return "Título já com opção de Protesto";
                case "58":
                    return "Processo de devolução em andamento";
                case "59":
                    return "Novo prazo p / Protesto / Devolução inválido";
                case "76":
                    return "Alteração do prazo de protesto inválida";
                case "77":
                    return "Alteração do prazo de devolução inválida";
                case "81":
                    return "CEP do Pagador inválido";
                case "82":
                    return "CNPJ / CPF do Pagador inválido (dígito não confere)";
                case "83":
                    return "Número do Documento(seu número) inválido";
                case "84":
                    return "Protesto inválido para título sem Número do documento(seu número)";
                default:
                    return "";
            }
        }

        private TipoEspecieDocumento AjustaEspecieCnab400(string codigoEspecie)
        {
            switch (codigoEspecie)
            {
                case "01":
                    return TipoEspecieDocumento.CH;
                case "02":
                    return TipoEspecieDocumento.DM;
                case "03":
                    return TipoEspecieDocumento.DMI;
                case "04":
                    return TipoEspecieDocumento.DS;
                case "05":
                    return TipoEspecieDocumento.DSI;
                case "06":
                    return TipoEspecieDocumento.DR;
                case "07":
                    return TipoEspecieDocumento.LC;
                case "08":
                    return TipoEspecieDocumento.NCC;
                case "09":
                    return TipoEspecieDocumento.NCE;
                case "10":
                    return TipoEspecieDocumento.NCI;
                case "11":
                    return TipoEspecieDocumento.NCR;
                case "12":
                    return TipoEspecieDocumento.NP;
                case "13":
                    return TipoEspecieDocumento.NPR;
                case "14":
                    return TipoEspecieDocumento.TM;
                case "15":
                    return TipoEspecieDocumento.TS;
                case "16":
                    return TipoEspecieDocumento.NS;
                case "17":
                    return TipoEspecieDocumento.RC;
                case "18":
                    return TipoEspecieDocumento.FAT;
                case "19":
                    return TipoEspecieDocumento.ND;
                case "20":
                    return TipoEspecieDocumento.AP;
                case "21":
                    return TipoEspecieDocumento.ME;
                case "22":
                    return TipoEspecieDocumento.PC;
                case "23":
                    return TipoEspecieDocumento.NF;
                case "24":
                    return TipoEspecieDocumento.DD;
                case "25":
                    return TipoEspecieDocumento.CPR;
                case "31":
                    return TipoEspecieDocumento.CC;
                case "32":
                    return TipoEspecieDocumento.BP;
                case "99":
                    return TipoEspecieDocumento.OU;
                default:
                    return TipoEspecieDocumento.OU;
            }
        }

        #endregion
    }
}