using System;
using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using static System.String;

namespace BoletoNetCore
{
    partial class BancoSicoob : IBancoCNAB400
    {
        public string GerarDetalheRemessaCNAB400(Boleto boleto, ref int registro)
        {
            return GerarDetalheRemessaCNAB400Registro1(boleto, ref registro);
        }

        public string GerarHeaderRemessaCNAB400(ref int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0003, 007, 0, "REMESSA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0010, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0012, 008, 0, "COBRANÇA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0020, 007, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 004, 0, Beneficiario.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0031, 001, 0, Beneficiario.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0032, 008, 0, Beneficiario.Codigo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0040, 001, 0, Beneficiario.CodigoDV, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0041, 006, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 030, 0, Beneficiario.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0077, 018, 0, "756BANCOOBCED", ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0095, 006, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0101, 007, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0108, 287, 0, Empty, ' ');
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
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 001, 0, "9", ' ');
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

        private string GerarDetalheRemessaCNAB400Registro1(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 002, 0, boleto.Banco.Beneficiario.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 014, 0, boleto.Banco.Beneficiario.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 004, 0, boleto.Banco.Beneficiario.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0022, 001, 0, boleto.Banco.Beneficiario.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0023, 008, 0, boleto.Banco.Beneficiario.ContaBancaria.Conta.Substring(boleto.Banco.Beneficiario.ContaBancaria.Conta.Length - 8, 8), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0031, 001, 0, boleto.Banco.Beneficiario.ContaBancaria.DigitoConta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0032, 006, 0, "000000", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0038, 025, 0, boleto.NumeroControleParticipante, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0063, 011, 0, boleto.NossoNumero, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0074, 001, 0, boleto.NossoNumeroDV, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0075, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0077, 002, 0, "00", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0079, 003, 0, Empty, ' ');
                if (boleto.Avalista.Nome == Empty)
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0082, 001, 0, Empty, ' ');
                else
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0082, 001, 0, Empty, 'A');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0083, 003, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0086, 003, 0, "000", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0089, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0090, 005, 0, "00000", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0095, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0096, 006, 0, "000000", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0102, 004, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0106, 001, 0, (int)boleto.Banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto, '0');

                switch (boleto.TipoCarteira)
                {
                    case TipoCarteira.CarteiraCobrancaSimples:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0107, 002, 0, "01", ' '); // Simples com Registro
                        break;
                    case TipoCarteira.CarteiraCobrancaCaucionada:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0107, 002, 0, "03", ' '); // Garantida Caucionada
                        break;
                    default:
                        throw new Exception("Tipo de carteira não suportada: (" + boleto.TipoCarteira.ToString() + ").");
                }
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, boleto.CodigoMovimentoRetorno, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0111, 010, 0, boleto.NumeroDocumento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0121, 006, 0, boleto.DataVencimento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 013, 2, boleto.ValorTitulo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 003, 0, "756", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 004, 0, boleto.Banco.Beneficiario.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0147, 001, 0, boleto.Banco.Beneficiario.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0148, 002, 0, AjustaEspecieCnab400(boleto.EspecieDocumento), '0');
                if (boleto.Aceite == "N")
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0150, 001, 0, "0", ' ');
                else
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0150, 001, 0, "1", ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0151, 006, 0, boleto.DataEmissao, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0157, 002, 0, boleto.CodigoInstrucao1, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0159, 002, 0, boleto.CodigoInstrucao2, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0161, 006, 4, boleto.PercentualJurosDia * 30, '0'); // Multiplica por 30 dias = Taxa de juros ao mês
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0167, 006, 4, boleto.PercentualMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0173, 001, 0, (int)boleto.Banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto, '0');

                if (boleto.ValorDesconto == 0)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0174, 006, 0, "000000", '0');   // Sem Desconto
                else
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0174, 006, 0, boleto.DataDesconto, '0');   // Com Desconto

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0180, 013, 2, boleto.ValorDesconto, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0193, 001, 0, boleto.CodigoMoeda, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0194, 012, 2, boleto.ValorIOF, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0206, 013, 2, boleto.ValorAbatimento, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0219, 002, 0, boleto.Pagador.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 014, 0, boleto.Pagador.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0235, 040, 0, boleto.Pagador.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0275, 037, 0, boleto.Pagador.Endereco.FormataLogradouro(37), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0312, 015, 0, boleto.Pagador.Endereco.Bairro, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0327, 008, 0, boleto.Pagador.Endereco.CEP.Replace("-", ""), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0335, 015, 0, boleto.Pagador.Endereco.Cidade, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0350, 002, 0, boleto.Pagador.Endereco.UF, ' ');
                if (IsNullOrEmpty(boleto.Avalista.Nome))
                {
                    // Mensagem para o pagador
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 040, 0, boleto.MensagemArquivoRemessa, ' ');
                }
                else
                {
                    // Avalista
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 040, 0, boleto.Avalista.Nome, ' ');
                }

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0392, 002, 0, boleto.DiasProtesto, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0394, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400.", ex);
            }
        }

        private string OcorrenciasCnab400(string codigo)
        {
            switch (codigo)
            {
                case "02":
                    return "Confirmação de Entrada de Título";
                case "05":
                    return "Liquidação sem registro";
                case "06":
                    return "Liquidação normal";
                case "09":
                    return "Baixa de Titulo";
                case "10":
                    return "Baixa Solicitada";
                case "11":
                    return "Títulos em Ser";
                case "14":
                    return "Alteração de Vencimento do título";
                case "15":
                    return "Liquidação em Cartório";
                case "23":
                    return "Indicação de encaminhamento a cartório";
                case "27":
                    return "Confirmação Alteração Dados";
                case "48":
                    return "Confirmação de instrução de transferência de carteira/modalidade de cobrança";
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
                case "06":
                    return TipoEspecieDocumento.DR;
                case "08":
                    return TipoEspecieDocumento.LC;
                case "09":
                    return TipoEspecieDocumento.WAR;
                case "10":
                    return TipoEspecieDocumento.CH;
                case "12":
                    return TipoEspecieDocumento.DS;
                case "13":
                    return TipoEspecieDocumento.ND;
                case "14":
                    return TipoEspecieDocumento.TM;
                case "15":
                    return TipoEspecieDocumento.TS;
                case "18":
                    return TipoEspecieDocumento.FAT;
                case "20":
                    return TipoEspecieDocumento.AP;
                case "21":
                    return TipoEspecieDocumento.ME;
                case "22":
                    return TipoEspecieDocumento.PC;
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
                case TipoEspecieDocumento.DR:
                    return "06";
                case TipoEspecieDocumento.LC:
                    return "08";
                case TipoEspecieDocumento.WAR:
                    return "09";
                case TipoEspecieDocumento.CH:
                    return "10";
                case TipoEspecieDocumento.DS:
                    return "12";
                case TipoEspecieDocumento.ND:
                    return "13";
                case TipoEspecieDocumento.TM:
                    return "14";
                case TipoEspecieDocumento.TS:
                    return "15";
                case TipoEspecieDocumento.FAT:
                    return "18";
                case TipoEspecieDocumento.AP:
                    return "20";
                case TipoEspecieDocumento.ME:
                    return "21";
                case TipoEspecieDocumento.PC:
                    return "22";
                default:
                    return "99";
            }
        }

        public override void LerHeaderRetornoCNAB400(string registro)
        {
            try
            {
                if (registro.Substring(0, 9) != "02RETORNO")
                {
                    throw new Exception("O arquivo não é do tipo \"02RETORNO\"");
                }
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
                boleto.NumeroControleParticipante = registro.Substring(37, 25);

                //Carteira
                boleto.Carteira = registro.Substring(106, 2);
                switch (boleto.Carteira)
                {
                    case "01":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;
                        break;
                    case "03":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaCaucionada;
                        break;
                    default:
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;
                        break;
                }

                //Identificação do Título no Banco
                //O DV do nosso número considera apenas 7 posições, mas no retorno, vem com 12 posições (63 a 74).
                //Conforme Manual, Nosso Número deve ter 7 dígitos + DV.
                //No arquivo retorno, volta com 12 dígitos. Entendemos que os 4 primeiros serão sempre 00, seguido de 7 dígitos + 1 dígito DV.
                //Se isso não ocorrer, será necessário alterar o método FormatarNossoNumero (onde considera apenas 7 digitos).
                if (registro.Substring(62, 4) != "0000")
                    throw new Exception("Verificar arquivo retorno:  O nosso número no arquivo retorno é maior que 7 dígitos.");
                boleto.NossoNumero = registro.Substring(66, 7); //Sem o DV
                boleto.NossoNumeroDV = registro.Substring(73, 1); //DV
                boleto.NossoNumeroFormatado = $"{boleto.NossoNumero}-{boleto.NossoNumeroDV}";

                //Identificação de Ocorrência
                boleto.CodigoMovimentoRetorno = registro.Substring(108, 2);
                boleto.DescricaoMovimentoRetorno = OcorrenciasCnab400(boleto.CodigoMovimentoRetorno);
                boleto.CodigoMotivoOcorrencia = "";

                //Número do Documento
                boleto.NumeroDocumento = registro.Substring(116, 10);
                boleto.EspecieDocumento = AjustaEspecieCnab400(registro.Substring(173, 2));

                //Valores do Título
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(152, 13)) / 100;
                boleto.ValorTarifas = (Convert.ToDecimal(registro.Substring(181, 7)) / 100);
                boleto.ValorOutrasDespesas = (Convert.ToDecimal(registro.Substring(188, 13)) / 100);
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
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(175, 6)).ToString("##-##-##"));

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
    }
}
