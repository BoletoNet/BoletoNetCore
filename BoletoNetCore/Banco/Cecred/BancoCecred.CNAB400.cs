using System;
using static System.String;

namespace BoletoNetCore
{
    partial class BancoCecred : IBancoCNAB400
    {
        public string GerarDetalheRemessaCNAB400(Boleto boleto, ref int registro)
        {
            try
            {
                TRegistroEDI tregistroEdi = new TRegistroEDI();
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 1, 1, 0, "7", ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 2, 2, 0, Beneficiario.TipoCPFCNPJ("0"), ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 4, 14, 0, Beneficiario.CPFCNPJ, '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 18, 4, 0, Beneficiario.ContaBancaria.Agencia, '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 22, 1, 0, Beneficiario.ContaBancaria.DigitoAgencia, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 23, 8, 0, Beneficiario.ContaBancaria.Conta.PadRight(8, '0').Substring(0, 8), '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 31, 1, 0, Beneficiario.ContaBancaria.DigitoConta, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 32, 7, 0, "", '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 39, 25, 0, Empty, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 64, 17, 0, boleto.NossoNumero, '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 81, 2, 0, "00", '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 83, 2, 0, "00", '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 85, 3, 0, Empty, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 88, 1, 0, Empty, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 89, 3, 0, Empty, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 92, 3, 0, boleto.VariacaoCarteira, '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 95, 1, 0, "0", '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 96, 6, 0, "0", '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 102, 5, 0, Empty, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 107, 2, 0, boleto.Carteira, '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 109, 2, 0, (int)boleto.Banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 111, 10, 0, boleto.NumeroDocumento, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediDataDDMMAA___________, 121, 6, 0, boleto.DataVencimento, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, sbyte.MaxValue, 13, 2, boleto.ValorTitulo, '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 140, 3, 0, "085", '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 143, 4, 0, "0000", '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 147, 1, 0, Empty, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 148, 2, 0, (int)boleto.EspecieDocumento, '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 150, 1, 0, boleto.Aceite, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediDataDDMMAA___________, 151, 6, 0, boleto.DataProcessamento, ' '));
                string str1 = "";
                string str2 = "";
                int num;
                switch (boleto.CodigoProtesto)
                {
                    case TipoCodigoProtesto.NaoProtestar:
                        str1 = "00";
                        str2 = "0";
                        break;
                    case TipoCodigoProtesto.ProtestarDiasCorridos:
                        str1 = "06";
                        num = boleto.DiasProtesto;
                        str2 = num.ToString();
                        break;
                }
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 157, 2, 0, str1, '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 159, 2, 0, "00", '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 161, 13, 2, boleto.ValorJurosDia, '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 174, 6, 0, "000000", '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 180, 13, 2, boleto.ValorDesconto, '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 193, 13, 0, "000000", '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 206, 13, 2, boleto.ValorAbatimento, '0'));
                string str3 = "02";
                string str4 = "02";
                num = Beneficiario.CPFCNPJ.Length;
                if (num.Equals(11))
                    str3 = "01";
                num = boleto.Pagador.CPFCNPJ.Length;
                if (num.Equals(11))
                    str4 = "01";
                else if (IsNullOrEmpty(boleto.Pagador.CPFCNPJ))
                    str4 = "00";
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 219, 2, 0, str4, '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 221, 14, 0, boleto.Pagador.CPFCNPJ, '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 235, 37, 0, boleto.Pagador.Nome.ToUpper(), ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 272, 3, 0, Empty, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 275, 40, 0, boleto.Pagador.Endereco.LogradouroEndereco.ToUpper(), ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 315, 12, 0, boleto.Pagador.Endereco.Bairro.ToUpper(), ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 327, 8, 0, boleto.Pagador.Endereco.CEP, '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 335, 15, 0, boleto.Pagador.Endereco.Cidade.ToUpper(), ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 350, 2, 0, boleto.Pagador.Endereco.UF.ToUpper(), ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 352, 40, 0, Empty, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 392, 2, 0, str2, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 394, 1, 0, Empty, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 395, 6, 0, registro, '0'));
                tregistroEdi.CodificarLinha();
                return Utils.SubstituiCaracteresEspeciais(tregistroEdi.LinhaRegistro);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400.", ex);
            }
        }

        public string GerarHeaderRemessaCNAB400(ref int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                TRegistroEDI tregistroEdi = new TRegistroEDI();
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 1, 1, 0, "0", '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 2, 1, 0, "1", '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 3, 7, 0, "REMESSA", ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 10, 2, 0, "01", '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 12, 8, 0, "COBRANCA", ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 20, 7, 0, Empty, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 27, 4, 0, Beneficiario.ContaBancaria.Agencia, '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 31, 1, 0, Beneficiario.ContaBancaria.DigitoAgencia, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 32, 8, 0, Beneficiario.ContaBancaria.Conta.PadRight(8, '0').Substring(0, 8), '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 40, 1, 0, Beneficiario.ContaBancaria.DigitoConta, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 41, 6, 0, "000000", '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 47, 30, 0, Beneficiario.Nome.ToUpper(), ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 77, 18, 0, "085CECRED", ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediDataDDMMAA___________, 95, 6, 0, DateTime.Now, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 101, 7, 0, numeroArquivoRemessa, '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 108, 22, 0, Empty, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 130, 7, 0, Beneficiario.Codigo.ToString(), '0'));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 137, 258, 0, Empty, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 395, 6, 0, "000001", ' '));
                tregistroEdi.CodificarLinha();
                return Utils.SubstituiCaracteresEspeciais(tregistroEdi.LinhaRegistro);
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
                TRegistroEDI tregistroEdi = new TRegistroEDI();
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 1, 1, 0, "9", ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 2, 393, 0, Empty, ' '));
                tregistroEdi.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 395, 6, 0, numeroRegistroGeral, '0'));
                tregistroEdi.CodificarLinha();
                return Utils.SubstituiCaracteresEspeciais(tregistroEdi.LinhaRegistro);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do registro TRAILER do arquivo de REMESSA.", ex);
            }
        }

        public void LerDetalheRetornoCNAB400Segmento1(ref Boleto boleto, string registro)
        {
            try
            {
                boleto.NossoNumero = registro.Substring(47, 8);
                boleto.NossoNumeroDV = registro.Substring(55, 1);
                boleto.NossoNumeroFormatado = Format("{0}/{1}-{2}", boleto.NossoNumero.Substring(0, 2), boleto.NossoNumero.Substring(2, 6), boleto.NossoNumeroDV);
                boleto.CodigoMovimentoRetorno = registro.Substring(108, 2);
                boleto.DescricaoMovimentoRetorno = DescricaoOcorrenciaCnab400(boleto.CodigoMovimentoRetorno);
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));
                boleto.NumeroDocumento = registro.Substring(116, 10).Trim();
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(146, 6)).ToString("##-##-##"));
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(152, 13)) / 100M;
                boleto.ValorAbatimento = Convert.ToDecimal(registro.Substring(227, 13)) / 100M;
                boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(240, 13)) / 100M;
                boleto.ValorPago = Convert.ToDecimal(registro.Substring(253, 13)) / 100M;
                boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(266, 13)) / 100M;
                boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(279, 13)) / 100M;
                boleto.ValorPago += boleto.ValorJurosDia;
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(294, 6)).ToString("##-##-##"));
                boleto.CodigoMotivoOcorrencia = registro.Substring(381, 10);
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

        public void LerTrailerRetornoCNAB400(string registro)
        {
            throw new NotImplementedException();
        }
    }
}
