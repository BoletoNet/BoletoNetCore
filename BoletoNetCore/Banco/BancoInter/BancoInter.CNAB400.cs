using System;
using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using BoletoNetCore.Extensions;

namespace BoletoNetCore
{
    partial class BancoInter : IBancoCNAB400
    {
        #region Remessa - CNAB400

        public string GerarDetalheRemessaCNAB400(Boleto boleto, ref int registro)
        {
            // Registro 1 - Obrigatório
            var detalhe = GerarDetalheRemessaCNAB400Registro1(boleto, ref registro);
            // Registro 2 Registro Opcional
            string strline = GerarDetalheRemessaCNAB400Registro2(boleto, ref registro);
            if (!string.IsNullOrWhiteSpace(strline))
            {
                detalhe += Environment.NewLine;
                detalhe += strline;
            }
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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 020, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 030, 0, Beneficiario.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0077, 018, 0, "077Inter", ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0095, 006, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0101, 010, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0111, 007, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0118, 277, 0, string.Empty, ' ');
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
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 006, 0, numeroRegistroCobrancaSimples, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0008, 387, 0, string.Empty, ' ');
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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 019, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0021, 003, 0, boleto.Carteira, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0024, 004, 0, boleto.Banco.Beneficiario.ContaBancaria.Agencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0028, 010, 0, boleto.Banco.Beneficiario.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0038, 025, 0, boleto.NumeroControleParticipante, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0063, 003, 0, string.Empty, ' ');
                if(boleto.PercentualMulta > 0 || boleto.ValorMulta > 0)
                {
                    if (boleto.DataVencimento.ToString("dd/MM/yyyy") == boleto.DataMulta.ToString("dd/MM/yyyy"))
                        boleto.DataMulta = boleto.DataVencimento.AddDays(1);

                    if (boleto.ValorMulta > 0)
                    {
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0066, 001, 0, "1", '0');
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0067, 013, 2, boleto.ValorMulta, '0');
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0080, 004, 2, "0", '0');
                    }
                    else
                    {
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0066, 001, 0, "2", '0');
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0067, 013, 2, "0", '0');
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0080, 004, 2, boleto.PercentualMulta, '0');
                    }
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0084, 006, 0, boleto.DataMulta, '0');
                }
                else
                {
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0066, 001, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0067, 013, 2, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0080, 004, 2, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0084, 006, 0, "0", '0');
                }
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0090, 011, 0, boleto.NossoNumero + boleto.NossoNumeroDV, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0101, 008, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0109, 002, 0, "01", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0111, 010, 0, boleto.NumeroDocumento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0121, 006, 0, boleto.DataVencimento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 013, 2, boleto.ValorTitulo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 002, 0, "60", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0142, 006, 0, string.Empty, ' ');
                switch (boleto.EspecieDocumento)
                {
                    case TipoEspecieDocumento.DM:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0148, 002, 0, "01", '0');
                        break;
                    default:
                        throw new Exception("Espécie do título não suportada: (" + boleto.EspecieDocumento + ").");
                }
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0150, 001, 0, "N", ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0151, 006, 0, boleto.DataEmissao, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0157, 003, 0, string.Empty, ' ');
                if (boleto.PercentualJurosDia > 0 || boleto.ValorJurosDia > 0)
                {
                    if (boleto.DataVencimento.ToString("dd/MM/yyyy") == boleto.DataJuros.ToString("dd/MM/yyyy"))
                        boleto.DataJuros = boleto.DataVencimento.AddDays(1);

                    if (boleto.ValorJurosDia > 0)
                    {
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0160, 001, 0, "1", '0');
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0161, 013, 2, boleto.ValorJurosDia, '0');
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0174, 004, 2, "0", '0');
                    }
                    else
                    {
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0160, 001, 0, "2", '0');
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0161, 013, 2, "0", '0');
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0174, 004, 2, (boleto.PercentualJurosDia * 30), '0');
                    }
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0178, 006, 0, boleto.DataJuros, '0');
                }
                else
                {
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0160, 001, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0161, 013, 2, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0174, 004, 2, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0178, 006, 0, "0", '0');
                }
                if (boleto.ValorDesconto == 0)
                {
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0184, 001, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0185, 013, 2, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0198, 004, 2, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0202, 006, 0, "0", '0');
                }
                else
                {
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0184, 001, 0, "1", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0185, 013, 2, boleto.ValorDesconto, '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0198, 004, 2, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0202, 006, 0, boleto.DataDesconto, '0');
                }
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0208, 013, 0, string.Empty, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 002, 0, boleto.Pagador.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0223, 014, 0, boleto.Pagador.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0237, 040, 0, boleto.Pagador.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0277, 040, 0, boleto.Pagador.Endereco.FormataLogradouro(40), ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0317, 008, 0, boleto.Pagador.Endereco.CEP.Replace("-", ""), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0325, 070, 0, boleto.Pagador.Observacoes, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400.", ex);
            }
        }

        private string GerarDetalheRemessaCNAB400Registro2(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(boleto.MensagemArquivoRemessa) && boleto.ValorDesconto2 == 0 && boleto.ValorDesconto3 == 0)
                    return "";

                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "2", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 312, 0, boleto.MensagemArquivoRemessa, ' '); // 4 campos de 75 caracteres cada.
                if (boleto.ValorDesconto2 == 0)
                {
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0314, 006, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0320, 013, 2, "0", '0');
                }
                else
                {
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0314, 006, 0, boleto.DataDesconto2, '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0320, 013, 2, boleto.ValorDesconto2, '0');
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0333, 004, 2, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0337, 010, 0, string.Empty, ' ');
                if (boleto.ValorDesconto3 == 0)
                {
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0347, 006, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0353, 013, 2, "0", '0');
                }
                else
                {
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0347, 006, 0, boleto.DataDesconto3, '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0353, 013, 2, boleto.ValorDesconto3, '0');
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0366, 004, 2, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0370, 010, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0380, 011, 0, boleto.NossoNumero + boleto.NossoNumeroDV, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0391, 004, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400.", ex);
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
                if (registro.Substring(76, 8) != "077INTER")
                    throw new Exception("O arquivo não é do tipo \"077INTER\"");
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
                boleto.NumeroControleParticipante = registro.Substring(37, 25);

                // Identificação do Título no Banco
                boleto.NossoNumero = registro.Substring(70, 10); //Sem o DV
                boleto.NossoNumeroDV = registro.Substring(80, 1); //DV
                boleto.NossoNumeroFormatado = boleto.NossoNumero + "-" + boleto.NossoNumeroDV;

                // Carteira
                boleto.Carteira = registro.Substring(86, 3);
                boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;

                // Identificação de Ocorrência
                boleto.CodigoMovimentoRetorno = registro.Substring(89, 2);
                boleto.DescricaoMovimentoRetorno = DescricaoMovimentoRetornoCnab400(boleto.CodigoMovimentoRetorno, registro);
                boleto.DescricaoMovimentoRetorno += $": {registro.Substring(240, 140).Trim()}";

                // Data Ocorrência no Banco
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(91, 6)).ToString("##-##-##"));

                // Número do Documento
                boleto.NumeroDocumento = registro.Substring(97, 10);

                //Data Vencimento do Título
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(118, 6)).ToString("##-##-##"));

                //Valores do Título
                boleto.ValorTitulo = string.IsNullOrWhiteSpace(registro.Substring(124, 13)) ? 0 : Convert.ToDecimal(registro.Substring(124, 13)) / 100;
                boleto.ValorPago = string.IsNullOrWhiteSpace(registro.Substring(159, 13)) ? 0 : Convert.ToDecimal(registro.Substring(159, 13)) / 100;
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(172, 6)).ToString("##-##-##"));

                boleto.ListMotivosOcorrencia = new List<string>
                    {
                        registro.Substring(240, 140).Trim()
                    };

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
                case "02":
                    return "Em aberto";
                case "03":
                    return "Erro";
                case "06":
                    return "Pago";
                case "07":
                    return "Cancelado";
                default:
                    return "";
            }
        }

        #endregion

    }
}