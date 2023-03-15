using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using BoletoNetCore.Exceptions;
using BoletoNetCore.Extensions;
using static System.String;

namespace BoletoNetCore
{
    partial class BancoBradesco : IBancoCNAB400
    {
        public string GerarDetalheRemessaCNAB400(Boleto boleto, ref int numeroRegistro)
        {
            string detalhe = GerarDetalheRemessaCNAB400Registro1(boleto, ref numeroRegistro);
            string strline = GerarDetalheRemessaCNAB400Registro2(boleto, ref numeroRegistro);
            if (!IsNullOrWhiteSpace(strline))
            {
                detalhe += Environment.NewLine;
                detalhe += strline;
            }
            return detalhe;
        }

        public void LerDetalheRetornoCNAB400Segmento1(ref Boleto boleto, string registro)
        {
            try
            {
                //Nº Controle do Participante
                boleto.NumeroControleParticipante = registro.Substring(37, 25);

                //Carteira (no arquivo retorno, vem com 1 caracter. Ajustamos para 2 caracteres, como no manual do Bradesco.
                boleto.Carteira = registro.Substring(107, 1).PadLeft(2, '0');
                boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;

                //Identificação do Título no Banco
                boleto.NossoNumero = registro.Substring(70, 11); //Sem o DV
                boleto.NossoNumeroDV = registro.Substring(81, 1); //DV
                boleto.NossoNumeroFormatado = $"{boleto.Carteira}/{boleto.NossoNumero}-{boleto.NossoNumeroDV}";

                //Identificação de Ocorrência
                boleto.CodigoMovimentoRetorno = registro.Substring(108, 2);
                boleto.DescricaoMovimentoRetorno = DescricaoOcorrenciaCnab400(boleto.CodigoMovimentoRetorno);
                boleto.CodigoMotivoOcorrencia = registro.Substring(318, 10);

                //Número do Documento
                boleto.NumeroDocumento = registro.Substring(116, 10);
                boleto.EspecieDocumento = AjustaEspecieCnab400(registro.Substring(173, 2));

                //Valores do Título
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(152, 13)) / 100;
                boleto.ValorTarifas = Convert.ToDecimal(registro.Substring(175, 13)) / 100;
                boleto.ValorOutrasDespesas = Convert.ToDecimal(registro.Substring(188, 13)) / 100;
                boleto.ValorIOF = Convert.ToDecimal(registro.Substring(214, 13)) / 100;
                boleto.ValorAbatimento = Convert.ToDecimal(registro.Substring(227, 13)) / 100;
                boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(240, 13)) / 100;
                boleto.ValorPago = Convert.ToDecimal(registro.Substring(253, 13)) / 100;
                boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(266, 13)) / 100;
                boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(279, 13)) / 100;

                //Data Ocorrência no Banco
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));

                //Data Vencimento do Título
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(146, 6)).ToString("##-##-##"));

                // Data do Crédito
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(295, 6)).ToString("##-##-##"));

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
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0012, 008, 0, "COBRANCA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0020, 007, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 020, 0, Beneficiario.Codigo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 030, 0, Beneficiario.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0077, 018, 0, "237BRADESCO", ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0095, 006, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0101, 008, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, "MX", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0111, 007, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0118, 277, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB400.", ex);
            }
        }

        private string GerarDetalheRemessaCNAB400Registro1(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0007, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0013, 007, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0020, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0021, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0022, 003, 0, boleto.Carteira, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0025, 005, 0, boleto.Banco.Beneficiario.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0030, 007, 0, boleto.Banco.Beneficiario.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0037, 001, 0, boleto.Banco.Beneficiario.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0038, 025, 0, boleto.NumeroControleParticipante, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0063, 003, 0, "0", '0');

                // 0=sem multa, 2=com multa (1, N)
                if (boleto.PercentualMulta > 0)
                {
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0066, 001, 0, "2", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0067, 004, 2, boleto.PercentualMulta, '0');
                }
                else
                {
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0066, 001, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0067, 004, 2, "0", '0');
                }

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0071, 011, 0, boleto.NossoNumero, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0082, 001, 0, boleto.NossoNumeroDV, '0');

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0083, 010, 0, "0", '0');

                switch (boleto.Banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto)
                {
                    case TipoImpressaoBoleto.Banco:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0093, 001, 0, "1", '0');
                        break;
                    case TipoImpressaoBoleto.Empresa:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0093, 001, 0, "2", '0');
                        break;
                }

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0094, 001, 0, boleto.EmiteBoletoDebitoAutomatico, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0095, 010, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0105, 001, 0, boleto.RateioCredito, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0106, 001, 0, boleto.AvisoDebitoAutomaticoContaCorrente, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0107, 002, 0, boleto.QuantidadePagamentos, ' ');

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, boleto.CodigoMovimentoRetorno, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0111, 010, 0, boleto.NumeroDocumento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0121, 006, 0, boleto.DataVencimento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 013, 2, boleto.ValorTitulo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 003, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0148, 002, 0, AjustaEspecieCnab400(boleto.EspecieDocumento), '0');
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
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0315, 012, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0327, 008, 0, boleto.Pagador.Endereco.CEP.Replace("-", ""), '0');
                if (IsNullOrEmpty(boleto.Avalista.Nome))
                {
                    // Não tem avalista.
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0335, 060, 0, Empty, ' ');
                }
                else if (boleto.Avalista.TipoCPFCNPJ("A") == "F")
                {
                    // Avalista Pessoa Física
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0335, 009, 0, boleto.Avalista.CPFCNPJ.Substring(0, 9), '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0344, 004, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0348, 002, 0, boleto.Avalista.CPFCNPJ.Substring(9, 2), '0');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0350, 002, 0, Empty, ' ');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 043, 0, boleto.Avalista.Nome, ' ');
                }
                else
                {
                    // Avalista Pessoa Juridica
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0335, 015, 0, boleto.Avalista.CPFCNPJ, '0');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0350, 002, 0, Empty, ' ');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 043, 0, boleto.Avalista.Nome, '0');
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400 - Registro 1.", ex);
            }
        }

        private string GerarDetalheRemessaCNAB400Registro2(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                if (IsNullOrWhiteSpace(boleto.MensagemArquivoRemessa))
                    return "";

                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "2", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 320, 0, boleto.MensagemArquivoRemessa, ' '); // 4 campos de 80 caracteres cada.
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0322, 006, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0328, 013, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0341, 006, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0347, 013, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0360, 007, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0367, 003, 0, boleto.Carteira, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0370, 005, 0, boleto.Banco.Beneficiario.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0375, 007, 0, boleto.Banco.Beneficiario.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0382, 001, 0, boleto.Banco.Beneficiario.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0383, 011, 0, boleto.NossoNumero, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0394, 001, 0, boleto.NossoNumeroDV, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400 - Registro 2.", ex);
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

        private string DescricaoOcorrenciaCnab400(string codigo)
        {
            switch (codigo)
            {
                case "02":
                    return "Entrada Confirmada";
                case "03":
                    return "Entrada Rejeitada";
                case "06":
                    return "Liquidação normal";
                case "09":
                    return "Baixado Automaticamente via Arquivo";
                case "10":
                    return "Baixado conforme instruções da Agência";
                case "11":
                    return "Em Ser - Arquivo de Títulos pendentes";
                case "12":
                    return "Abatimento Concedido";
                case "13":
                    return "Abatimento Cancelado";
                case "14":
                    return "Vencimento Alterado";
                case "15":
                    return "Liquidação em Cartório";
                case "16":
                    return "Título Pago em Cheque – Vinculado";
                case "17":
                    return "Liquidação após baixa ou Título não registrado";
                case "18":
                    return "Acerto de Depositária";
                case "19":
                    return "Confirmação Recebimento Instrução de Protesto";
                case "20":
                    return "Confirmação Recebimento Instrução Sustação de Protesto";
                case "21":
                    return "Acerto do Controle do Participante";
                case "23":
                    return "Entrada do Título em Cartório";
                case "24":
                    return "Entrada rejeitada por CEP Irregular";
                case "27":
                    return "Baixa Rejeitada";
                case "28":
                    return "Débito de tarifas/custas";
                case "30":
                    return "Alteração de Outros Dados Rejeitados";
                case "32":
                    return "Instrução Rejeitada";
                case "33":
                    return "Confirmação Pedido Alteração Outros Dados";
                case "34":
                    return "Retirado de Cartório e Manutenção Carteira";
                case "35":
                    return "Desagendamento ) débito automático";
                case "68":
                    return "Acerto dos dados ) rateio de Crédito";
                case "69":
                    return "Cancelamento dos dados ) rateio";
                default:
                    return "";
            }
        }

        private TipoEspecieDocumento AjustaEspecieCnab400(string codigoEspecie)
        {
            switch (codigoEspecie)
            {
                case "01":
                    return TipoEspecieDocumento.DM;
                case "02":
                    return TipoEspecieDocumento.NP;
                case "03":
                    return TipoEspecieDocumento.NS;
                case "05":
                    return TipoEspecieDocumento.RC;
                case "10":
                    return TipoEspecieDocumento.LC;
                case "11":
                    return TipoEspecieDocumento.ND;
                case "12":
                    return TipoEspecieDocumento.DS;
                default:
                    return TipoEspecieDocumento.OU;
            }
        }

        private string AjustaEspecieCnab400(TipoEspecieDocumento especieDocumento)
        {
            switch (especieDocumento)
            {
                case TipoEspecieDocumento.DM:
                    return "01";
                case TipoEspecieDocumento.NP:
                    return "02";
                case TipoEspecieDocumento.NS:
                    return "03";
                case TipoEspecieDocumento.RC:
                    return "05";
                case TipoEspecieDocumento.LC:
                    return "10";
                case TipoEspecieDocumento.ND:
                    return "11";
                case TipoEspecieDocumento.DS:
                    return "12";
                default:
                    return "99";
            }
        }

        public void LerTrailerRetornoCNAB400(string registro)
        {
        }

        public override void CompletarHeaderRetornoCNAB400(string registro)
        {
            //021 a 037 - Identificações da Empresa Beneficiária no Banco
            //Deverá ser preenchido(esquerda para direita), da seguinte maneira:
            //21 a 21 - Zero
            //22 a 24 - códigos da carteira
            //25 a 29 - códigos da Agência Beneficiários, sem o dígito.
            //30 a 36 - Contas Corrente
            //37 a 37 - dígitos da Conta

            this.Beneficiario.ContaBancaria = new ContaBancaria();
            this.Beneficiario.ContaBancaria.Agencia = registro.Substring(24, 5);

            var conta = registro.Substring(29, 8).Trim();
            this.Beneficiario.ContaBancaria.Conta = conta.Substring(0, 7);
            this.Beneficiario.ContaBancaria.DigitoConta = conta.Substring(7, 1);

            // 01 - cpf / 02 - cnpj
            if (registro.Substring(1, 2) == "01")
                this.Beneficiario.CPFCNPJ = registro.Substring(6, 11);
            else
                this.Beneficiario.CPFCNPJ = registro.Substring(3, 14);
        }
    }
}