using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using System;

namespace BoletoNetCore
{
    partial class BancoSicredi : IBancoCNAB400
    {
        public string GerarDetalheRemessaCNAB400(Boleto boleto, ref int registro)
        {
            string detalhe = string.Empty;

            registro++;

            //Redireciona para o Detalhe da remessa Conforme o "Tipo de Documento" = "Tipo de Cobrança do CNAB400":
            //  A = 'A' - SICREDI com Registro
            // C1 = 'C' - SICREDI sem Registro Impressão Completa pelo Sicredi
            // C2 = 'C' - SICREDI sem Registro Pedido de bloquetos pré-impressos
            if (boleto.VariacaoCarteira.Equals("A"))
                detalhe = GerarDetalheRemessaCNAB400_A(boleto, registro);
            else if (boleto.VariacaoCarteira.Equals("C1"))
                detalhe = GerarDetalheRemessaCNAB400_C1(boleto, registro);
            else if (boleto.VariacaoCarteira.Equals("C2"))
                detalhe = GerarDetalheRemessaCNAB400_C2(boleto, registro);
            return detalhe;
        }

        public string GerarHeaderRemessaCNAB400(ref int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 001, 0, "0", ' '));                             //001-001
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 001, 0, "1", ' '));                             //002-002
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0003, 007, 0, "REMESSA", ' '));                       //003-009
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0010, 002, 0, "01", ' '));                            //010-011
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0012, 015, 0, "COBRANCA", ' '));                      //012-026
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0027, 005, 0, Beneficiario.Codigo, ' '));             //027-031
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0032, 014, 0, Beneficiario.CPFCNPJ, ' '));            //032-045
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0046, 031, 0, "", ' '));                              //046-076
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0077, 003, 0, "748", ' '));                           //077-079
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0080, 015, 0, "SICREDI", ' '));                       //080-094
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediDataAAAAMMDD_________, 0095, 008, 0, DateTime.Now, ' '));                    //095-102
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0103, 008, 0, "", ' '));                              //103-110
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0111, 007, 0, numeroArquivoRemessa.ToString(), '0')); //111-117
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0118, 273, 0, "", ' '));                              //118-390
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0391, 004, 0, "2.00", ' '));                          //391-394
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0395, 006, 0, numeroRegistroGeral, '0'));             //395-400

                reg.CodificarLinha();

                string vLinha = reg.LinhaRegistro;
                string _header = Utils.SubstituiCaracteresEspeciais(vLinha);

                return _header;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB400.", ex);
            }
        }

        public string GerarTrailerRemessaCNAB400(int numeroRegistroGeral, decimal valorBoletoGeral, int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples, int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada, int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada, int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 001, 0, "9", ' '));                         //001-001
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 001, 0, "1", ' '));                         //002-002
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0003, 003, 0, "748", ' '));                       //003-006
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0006, 005, 0, Beneficiario.Codigo, ' '));              //006-010                
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0011, 384, 0, string.Empty, ' '));                //011-394
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0'));         //395-400

                reg.CodificarLinha();

                string vLinha = reg.LinhaRegistro;
                string _trailer = Utils.SubstituiCaracteresEspeciais(vLinha);

                return _trailer;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do registro TRAILER do arquivo de REMESSA.", ex);
            }
        }

        private string GerarDetalheRemessaCNAB400_A(Boleto boleto, int numeroRegistro)
        {
            try
            {
                //string NumeroDocumento = boleto.NossoNumero;

                TRegistroEDI reg = new TRegistroEDI();
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 001, 0, "1", ' '));                                                    //001-001
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 001, 0, "A", ' '));                                                    //002-002  'A' - SICREDI com Registro
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0003, 001, 0, "A", ' '));                                                    //003-003  'A' - Simples
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0004, 001, 0, "A", ' '));                                                    //004-004  'A' – Normal
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0005, 012, 0, string.Empty, ' '));                                           //005-016
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0017, 001, 0, "A", ' '));                                                    //017-017  Tipo de moeda: 'A' - REAL
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0018, 001, 0, "A", ' '));                                                    //018-018  Tipo de desconto: 'A' - VALOR
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0019, 001, 0, "B", ' '));                                                    //019-019  Tipo de juros: 'A' - VALOR / 'B' - PERCENTUAL
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0020, 028, 0, string.Empty, ' '));                                           //020-047
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0048, 009, 0, boleto.NossoNumero, '0'));                                     //048-056

                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0057, 006, 0, string.Empty, ' '));                                           //057-062
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediDataAAAAMMDD_________, 0063, 008, 0, boleto.DataProcessamento, ' '));                               //063-070
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0071, 001, 0, string.Empty, ' '));                                           //071-071
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0072, 001, 0, 'N', ' '));                                                    //072-072 'S' - Para postar o título diretamente ao pagador / 'N' - Não Postar e remeter para o beneficiário
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0073, 001, 0, string.Empty, ' '));                                           //073-073

                switch (boleto.Banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto)
                {
                    case TipoImpressaoBoleto.Banco:
                        reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0074, 001, 0, 'A', ' '));                                            //074-074 'A' - Impressão é feita pelo Sicredi
                        break;
                    case TipoImpressaoBoleto.Empresa:
                        reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0074, 001, 0, 'B', ' '));                                            //'B' – Impressão é feita pelo Beneficiário
                        break;
                }

                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0075, 002, 0, 0, '0'));                                                     //075-076
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0077, 002, 0, 0, '0'));                                                     //077-078
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0079, 004, 0, string.Empty, ' '));                                          //079-082
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0083, 010, 2, boleto.ValorDesconto, '0'));                                  //083-092
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0093, 004, 2, boleto.PercentualMulta, '0'));                                //093-096
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0097, 012, 0, string.Empty, ' '));                                          //097-108
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, "01", ' '));                                                  //109-110 01 - Cadastro de título;
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0111, 010, 0, boleto.NumeroDocumento, ' '));                                //111-120
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediDataDDMMAA___________, 0121, 006, 0, boleto.DataVencimento, ' '));                                 //121-126
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 013, 2, boleto.ValorTitulo, '0'));                                    //127-139
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 009, 0, string.Empty, ' '));                                          //140-148
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0149, 001, 0, EspecieDocumentoSicrediCNAB400(boleto.EspecieDocumento), ' '));      //149-149
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0150, 001, 0, boleto.Aceite, ' '));                                         //150-150
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediDataDDMMAA___________, 0151, 006, 0, boleto.DataProcessamento, ' '));                              //151-156

                //Instruções de protesto
                string vInstrucao1 = "";
                string vInstrucao2 = "";
                switch (boleto.CodigoProtesto)
                {
                    case TipoCodigoProtesto.NaoProtestar:
                        vInstrucao1 = "00";
                        vInstrucao2 = "0";
                        break;
                    case TipoCodigoProtesto.ProtestarDiasCorridos:
                        vInstrucao1 = "06";
                        vInstrucao2 = boleto.DiasProtesto.ToString("00");
                        break;
                }
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0157, 002, 0, vInstrucao1, '0'));                                           //157-158
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0159, 002, 0, vInstrucao2, '0'));                                           //159-160

                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0161, 013, 2, boleto.PercentualJurosDia, '0'));                             //161-173

                //DataDesconto
                string vDataDesconto = "000000";
                if (!boleto.DataDesconto.Equals(DateTime.MinValue))
                    vDataDesconto = boleto.DataDesconto.ToString("ddMMyy");

                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0174, 006, 0, vDataDesconto, '0'));                                         //174-179
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0180, 013, 2, boleto.ValorDesconto, '0'));                                  //180-192
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0193, 013, 0, 0, '0'));                                                     //193-205
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0206, 013, 2, boleto.ValorAbatimento, '0'));                                //206-218

                //Regra Tipo de Inscrição Pagador
                string vCpfCnpjSac = "0";
                if (boleto.Pagador.CPFCNPJ.Length.Equals(11)) vCpfCnpjSac = "1";
                else if (boleto.Pagador.CPFCNPJ.Length.Equals(14)) vCpfCnpjSac = "2";

                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0219, 001, 0, vCpfCnpjSac, '0'));                                           //219-219
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0220, 001, 0, "0", '0'));                                                   //220-220
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 014, 0, boleto.Pagador.CPFCNPJ, '0'));                                 //221-234
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0235, 040, 0, boleto.Pagador.Nome.ToUpper(), ' '));                          //235-274
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0275, 040, 0, boleto.Pagador.Endereco.LogradouroEndereco.ToUpper(), ' '));   //275-314
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0315, 005, 0, 0, '0'));                                                     //315-319
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0320, 006, 0, 0, '0'));                                                     //320-325
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0326, 001, 0, string.Empty, ' '));                                          //326-326
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0327, 008, 0, boleto.Pagador.Endereco.CEP, '0'));                            //327-334
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0335, 005, 1, 0, '0'));                                                     //335-339
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0340, 014, 0, string.Empty, ' '));                                          //340-353
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0354, 041, 0, string.Empty, ' '));                                          //354-394
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistro, '0'));                                        //395-400

                reg.CodificarLinha();

                string _detalhe = Utils.SubstituiCaracteresEspeciais(reg.LinhaRegistro);

                return _detalhe;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400.", ex);
            }
        }

        private string EspecieDocumentoSicrediCNAB400(TipoEspecieDocumento EspecieDocumento)
        {
            switch (EspecieDocumento)
            {
                case TipoEspecieDocumento.DMI:
                    return "A";
                case TipoEspecieDocumento.DR:
                    return "B";
                case TipoEspecieDocumento.NP:
                    return "C";
                case TipoEspecieDocumento.NPR:
                    return "D";
                case TipoEspecieDocumento.NS:
                    return "E";
                case TipoEspecieDocumento.RC:
                    return "G";
                case TipoEspecieDocumento.LC:
                    return "H";
                case TipoEspecieDocumento.DSI:
                    return "J";
                case TipoEspecieDocumento.OU:
                    return "K";
            }

            return string.Empty;
        }

        private string GerarDetalheRemessaCNAB400_C1(Boleto boleto, int numeroRegistro)
        {
            throw new NotImplementedException("Função não implementada.");
        }
        private string GerarDetalheRemessaCNAB400_C2(Boleto boleto, int numeroRegistro)
        {
            throw new NotImplementedException("Função não implementada.");
        }


        public void LerDetalheRetornoCNAB400Segmento1(ref Boleto boleto, string registro)
        {
            try
            {
                // Identificação do Título no Banco
                boleto.NossoNumero = registro.Substring(47, 8);
                boleto.NossoNumeroDV = registro.Substring(55, 1);
                boleto.NossoNumeroFormatado = string.Format("{0}/{1}-{2}", boleto.NossoNumero.Substring(0, 2), boleto.NossoNumero.Substring(2, 6), boleto.NossoNumeroDV);

                // Identificação de Ocorrência
                boleto.CodigoMovimentoRetorno = registro.Substring(108, 2);
                boleto.DescricaoMovimentoRetorno = DescricaoOcorrenciaCnab400(boleto.CodigoMovimentoRetorno);

                // Data Ocorrência no Banco
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));

                // Número do Documento
                boleto.NumeroDocumento = registro.Substring(116, 10).Trim();

                // Seu número - Seu número enviado na Remessa
                boleto.NumeroControleParticipante = registro.Substring(116, 10).Trim();

                //Data Vencimento do Título
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(146, 6)).ToString("##-##-##"));

                //Valores do Título
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(152, 13)) / 100;
                boleto.ValorAbatimento = Convert.ToDecimal(registro.Substring(227, 13)) / 100;
                boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(240, 13)) / 100;
                boleto.ValorPago = Convert.ToDecimal(registro.Substring(253, 13)) / 100;
                boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(266, 13)) / 100;
                boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(279, 13)) / 100;

                boleto.ValorPago += boleto.ValorJurosDia;

                // Data do Crédito
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(328, 8)).ToString("####-##-##"));

                // Identificação de Ocorrência - Código Auxiliar
                boleto.CodigoMotivoOcorrencia = registro.Substring(318, 10);
                boleto.ListMotivosOcorrencia = Cnab.MotivoOcorrenciaCnab240(boleto.CodigoMotivoOcorrencia, boleto.CodigoMovimentoRetorno);
               
                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

        private string DescricaoOcorrenciaCnab400(string codigo)
        {
            switch (codigo)
            {
                case "02":
                    return "Confirmação de entrada";
                case "03":
                    return "Entrada rejeitada";
                case "04":
                    return "Baixa de título liquidado por edital";
                case "06":
                    return "Liquidação normal";
                case "07":
                    return "Liquidação parcial";
                case "08":
                    return "Baixa por pagamento, liquidação pelo saldo";
                case "09":
                    return "Devolução automática";
                case "10":
                    return "Baixado conforme instruções";
                case "11":
                    return "Arquivo levantamento";
                case "12":
                    return "Concessão de abatimento";
                case "13":
                    return "Cancelamento de abatimento";
                case "14":
                    return "Vencimento alterado";
                case "15":
                    return "Pagamento em cartório";
                case "16":
                    return "Alteração de dados";
                case "18":
                    return "Alteração de instruções";
                case "19":
                    return "Confirmação de instrução protesto";
                case "20":
                    return "Confirmação de instrução para sustar protesto";
                case "21":
                    return "Aguardando autorização para protesto por edital";
                case "22":
                    return "Protesto sustado por alteração de vencimento e prazo de cartório";
                case "23":
                    return "Confirmação da entrada em cartório";
                case "25":
                    return "Devolução, liquidado anteriormente";
                case "26":
                    return "Devolvido pelo cartório – erro de informação";
                case "30":
                    return "Cobrança a creditar (liquidação em trânsito)";
                case "31":
                    return "Título em trânsito pago em cartório";
                case "32":
                    return "Reembolso e transferência Desconto e Vendor ou carteira em garantia";
                case "33":
                    return "Reembolso e devolução Desconto e Vendor";
                case "34":
                    return "Reembolso não efetuado por falta de saldo";
                case "40":
                    return "Baixa de títulos protestados";
                case "41":
                    return "Despesa de aponte";
                case "42":
                    return "Alteração de título";
                case "43":
                    return "Relação de títulos";
                case "44":
                    return "Manutenção mensal";
                case "45":
                    return "Sustação de cartório e envio de título a cartório";
                case "46":
                    return "Fornecimento de formulário pré-impresso";
                case "47":
                    return "Confirmação de entrada – Pagador DDA";
                case "68":
                    return "Acerto dos dados do rateio de crédito";
                case "69":
                    return "Cancelamento dos dados do rateio";
                default:
                    return "";
            }
        }

        public void LerDetalheRetornoCNAB400Segmento7(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException();
        }


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

        public void LerTrailerRetornoCNAB400(string registro)
        {

        }
    }

}