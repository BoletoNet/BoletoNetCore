using System;
using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using BoletoNetCore.Extensions;

namespace BoletoNetCore
{
    partial class BancoBrasil : IBancoCNAB400
    {

        #region Remessa - CNAB400

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
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0020, 007, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 004, 0, Beneficiario.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0031, 001, 0, Beneficiario.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0032, 008, 0, Beneficiario.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0040, 001, 0, Beneficiario.ContaBancaria.DigitoConta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0041, 006, 0, "000000", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 030, 0, Beneficiario.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0077, 018, 0, "001BANCODOBRASIL", ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0095, 006, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0101, 007, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0108, 022, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0130, 007, 0, Beneficiario.Codigo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0137, 258, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB400.", ex);
            }
        }

        private string GerarDetalheRemessaCNAB400Registro7(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "7", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 002, 0, boleto.Banco.Beneficiario.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 014, 0, boleto.Banco.Beneficiario.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 004, 0, boleto.Banco.Beneficiario.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0022, 001, 0, boleto.Banco.Beneficiario.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0023, 008, 0, boleto.Banco.Beneficiario.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0031, 001, 0, boleto.Banco.Beneficiario.ContaBancaria.DigitoConta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0032, 007, 0, boleto.Banco.Beneficiario.Codigo, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0039, 025, 0, boleto.NumeroControleParticipante, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0064, 017, 0, boleto.NossoNumero, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0081, 002, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0083, 002, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0085, 003, 0, string.Empty, ' ');
                if (boleto.Avalista.Nome == string.Empty)
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0088, 001, 0, string.Empty, ' ');
                else
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0088, 001, 0, string.Empty, 'A');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0089, 003, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0092, 003, 0, boleto.VariacaoCarteira, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0095, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0096, 006, 0, "0", '0');
                switch (boleto.TipoCarteira)
                {
                    case TipoCarteira.CarteiraCobrancaSimples:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0102, 005, 0, string.Empty, ' ');
                        break;
                    case TipoCarteira.CarteiraCobrancaVinculada:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0102, 005, 0, "02VIN", ' ');
                        break;
                    case TipoCarteira.CarteiraCobrancaDescontada:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0102, 005, 0, "04DSC", ' ');
                        break;
                    case TipoCarteira.CarteiraCobrancaVendor:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0102, 005, 0, "08VDR", ' ');
                        break;
                    default:
                        throw new Exception("Tipo de carteira não suportada: (" + boleto.TipoCarteira + ").");
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0107, 002, 0, boleto.Carteira, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, boleto.CodigoMovimentoRetorno, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0111, 010, 0, boleto.NumeroDocumento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0121, 006, 0, boleto.DataVencimento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 013, 2, boleto.ValorTitulo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 003, 0, "001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 004, 0, "0000", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0147, 001, 0, string.Empty, ' ');
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
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0235, 037, 0, boleto.Pagador.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0272, 003, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0275, 040, 0, boleto.Pagador.Endereco.FormataLogradouro(40), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0315, 012, 0, boleto.Pagador.Endereco.Bairro, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0327, 008, 0, boleto.Pagador.Endereco.CEP.Replace("-", ""), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0335, 015, 0, boleto.Pagador.Endereco.Cidade, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0350, 002, 0, boleto.Pagador.Endereco.UF, ' ');
                if (string.IsNullOrEmpty(boleto.Avalista.Nome))
                {
                    // Não tem avalista, utiliza a mensagem para o pagador
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 040, 0, boleto.MensagemArquivoRemessa, ' ');
                }
                else if (boleto.Avalista.TipoCPFCNPJ("A") == "F")
                {
                    // Avalista Pessoa Física
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 025, 0, boleto.Avalista.Nome, ' ');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0377, 001, 0, string.Empty, ' ');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0378, 003, 0, "CPF", ' ');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0381, 011, 0, boleto.Avalista.CPFCNPJ, ' ');
                }
                else
                {
                    // Avalista Pessoa Juridica
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 021, 0, boleto.Avalista.Nome, ' ');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0373, 001, 0, string.Empty, ' ');
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0374, 004, 0, "CNPJ", ' ');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0378, 014, 0, boleto.Avalista.CPFCNPJ, ' ');
                }

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0392, 002, 0, boleto.CodigoProtesto == TipoCodigoProtesto.ProtestarDiasCorridos ? boleto.DiasProtesto.ToString("00") : string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0394, 001, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400.", ex);
            }
        }

        private string GerarDetalheRemessaCNAB400Registro5Multa(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                if (boleto.ValorMulta == 0 && boleto.PercentualMulta == 0)
                    return "";
                
                var valorOuPercentualMulta = boleto.TipoCodigoMulta == Enums.TipoCodigoMulta.Valor ? boleto.ValorMulta : boleto.PercentualMulta;

                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "5", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 002, 0, "99", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 001, 0, (int)boleto.TipoCodigoMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0005, 006, 0, boleto.DataMulta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0011, 012, 2, valorOuPercentualMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0023, 003, 0, boleto.DiasLimiteRecebimento.HasValue ? boleto.DiasLimiteRecebimento.Value.ToString("000") : string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0026, 369, 0, string.Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
                    
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400.", ex);
            }
        }

        public string GerarTrailerRemessaCNAB400(int numeroRegistroGeral, decimal valorBoletoGeral, int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples, int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada, int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada, int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 001, 0, "9", ' '); //001-001
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 393, 0, string.Empty, ' '); //002-393
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0'); //395-400
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do registro TRAILER do arquivo de REMESSA.", ex);
            }
        }

        #endregion

        #region Retorno - CNAB400

        public override void LerHeaderRetornoCNAB400(string registro)
        {
            try
            {
                if (registro.Substring(0, 9) != "02RETORNO")
                    throw new Exception("O arquivo não é do tipo \"02RETORNO\"");

                this.Beneficiario = new Beneficiario();
                this.Beneficiario.ContaBancaria = new ContaBancaria();

                this.Beneficiario.ContaBancaria.Agencia = registro.Substring(26, 4);
                this.Beneficiario.ContaBancaria.DigitoAgencia = registro.Substring(30, 1);
                this.Beneficiario.ContaBancaria.Conta = registro.Substring(31, 8);
                this.Beneficiario.ContaBancaria.DigitoConta = registro.Substring(39, 1);
                this.Beneficiario.Nome = registro.Substring(46, 30).Trim();
                
                this.Beneficiario.Codigo = registro.Substring(149, 7).Trim();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler HEADER do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

        public void LerDetalheRetornoCNAB400Segmento1(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException();
        }

        public void LerDetalheRetornoCNAB400Segmento7(ref Boleto boleto, string registro)
        {
            try
            {
                //Nº Controle do Participante
                boleto.NumeroControleParticipante = registro.Substring(38, 25);

                //Carteira
                boleto.Carteira = registro.Substring(106, 2);
                boleto.VariacaoCarteira = registro.Substring(091, 3);
                switch (registro.Substring(80, 1))
                {
                    case "2":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaVinculada;
                        break;
                    case "4":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaDescontada;
                        break;
                    case "8":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaVendor;
                        break;
                    default:
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;
                        break;
                }

                //Identificação do Título no Banco
                boleto.NossoNumero = registro.Substring(63, 17); //Sem o DV
                boleto.NossoNumeroDV = ""; //DV
                boleto.NossoNumeroFormatado = boleto.NossoNumero;

                //Identificação de Ocorrência
                boleto.CodigoMovimentoRetorno = registro.Substring(108, 2);
                boleto.DescricaoMovimentoRetorno = DescricaoOcorrenciaCnab400(boleto.CodigoMovimentoRetorno);
                boleto.CodigoMotivoOcorrencia = registro.Substring(86, 2);

                //Número do Documento
                boleto.NumeroDocumento = registro.Substring(116, 10);
                boleto.EspecieDocumento = AjustaEspecieCnab400(registro.Substring(173, 2));

                //Valores do Título
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(152, 13)) / 100;
                boleto.ValorTarifas = Convert.ToDecimal(registro.Substring(181, 7)) / 100;
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
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(175, 6)).ToString("##-##-##"));

                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

        #endregion

        private static TipoEspecieDocumento AjustaEspecieCnab400(string codigoEspecie)
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
                case "08":
                    return TipoEspecieDocumento.LC;
                case "10":
                    return TipoEspecieDocumento.CH;
                case "12":
                    return TipoEspecieDocumento.DS;
                case "13":
                    return TipoEspecieDocumento.ND;
                case "15":
                    return TipoEspecieDocumento.AP;
                case "25":
                    return TipoEspecieDocumento.DAU;
                case "26":
                    return TipoEspecieDocumento.DAE;
                case "27":
                    return TipoEspecieDocumento.DAM;
                case "32":
                    return TipoEspecieDocumento.BP;
                default:
                    return TipoEspecieDocumento.OU;
            }
        }

        private static string AjustaEspecieCnab400(TipoEspecieDocumento especieDocumento)
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
                    return "08";
                case TipoEspecieDocumento.CH:
                    return "10";
                case TipoEspecieDocumento.DS:
                    return "12";
                case TipoEspecieDocumento.ND:
                    return "13";
                case TipoEspecieDocumento.AP:
                    return "15";
                case TipoEspecieDocumento.DAU:
                    return "25";
                case TipoEspecieDocumento.DAE:
                    return "26";
                case TipoEspecieDocumento.DAM:
                    return "27";
                case TipoEspecieDocumento.BP:
                    return "32";
                default:
                    return "99";
            }
        }

        private static string DescricaoOcorrenciaCnab400(string codigo)
        {
            switch (codigo)
            {
                case "02":
                    return "Confirmação de Entrada de Título";
                case "03":
                    return "Comando recusado";
                case "05":
                    return "Liquidado sem registro";
                case "06":
                    return "Liquidação normal";
                case "07":
                    return "Liquidação por Conta/Parcial";
                case "08":
                    return "Liquidação por Saldo";
                case "09":
                    return "Baixa de Titulo";
                case "10":
                    return "Baixa Solicitada";
                case "11":
                    return "Títulos em Ser";
                case "12":
                    return "Abatimento Concedido";
                case "13":
                    return "Abatimento Cancelado";
                case "14":
                    return "Alteração de Vencimento do título";
                case "15":
                    return "Liquidação em Cartório";
                case "16":
                    return "Confirmação de alteração de juros de mora";
                case "19":
                    return "Confirmação de recebimento de instruções para protesto";
                case "20":
                    return "Débito em Conta";
                case "21":
                    return "Alteração do Nome do Pagador";
                case "22":
                    return "Alteração do Endereço do Pagador";
                case "23":
                    return "Indicação de encaminhamento a cartório";
                case "24":
                    return "Sustar Protesto";
                case "25":
                    return "Dispensar Juros de mora";
                case "26":
                    return "Alteração do número do título dado pelo Beneficiário";
                case "28":
                    return "Manutenção de titulo vencido";
                case "31":
                    return "Conceder desconto";
                case "32":
                    return "Não conceder desconto";
                case "33":
                    return "Retificar desconto";
                case "34":
                    return "Alterar data para desconto";
                case "35":
                    return "Cobrar Multa";
                case "36":
                    return "Dispensar Multa";
                case "37":
                    return "Dispensar Indexador";
                case "38":
                    return "Dispensar prazo limite para recebimento";
                case "39":
                    return "Alterar prazo limite para recebimento";
                case "41":
                    return "Alteração do número do controle do participante";
                case "42":
                    return "Alteração do número do documento do pagador";
                case "44":
                    return "Título pago com cheque devolvido";
                case "46":
                    return "Título pago com cheque, aguardando compensação";
                case "72":
                    return "Alteração de tipo de cobrança";
                case "73":
                    return "Confirmação de Instrução de Parâmetro de Pagamento Parcial";
                case "96":
                    return "Despesas de Protesto";
                case "97":
                    return "Despesas de Sustação de Protesto";
                case "98":
                    return "Débito de Custas Antecipadas";
                default:
                    return "";
            }
        }

        public string GerarDetalheRemessaCNAB400(Boleto boleto, ref int numeroRegistro)
        {
            try
            {
                // Registro 7 - Obrigatório
                string detalhe = GerarDetalheRemessaCNAB400Registro7(boleto, ref numeroRegistro);

                // Registro 5 - Registro Opcional - Multa
                var strline = GerarDetalheRemessaCNAB400Registro5Multa(boleto, ref numeroRegistro);
                if (!string.IsNullOrWhiteSpace(strline))
                {
                    detalhe += Environment.NewLine;
                    detalhe += strline;
                }
                return detalhe;

            }
            catch (Exception ex)
            {
                throw BoletoNetCoreException.ErroAoGerarRegistroDetalheDoArquivoRemessa(ex);
            }
        }

        public void LerTrailerRetornoCNAB400(string registro)
        {

        }


    }
}