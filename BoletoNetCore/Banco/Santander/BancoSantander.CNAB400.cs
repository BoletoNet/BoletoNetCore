using System;
using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using BoletoNetCore.Extensions;

namespace BoletoNetCore
{
    partial class BancoSantander : IBancoCNAB400
    {
        #region Remessa - CNAB400

        public string GerarDetalheRemessaCNAB400(Boleto boleto, ref int registro)
        {
            // Registro 1 - Obrigatório
            var detalhe = GerarDetalheRemessaCNAB400Registro1(boleto, ref registro);
            return detalhe;
        }

        public string GerarHeaderRemessaCNAB400(ref int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 009, 0, "01REMESSA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0010, 017, 0, "01COBRANCA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 020, 0, Beneficiario.CodigoTransmissao, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 030, 0, Beneficiario.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0077, 018, 0, "033SANTANDER", ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0095, 006, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0101, 291, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0392, 003, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
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
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 001, 0, "9", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 006, 0, numeroRegistroCobrancaSimples, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 013, 2, valorBoletoGeral, '0'); 
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0021, 374, 0, string.Empty, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do registro TRAILER do arquivo de REMESSA.", ex);
            }
        }

        private string GerarDetalheRemessaCNAB400Registro1(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 002, 0, boleto.Banco.Beneficiario.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 014, 0, boleto.Banco.Beneficiario.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0018, 004, 0, boleto.Banco.Beneficiario.ContaBancaria.Agencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0022, 008, 0, boleto.Banco.Beneficiario.Codigo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0030, 008, 0, boleto.Banco.Beneficiario.CodigoTransmissao.Substring(boleto.Banco.Beneficiario.CodigoTransmissao.Length - 8), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0038, 025, 0, boleto.NumeroControleParticipante, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0063, 008, 0, boleto.NossoNumero.ToString().Substring(boleto.NossoNumero.Length - 8), '0');
                if (boleto.ValorDesconto2 == 0)
                {
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0071, 006, 0, "0", '0');
                }
                else
                {
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0071, 006, 0, boleto.DataDesconto2, '0');
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0077, 001, 0, string.Empty, ' ');
                if (boleto.PercentualMulta > 0)
                {
                    if (boleto.DataVencimento.ToString("dd/MM/yyyy") == boleto.DataMulta.ToString("dd/MM/yyyy"))
                        boleto.DataMulta = boleto.DataVencimento.AddDays(1);

                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0078, 001, 0, "4", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0079, 004, 2, boleto.PercentualMulta, '0');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0083, 002, 0, "00", '0');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0085, 013, 0, string.Empty, '0');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0098, 004, 0, string.Empty, ' ');
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0102, 006, 0, boleto.DataMulta, ' ');
                }
                else
                {
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0078, 001, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0079, 004, 2, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0083, 002, 0, "00", '0');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0085, 013, 0, string.Empty, '0');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0098, 004, 0, string.Empty, ' ');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0102, 006, 0, string.Empty, ' ');
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0108, 001, 0, ConvertTipoCarteira(boleto.TipoCarteira), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0109, 002, 0, boleto.CodigoMovimentoRetorno, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0111, 010, 0, boleto.NumeroDocumento, '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0121, 006, 0, boleto.DataVencimento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 013, 2, boleto.ValorTitulo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 003, 0, "033", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0143, 005, 0, string.Empty, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0148, 002, 0, boleto.EspecieDocumento.GetHashCode(), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0150, 001, 0, boleto.Aceite, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0151, 006, 0, boleto.DataEmissao, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0157, 002, 0, boleto.CodigoInstrucao1, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0159, 002, 0, "00", '0');
                if (boleto.ValorJurosDia > 0)
                {
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0161, 013, 2, boleto.ValorJurosDia, '0');
                }
                else
                {
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0161, 013, 2, "0", '0');
                }
                if (boleto.ValorDesconto == 0)
                {
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0174, 006, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0180, 013, 2, '0', '0');
                }
                else
                {
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0174, 006, 0, boleto.DataDesconto, '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0180, 013, 2, boleto.ValorDesconto, '0');
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0193, 013, 5, boleto.ValorIOF, '0');
                if (boleto.ValorDesconto2 == 0)
                {
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0206, 013, 0, "0", '0');
                }
                else
                {
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0206, 013, 2, boleto.ValorDesconto2, '0');
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0219, 002, 0, boleto.Pagador.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 014, 0, boleto.Pagador.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0235, 040, 0, boleto.Pagador.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0275, 040, 0, boleto.Pagador.Endereco.FormataLogradouro(40), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0315, 012, 0, boleto.Pagador.Endereco.Bairro, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0327, 008, 0, boleto.Pagador.Endereco.CEP.Replace("-", ""), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0335, 015, 0, boleto.Pagador.Endereco.Cidade, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0350, 002, 0, boleto.Pagador.Endereco.UF, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 031, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0383, 001, 0, 'I', ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0384, 002, 0, 32, ' ');

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0386, 009, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400.", ex);
            }
        }

        private static int ConvertTipoCarteira(TipoCarteira tipoCarteira)
        {
            switch (tipoCarteira)
            {   
                case TipoCarteira.CarteiraCobrancaSimples:
                    return 5;
                case TipoCarteira.CarteiraCobrancaCaucionada:
                    return 6;
                case TipoCarteira.CarteiraCobrancaDescontada:
                    return 7;
                case TipoCarteira.CarteiraCobrancaVendor:
                default:
                    return tipoCarteira.GetHashCode();
            }
        }

        #endregion

        #region Retorno - CNAB400

        public override void LerHeaderRetornoCNAB400(string registro)
        {
            try
            {
                if (registro.Substring(0, 19) != "02RETORNO01COBRANCA")
                    throw new Exception("O arquivo não é do tipo \"02RETORNO01COBRANCA\"");
                if (registro.Substring(79, 15).TrimEnd() != "SANTANDER")
                    throw new Exception("O arquivo não é do tipo \"SANTANDER\"");
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
                // Nº Controle do Participante
                boleto.NumeroControleParticipante = registro.Substring(39, 25);

                // Identificação do Título no Banco
                boleto.NossoNumero = registro.Substring(62, 8); //Sem o DV
                boleto.NossoNumeroFormatado = boleto.NossoNumero + "-" + boleto.NossoNumeroDV;


                // Identificação de Ocorrência
                boleto.CodigoMovimentoRetorno = registro.Substring(108, 2);
                boleto.DescricaoMovimentoRetorno = DescricaoMovimentoRetornoCnab400(boleto.CodigoMovimentoRetorno, registro);

                // Data Ocorrência no Banco
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));

                // Número do Documento
                boleto.NumeroDocumento = registro.Substring(116, 10);

                //Data Vencimento do Título
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(146, 6)).ToString("##-##-##"));

                //Valores do Título
                boleto.ValorTitulo = string.IsNullOrWhiteSpace(registro.Substring(152, 13)) ? 0 : Convert.ToDecimal(registro.Substring(152, 13)) / 100;

                

                boleto.ValorTarifas = string.IsNullOrWhiteSpace(registro.Substring(175, 13)) ? 0 : Convert.ToDecimal(registro.Substring(175, 13)) / 100;
                boleto.ValorOutrasDespesas = string.IsNullOrWhiteSpace(registro.Substring(188, 13)) ? 0 : Convert.ToDecimal(registro.Substring(188, 13)) / 100;
                boleto.ValorJurosDia = string.IsNullOrWhiteSpace(registro.Substring(201, 13)) ? 0 : Convert.ToDecimal(registro.Substring(201, 13)) / 100;
                boleto.ValorIOF = string.IsNullOrWhiteSpace(registro.Substring(214, 13)) ? 0 : Convert.ToDecimal(registro.Substring(214, 13)) / 100;
                boleto.ValorAbatimento = string.IsNullOrWhiteSpace(registro.Substring(227, 13)) ? 0 : Convert.ToDecimal(registro.Substring(227, 13)) / 100;
                boleto.ValorDesconto = string.IsNullOrWhiteSpace(registro.Substring(240, 13)) ? 0 : Convert.ToDecimal(registro.Substring(240, 13)) / 100;
                boleto.ValorPago = string.IsNullOrWhiteSpace(registro.Substring(253, 13)) ? 0 : Convert.ToDecimal(registro.Substring(253, 13)) / 100;
                boleto.ValorMulta = string.IsNullOrWhiteSpace(registro.Substring(266, 13)) ? 0 : Convert.ToDecimal(registro.Substring(266, 13)) / 100;
                boleto.ValorOutrosCreditos = string.IsNullOrWhiteSpace(registro.Substring(279, 13)) ? 0 : Convert.ToDecimal(registro.Substring(279, 13)) / 100;

                
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(295, 6)).ToString("##-##-##"));

                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

        public void LerTrailerRetornoCNAB400(string registro)
        {
        }

        public void LerDetalheRetornoCNAB400Segmento7(ref Boleto boleto, string registro)
        {
            
        }

        private string DescricaoMovimentoRetornoCnab400(string codigo, string registro)
        {
            switch (codigo)
            {
                case "01":
                    return "Boleto não existe";
                case "02":
                    return "Entrada boleto Confirmada";
                case "03":
                    return "Entrada boleto Rejeitada";
                case "04":
                    return "Transferência para carteira Simples";
                case "05":
                    return "Transferência para Carteira Penhor/Desconto/Cessão";
                case "06":
                    return "Liquidação";
                case "07":
                    return "Liquidação por Conta";
                case "08":
                    return "Liquidação por Saldo";
                case "09":
                    return "Baixa Automática";
                case "10":
                    return "Boleto Baixado Conforme Instrução";
                case "11":
                    return "Boletos em carteira (em ser)";
                case "12":
                    return "Abatimento Concedido";
                case "13":
                    return "Abatimento Cancelado";
                case "14":
                    return "Alteração de Vencimento";
                case "15":
                    return "Confirmação de Protesto";
                case "16":
                    return "Boleto Baixado/Liquidado";
                case "17":
                    return "Liquidado em Cartório";
                case "21":
                    return "Boleto Enviado a Cartório";
                case "22":
                    return "Boleto Retirado do Cartório";
                case "24":
                    return "Custas de Cartório";
                case "25":
                    return "Boleto Protestado";
                case "26":
                    return "Sustar Protesto";
                case "27":
                    return "Cancelar Boleto Protestado";
                case "35":
                    return "Boleto DDA Reconhecido pelo Pagador";
                case "36":
                    return "Boleto DDA Não Reconhecido pelo Pagador";
                case "37":
                    return "Boleto DDA Recusado pela CIP";
                case "38":
                    return "Não Protestar (antes de iniciar o ciclo de protesto)";
                case "39":
                    return "Espécie de Boleto não permite a instrução";
                case "61":
                    return "Confirmação de Alteração do Valor Nominal do Boleto";
                case "62":
                    return "Confirmação de Alteração do Valor ou Percentual mínimo";
                case "63":
                    return "Confirmação de Alteração do Valor ou Percentual máximo";
                case "93":
                    return "Baixa Operacional Enviado pela CIP";
                case "94":
                    return "Cancelamento da Baixa Operacional Enviado pela Cip";
                default:
                    return "";
            }
        }

        #endregion

    }
}