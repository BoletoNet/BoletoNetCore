using System;
using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using BoletoNetCore.Extensions;

namespace BoletoNetCore
{
    partial class BancoBanrisul : IBancoCNAB400
    {
        #region Remessa - CNAB400

        public string GerarDetalheRemessaCNAB400(Boleto boleto, ref int registro)
        {
            // Registro 1 - Obrigatório
            var detalhe = GerarDetalheRemessaCNAB400Registro1(boleto, ref registro);

            // Registro 1 Adicional - Registro Opcional
            var strline = GerarDetalheRemessaCNAB400Registro1Mensagem(boleto, ref registro);
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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 009, 0, "01REMESSA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0010, 017, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0027, 004, 0, Beneficiario.ContaBancaria.Agencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0031, 008, 0, Beneficiario.ContaBancaria.Conta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0039, 001, 0, Beneficiario.ContaBancaria.DigitoConta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0040, 007, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 030, 0, Beneficiario.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0077, 011, 0, "041BANRISUL", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0088, 007, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0095, 006, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0101, 009, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0110, 004, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0114, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0115, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0116, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0117, 010, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0127, 268, 0, string.Empty, ' ');
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
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 026, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0028, 013, 2, valorBoletoGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0041, 354, 0, string.Empty, ' ');
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


        

        private string GerarDetalheRemessaCNAB400Registro1(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 016, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0018, 004, 0, boleto.Banco.Beneficiario.ContaBancaria.Agencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0022, 008, 0, boleto.Banco.Beneficiario.ContaBancaria.Conta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0030, 001, 0, boleto.Banco.Beneficiario.ContaBancaria.DigitoConta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0031, 007, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0038, 025, 0, boleto.NumeroControleParticipante, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0063, 010, 0, boleto.NossoNumero + boleto.NossoNumeroDV, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0073, 032, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0105, 003, 0, string.Empty, ' ');
                switch (boleto.TipoCarteira)
                {
                    case TipoCarteira.CarteiraCobrancaSimples:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0108, 001, 0, "1", ' ');
                        break;
                    case TipoCarteira.CarteiraCobrancaVinculada:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0108, 001, 0, "C", ' ');
                        break;
                    case TipoCarteira.CarteiraCobrancaCaucionada:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0108, 001, 0, "3", ' ');
                        break;
                    case TipoCarteira.CarteiraCobrancaDescontada:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0108, 001, 0, "R", ' ');
                        break;
                    case TipoCarteira.CarteiraCobrancaVendor:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0108, 001, 0, "X", ' ');
                        break;
                    default:
                        throw new Exception("Tipo de carteira não suportada: (" + boleto.TipoCarteira + ").");
                }
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, boleto.CodigoMovimentoRetorno, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0111, 010, 0, boleto.NumeroDocumento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0121, 006, 0, boleto.DataVencimento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 013, 2, boleto.ValorTitulo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 003, 0, "041", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0143, 005, 0, string.Empty, ' ');
                switch (boleto.Banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto)
                {
                    case TipoImpressaoBoleto.Banco:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0148, 002, 0, "06", '0');
                        break;
                    case TipoImpressaoBoleto.Empresa:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0148, 002, 0, "08", '0');
                        break;
                }
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0150, 001, 0, boleto.Aceite, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0151, 006, 0, boleto.DataEmissao, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0157, 002, 0, boleto.CodigoInstrucao1, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0159, 002, 0, boleto.CodigoInstrucao2, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0161, 001, 2, '0', '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0162, 012, 2, boleto.ValorJurosDia, '0');
                if (boleto.ValorDesconto == 0)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0174, 006, 0, "0", '0'); // Sem Desconto
                else
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0174, 006, 0, boleto.DataDesconto, '0'); // Com Desconto
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0180, 013, 2, boleto.ValorDesconto, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0193, 013, 2, boleto.ValorIOF, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0206, 013, 2, boleto.ValorAbatimento, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0219, 002, 0, boleto.Pagador.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 014, 0, boleto.Pagador.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0235, 035, 0, boleto.Pagador.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0270, 005, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0275, 040, 0, boleto.Pagador.Endereco.FormataLogradouro(40), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0315, 007, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0322, 003, 1, boleto.PercentualMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0325, 002, 0, 0, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0327, 008, 0, boleto.Pagador.Endereco.CEP.Replace("-", ""), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0335, 015, 0, boleto.Pagador.Endereco.Cidade, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0350, 002, 0, boleto.Pagador.Endereco.UF, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0352, 004, 0, 0, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0356, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0357, 013, 0, 0, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0370, 002, 0, boleto.DiasProtesto, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0372, 023, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400.", ex);
            }
        }

        private string GerarDetalheRemessaCNAB400Registro1Mensagem(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(boleto.MensagemArquivoRemessa))
                    return "";

                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 002, 0, boleto.Banco.Beneficiario.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 014, 0, boleto.Banco.Beneficiario.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0018, 004, 0, boleto.Banco.Beneficiario.ContaBancaria.Agencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0022, 008, 0, boleto.Banco.Beneficiario.ContaBancaria.Conta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0030, 001, 0, boleto.Banco.Beneficiario.ContaBancaria.DigitoConta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0031, 007, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0038, 025, 0, boleto.NumeroControleParticipante, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0063, 010, 0, boleto.NossoNumero + boleto.NossoNumeroDV, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0073, 035, 0, string.Empty, ' ');
                switch (boleto.TipoCarteira)
                {
                    case TipoCarteira.CarteiraCobrancaSimples:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0108, 001, 0, "1", ' ');
                        break;
                    case TipoCarteira.CarteiraCobrancaVinculada:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0108, 001, 0, "C", ' ');
                        break;
                    case TipoCarteira.CarteiraCobrancaCaucionada:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0108, 001, 0, "3", ' ');
                        break;
                    case TipoCarteira.CarteiraCobrancaDescontada:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0108, 001, 0, "R", ' ');
                        break;
                    case TipoCarteira.CarteiraCobrancaVendor:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0108, 001, 0, "X", ' ');
                        break;
                    default:
                        throw new Exception("Tipo de carteira não suportada: (" + boleto.TipoCarteira + ").");
                }
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, "98", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0111, 001, 0, "1", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0112, 090, 0, boleto.MensagemArquivoRemessa, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0202, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0203, 090, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0293, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0294, 090, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0384, 011, 0, string.Empty, ' ');
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
                if (registro.Substring(76, 11) != "041BANRISUL")
                    throw new Exception("O arquivo não é do tipo \"041BANRISUL\"");
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
                boleto.NossoNumero = registro.Substring(62, 8); //Sem o DV
                boleto.NossoNumeroDV = registro.Substring(70, 2); //DV
                boleto.NossoNumeroFormatado = boleto.NossoNumero + "-" + boleto.NossoNumeroDV;

                // Carteira
                boleto.Carteira = registro.Substring(107, 1);
                boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;

                // Identificação de Ocorrência
                boleto.CodigoMovimentoRetorno = registro.Substring(108, 2);
                boleto.DescricaoMovimentoRetorno = DescricaoOcorrenciaCnab400(boleto.CodigoMovimentoRetorno);

                // Data Ocorrência no Banco
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));

                // Número do Documento
                boleto.NumeroDocumento = registro.Substring(116, 10);

                //Data Vencimento do Título
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(146, 6)).ToString("##-##-##"));

                //Valores do Título
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(152, 13)) / 100;
                boleto.ValorTarifas = Convert.ToDecimal(registro.Substring(175, 13)) / 100;
                boleto.ValorOutrasDespesas = Convert.ToDecimal(registro.Substring(188, 13)) / 100;
                boleto.ValorAbatimento = Convert.ToDecimal(registro.Substring(227, 13)) / 100;
                boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(240, 13)) / 100;
                boleto.ValorPago = Convert.ToDecimal(registro.Substring(253, 13)) / 100;
                boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(266, 13)) / 100;
                boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(279, 13)) / 100;

                // ATENÇÃO:
                // Conforme a página 21 (nota 1) do manual CobrancaEletronicaBanrisul_pdr400_vrs15072015_ed06.pdf:
                // O padrão do "Valor Pago" considera apenas Valor do título – Descontos + Outros recebimentos – Abatimentos – Outras despesas
                // O valor dos juros não é somado, apesar de ser destacado na posição 267 a 279.
                // Para seguir o padrão dos outros bancos implementados no BoletoNetCore, e não precisar tratar isso na aplicação, o valor dos juros está sendo somado ao ValorPago
                boleto.ValorPago += boleto.ValorJurosDia;

                // Data do Crédito
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(295, 6)).ToString("##-##-##"));

                // Identificação de Ocorrência - Código Auxiliar
                boleto.CodigoMotivoOcorrencia = registro.Substring(382, 10);

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

        #endregion

    }
}