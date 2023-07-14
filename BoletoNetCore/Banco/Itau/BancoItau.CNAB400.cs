using System;
using System.Collections.Generic;
using System.Linq;
using BoletoNetCore.Exceptions;
using static System.String;

namespace BoletoNetCore
{
    partial class BancoItau : IBancoCNAB400
    {
        public string GerarDetalheRemessaCNAB400(Boleto boleto, ref int registro)
        {
            var detalhe = GerarDetalheRemessaCNAB400Registro1(boleto, ref registro);
            var strline = GerarDetalheRemessaCNAB400Registro2(boleto, ref registro);
            if (!IsNullOrWhiteSpace(strline))
            {
                detalhe += Environment.NewLine;
                detalhe += strline;
            }
            strline = GerarDetalheRemessaCNAB400Registro5(boleto, ref registro);
            if (!IsNullOrWhiteSpace(strline))
            {
                detalhe += Environment.NewLine;
                detalhe += strline;
            }
            return detalhe;
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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 004, 0, boleto.Banco.Beneficiario.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0022, 002, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 005, 0, boleto.Banco.Beneficiario.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0029, 001, 0, boleto.Banco.Beneficiario.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0030, 004, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0034, 004, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0038, 025, 0, boleto.NumeroControleParticipante, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0063, 008, 0, boleto.NossoNumero, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0071, 013, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0084, 003, 0, boleto.Carteira, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0087, 021, 0, Empty, ' ');

                switch (boleto.Carteira)
                {
                    case "153":
                    case "109":
                    case "112":
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 108, 001, 0, "I", ' ');
                        break;

                    default:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 108, 001, 0, "?", ' ');
                        break;
                }

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, boleto.CodigoMovimentoRetorno, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0111, 010, 0, boleto.NumeroDocumento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0121, 006, 0, boleto.DataVencimento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 013, 2, boleto.ValorTitulo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 003, 0, "341", '0');
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
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0235, 030, 0, boleto.Pagador.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0265, 010, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0275, 040, 0, boleto.Pagador.Endereco.FormataLogradouro(40), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0315, 012, 0, boleto.Pagador.Endereco.Bairro, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0327, 008, 0, boleto.Pagador.Endereco.CEP.Replace("-", ""), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0335, 015, 0, boleto.Pagador.Endereco.Cidade, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0350, 002, 0, boleto.Pagador.Endereco.UF, ' ');
                if (boleto.CodigoInstrucao1 == "94" | boleto.CodigoInstrucao2 == "94")
                {
                    // Mensagem com 40 posições (Elimina Avalista, Data Mora)
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 040, 0, boleto.MensagemArquivoRemessa, ' ');
                }
                else
                {
                    if (boleto.CodigoInstrucao1 == "93" | boleto.CodigoInstrucao2 == "93")
                    {
                        // Mensagem com 30 posições (Elimina Avalista)
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 030, 0, boleto.MensagemArquivoRemessa, ' ');
                    }
                    else
                    {
                        // Nome do Avalista
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 030, 0, boleto.Avalista.Nome, ' ');
                    }
                    reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0382, 004, 0, Empty, ' ');
                    var dataJuros = boleto.DataJuros >= boleto.DataVencimento ? boleto.DataJuros : boleto.DataVencimento;
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0386, 006, 0, dataJuros, '0');
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0392, 002, 0, boleto.DiasProtesto, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0394, 001, 0, Empty, ' ');
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
                if (boleto.PercentualMulta == 0 && boleto.ValorMulta == 0)
                    return "";

                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "2", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 001, 0, "2", '0');

                var dataMulta = boleto.DataMulta >= boleto.DataVencimento ? boleto.DataMulta : boleto.DataVencimento;
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0003, 008, 0, dataMulta, '0');

                if (boleto.PercentualMulta > 0)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0011, 013, 2, boleto.PercentualMulta, '0');
                else
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0011, 013, 2, CalcularValorPercentualMulta(boleto.ValorTitulo, boleto.ValorMulta), '0');

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0024, 371, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400 - Registro 2.", ex);
            }
        }

        private string GerarDetalheRemessaCNAB400Registro5(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                if (boleto.Avalista.Nome == "")
                    return "";

                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "5", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 120, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0122, 002, 0, boleto.Avalista.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0124, 014, 0, boleto.Avalista.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0138, 040, 0, boleto.Avalista.Endereco.FormataLogradouro(40), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0178, 012, 0, boleto.Avalista.Endereco.Bairro, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0190, 008, 0, boleto.Avalista.Endereco.CEP.Replace("-", ""), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0198, 015, 0, boleto.Avalista.Endereco.Cidade, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0213, 002, 0, boleto.Avalista.Endereco.UF, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0215, 180, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');

                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400 - Registro 2.", ex);
            }
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
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0012, 015, 0, "COBRANCA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 004, 0, Beneficiario.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0031, 002, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0033, 005, 0, Beneficiario.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0038, 001, 0, Beneficiario.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0039, 008, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 030, 0, Beneficiario.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0077, 003, 0, "341", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0080, 015, 0, "BANCO ITAU SA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0095, 006, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0101, 294, 0, Empty, ' ');
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

        public override void CompletarHeaderRetornoCNAB400(string registro)
        {
            // 01 - cpf / 02 - cnpj
            if (registro.Substring(1, 2) == "01")
                this.Beneficiario.CPFCNPJ = registro.Substring(6, 11);
            else
                this.Beneficiario.CPFCNPJ = registro.Substring(3, 14);
        }

        public override void LerHeaderRetornoCNAB400(string registro)
        {
            try
            {
                if (registro.Substring(0, 9) != "02RETORNO")
                    throw new Exception("O arquivo não é do tipo \"02RETORNO\"");

                if (registro.Substring(9, 2) == "04") 
                    this.DescontoDuplicatas = true; 

                this.Beneficiario = new Beneficiario();
                this.Beneficiario.ContaBancaria = new ContaBancaria();

                this.Beneficiario.ContaBancaria.Agencia = registro.Substring(26, 4);
                this.Beneficiario.ContaBancaria.Conta = registro.Substring(32, 5);
                this.Beneficiario.ContaBancaria.DigitoConta = registro.Substring(37, 1);
                this.Beneficiario.Nome = registro.Substring(46, 30).Trim();
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
                if (this.DescontoDuplicatas == false){
                    //Nº Controle do Participante
                    boleto.NumeroControleParticipante = registro.Substring(37, 25);

                    //Carteira
                    boleto.Carteira = registro.Substring(82, 3);
                    boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;

                    //Identificação do Título no Banco
                    boleto.NossoNumero = registro.Substring(85, 8);
                    boleto.NossoNumeroDV = registro.Substring(93, 1); //DV
                    boleto.NossoNumeroFormatado = $"{boleto.Carteira}/{boleto.NossoNumero}-{boleto.NossoNumeroDV}";

                    //Identificação de Ocorrência
                    boleto.CodigoMovimentoRetorno = registro.Substring(108, 2);
                    boleto.DescricaoMovimentoRetorno = DescricaoOcorrenciaCnab400(boleto.CodigoMovimentoRetorno);
                    boleto.CodigoMotivoOcorrencia = registro.Substring(377, 8);
                    boleto.ListMotivosOcorrencia = MotivoOcorrenciaCnab400(boleto.CodigoMotivoOcorrencia, boleto.CodigoMovimentoRetorno);

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
                    boleto.ValorPagoCredito = Convert.ToDecimal(registro.Substring(253, 13)) / 100;
                    boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(266, 13)) / 100;
                    boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(279, 13)) / 100;

                    //Data Ocorrência no Banco
                    boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));

                    //Data Vencimento do Título
                    boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(146, 6)).ToString("##-##-##"));

                    // Data do Crédito
                    boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(295, 6)).ToString("##-##-##"));

                    boleto.Pagador = new Pagador();
                    boleto.Pagador.Nome = registro.Substring(324, 30).Trim();
                }
                else
                {
                    //Nº Controle do Participante
                    boleto.NumeroControleParticipante = registro.Substring(37, 25);

                    //Carteira
                    boleto.Carteira = registro.Substring(82, 3);
                    boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaDescontada;

                    //Identificação do Título no Banco
                    boleto.NossoNumero = registro.Substring(85, 8);
                    boleto.NossoNumeroDV = registro.Substring(93, 1); //DV
                    boleto.NossoNumeroFormatado = $"{boleto.Carteira}/{boleto.NossoNumero}-{boleto.NossoNumeroDV}";

                    //Identificação de Ocorrência
                    boleto.CodigoMovimentoRetorno = registro.Substring(108, 2);
                    boleto.DescricaoMovimentoRetorno = DescricaoOcorrenciaCnab400(boleto.CodigoMovimentoRetorno);
                    boleto.CodigoMotivoOcorrencia = registro.Substring(377, 8);
                    boleto.ListMotivosOcorrencia = MotivoOcorrenciaCnab400(boleto.CodigoMotivoOcorrencia, boleto.CodigoMovimentoRetorno);

                    //Número do Documento
                    boleto.NumeroDocumento = registro.Substring(116, 10);
                    boleto.EspecieDocumento = AjustaEspecieCnab400(registro.Substring(173, 2));

                    //Valores do Título
                    boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(152, 13)) / 100;
                    boleto.ValorTarifas = Convert.ToDecimal(registro.Substring(175, 13)) / 100;
                    boleto.ValorPagoCredito = Convert.ToDecimal(registro.Substring(201, 13)) / 100;
                    boleto.ValorIOF = Convert.ToDecimal(registro.Substring(214, 13)) / 100;
                    boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(240, 13)) / 100;
                    boleto.ValorMulta = Convert.ToDecimal(registro.Substring(253, 13)) / 100;
                    boleto.ValorOutrasDespesas = Convert.ToDecimal(registro.Substring(94, 13)) / 100;
                    boleto.ValorOutrasDespesas += Convert.ToDecimal(registro.Substring(266, 13)) / 100;
                    boleto.ValorOutrasDespesas += Convert.ToDecimal(registro.Substring(279, 13)) / 100;

                    //Data Ocorrência no Banco
                    boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));

                    // Data do Crédito
                    boleto.DataCredito = boleto.DataProcessamento;

                    //Data Vencimento do Título
                    boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(146, 6)).ToString("##-##-##"));

                    boleto.Pagador = new Pagador();
                    boleto.Pagador.Nome = registro.Substring(324, 30).Trim();
                }

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

        private string DescricaoOcorrenciaCnab400(string codigo)
        {
            if (this.DescontoDuplicatas == false)
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
                    case "29":
                        return "Tarifa de Manutenção de Título Vencido";
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
            else
            {
                switch (codigo)
                {
                    case "02":
                        return "Desconto Aceito";
                    case "03":
                        return "Desconto Recusado";
                    case "04":
                        return "Alteração de Dados - Nova Entrada";
                    case "05":
                        return "Alteração de Dados - Baixa";
                    case "06":
                        return "Liquidação Normal";
                    case "08":
                        return "Liquidado em Cartório";
                    case "09":
                        return "Liquidado pelo Cedente";
                    case "12":
                        return "Abatimento";
                    case "14":
                        return "Vencimento Alterado";
                    case "16":
                        return "Instruções Rejeitadas";
                    case "19":
                        return "Confirmação Recebimento Instrução de Protesto";
                    case "21":
                        return "Confirmação Recebimento Instrução Não Protestar";
                    case "22":
                        return "Liquidação de Título Baixado";
                    case "25":
                        return "Alegação do Sacado";
                    case "34":
                        return "Custas de Sustação de Protesto";
                    case "48":
                        return "Liquidado pelo Cedente com Transferência para Cobrança";
                    case "62":
                        return "Débito Mensal de Tarifas";
                    case "63":
                        return "Título Enviado para Cobrança Simples";
                    default:
                        return "";
                }
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
                case "08":
                    return TipoEspecieDocumento.DS;
                case "09":
                    return TipoEspecieDocumento.LC;
                case "13":
                    return TipoEspecieDocumento.ND;
                case "15":
                    return TipoEspecieDocumento.DD;
                case "16":
                    return TipoEspecieDocumento.EC;
                case "18":
                    return TipoEspecieDocumento.BP;
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
                case TipoEspecieDocumento.DS:
                    return "08";
                case TipoEspecieDocumento.LC:
                    return "09";
                case TipoEspecieDocumento.ND:
                    return "13";
                case TipoEspecieDocumento.DD:
                    return "15";
                case TipoEspecieDocumento.EC:
                    return "16";
                case TipoEspecieDocumento.BP:
                    return "18";
                default:
                    return "99";
            }
        }

        public void LerTrailerRetornoCNAB400(string registro)
        {

        }

        private decimal CalcularValorPercentualMulta(decimal valorTitulo, decimal valorMulta)
        {
            return (valorMulta / valorTitulo) * 100;
        }

        /// <summary>
        /// Recupera a Lista dos Motivos de Ocorrência na Cobrança
        /// </summary>
        /// <remarks> Poderão ser
        /// informados até quatro ocorrências distintas, incidente sobre o título
        /// </remarks>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public static IEnumerable<string> MotivoOcorrenciaCnab400(string codigo, string codigoMovimentoRetorno)
        {
            //define qual o domínio que será utilizado, conforme C047
            var funcaoDominio = new string[] {  "02"  }.Contains(codigoMovimentoRetorno) ? MotivoOcorrenciaTabela10 :
                          new string[] { "03" }.Contains(codigoMovimentoRetorno) ? MotivoOcorrenciaTabela1 : null;

            //retorna uma lista vazia caso ele não encontre um domínio de motivos de ocorrência
            if (funcaoDominio == null) return Enumerable.Empty<string>();

            //classifica os motivos e inclui na lista de resultados
            List<string> motivos = new List<string>(4);
            for (int posicao = 0; posicao < codigo.Length - 1; posicao = posicao + 2)
            {
                var entrada = codigo.Substring(posicao, 2);
                if (entrada.Equals("00")) continue;
                motivos.Add(funcaoDominio(entrada)); //inclui o motivo a partir 
            }
            return motivos;
        }

        public static Func<string, string> MotivoOcorrenciaTabela1 { get; set; } =
            (q) =>
            {
                switch (q)
                {
                    case "03": return "Cep sem Atendimento de Protesto no Momento";
                    case "04": return "Sigla do Estado Inválida";
                    case "05": return "Data de Vencimento Inválida";
                    case "07": return "Valor do Título Maior que 10.000.000,00";
                    case "08": return "Nome do Pagador Não Informado";
                    case "09": return "Agencia Encerrada";
                    case "10": return "Endereço do Pagador Não Informado";
                    case "11": return "CEP Inválido";
                    case "12": return "Avalista Não Informado";
                    case "13": return "CEP Incompatível Com a Unidade da Federação";
                    case "14": return "Nosso Número Duplicado ou Fora da Faixa";
                    case "15": return "Nosso Número Duplicado no Mesmo Movimento";
                    case "18": return "Data de Entrada Inválida Para Esta Carteira";
                    case "19": return "Ocorrência Inválida";
                    case "21": return "Carteira Não Aceita Depositária Correspondente";
                    case "22": return "Carteira Não Permitida";
                    case "26": return "Agência / Conta Não Liberada Para Operar Com Cobrança";
                    case "27": return "CNPJ do Beneficiário Inapto";
                    case "29": return "Categoria da Conta Inválida";
                    case "30": return "Entradas Bloqueadas, Conta Suspensa em Cobrança";
                    case "31": return "Conta Não Tem Permissão Para Protestar";
                    case "35": return "IOF Maior que 5%";
                    case "36": return "Quantidade de Moeda Incompatível Com Valor do Título";
                    case "37": return "Número do Documento do Pagador Não Informado ou Inválido";
                    case "42": return "Nosso Número Fora da Faixa";
                    case "52": return "Empresa Não Aceita Banco Correspondente";
                    case "53": return "Empresa Não Aceita Banco Correspondente - Cobrança Mensagem";
                    case "54": return "Banco Correspondente - Título Com Vencimento Inferior a 15 Dias";
                    case "55": return "CEP Não Pertence à Depositária Informada";
                    case "56": return "Vencimento Superior a 180 Dias da Data de Entrada";
                    case "57": return "CEP Só Despositária Banco do Brasil Com Vencimento Inferior a 8 Dias";
                    case "60": return "Valor do Abatimento Inválido";
                    case "61": return "Juros de Mora Maior que o Permitido";
                    case "62": return "Valor do Desconto Maior que o Valor do Título";
                    case "63": return "Valor da Importância por Dia de Desconto Não Permitido";
                    case "64": return "Data de Emissão do Título Inválida";
                    case "65": return "Taxa Inválida";
                    case "66": return "Data de Vencimento Inválida / Fora de Prazo de Operação";
                    case "67": return "Valor do Título / Quantidade de Moeda Inválido";
                    case "68": return "Carteira Inválida ou Não Cadastrada no Intercâmbio da Cobrança";
                    case "69": return "Carteira Inválida Para Títulos Com Rateio de Crédito";
                    case "70": return "Beneficiário Não Cadastrado Para Fazer Rateio de Crédito";
                    case "78": return "Duplicidade de Agência / Conta Beneficiária do Rateio de Crédito";
                    case "80": return "Quantidade de Contas Beneficiárias do Rateio Maior do que o Permitido";
                    case "81": return "Conta Para Rateio de Crédito Inválida / Não Pertence ao Itaú";
                    case "82": return "Desconto / Abatimento Não Permitido Para Títulos Com Rateio de Crédito";
                    case "83": return "Valor do Título Menor que a Soma dos Valores Estipulados para Rateio";
                    case "84": return "Agência / Conta Beneficiária do Rateio é a Centralizadora de Crédito do Beneficiário";
                    case "85": return "Agência / Conta do Beneficiário é Contratual / Rateio de Crédito Não Permitido";
                    case "86": return "Código do Tipo de Valor Inválido / Não Previsto Para Títulos Com Rateio de Crédito";
                    case "87": return "Registro Tipo 4 Sem Informação de Agências / Contas Beneficiárias do Rateio";
                    case "90": return "Cobrança Mensagem - Número da Linha da Mensagem Inválido ou Quantidade de Linhas Excedidas";
                    case "97": return "Cobrança Mensagem Sem Mensagem, Porém Com Registro do Tipo 7 ou 8";
                    case "98": return "Registro Mensagem Sem Flash Cadastrado ou Flash Informado Diferente do Cadastrado";
                    case "99": return "Conta de Cobrança Com Flash Cadastrado e Sem Registro de Mensagem Correspondente";
                    default: return "";
                }
            };

        public static Func<string, string> MotivoOcorrenciaTabela10 { get; set; } =
            (q) =>
            {
                switch (q)
                {
                    case "01": return "Cep Sem Atendimento de Protesto no Momento";
                    case "02": return "Estado Com Determinação Legal que Impede a Incrição de Inadimplentes nos Cadastros de Proteção ao Crédito no Prazo Solicitado";
                    case "03": return "Boleto Não Liquidado no Desconto de Duplicatas e Transferido Para Cobrança Simples";
                    default: return "";
                }
            };
    }
}


