using System;
using System.Collections.Generic;
using System.Web.UI;
using Boleto2Net.Exceptions;
using static System.String;

[assembly: WebResource("BoletoNet.Imagens.341.jpg", "image/jpg")]

namespace Boleto2Net
{
    internal sealed class BancoItau : IBanco
    {
        internal static Lazy<IBanco> Instance { get; } = new Lazy<IBanco>(() => new BancoItau());

        public Cedente Cedente { get; set; }
        public int Codigo { get; } = 341;
        public string Nome { get; } = "Itaú";
        public string Digito { get; } = "7";
        public List<string> IdsRetornoCnab400RegistroDetalhe { get; } = new List<string> { "1" };
        public bool RemoveAcentosArquivoRemessa { get; } = true;

        public void FormataCedente()
        {
            var contaBancaria = Cedente.ContaBancaria;

            if (!CarteiraFactory<BancoItau>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw Boleto2NetException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados("ATÉ O VENCIMENTO EM QUALQUER BANCO. APÓS O VENCIMENTO SOMENTE NO ITAÚ.", "", "", 5);

            Cedente.CodigoFormatado = $"{contaBancaria.Agencia} / {contaBancaria.Conta}-{contaBancaria.DigitoConta}";
        }

        public void ValidaBoleto(Boleto boleto)
        {
        }

        public void FormataNossoNumero(Boleto boleto)
        {
            var carteira = CarteiraFactory<BancoItau>.ObterCarteira(boleto.CarteiraComVariacao);
            carteira.FormataNossoNumero(boleto);
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var carteira = CarteiraFactory<BancoItau>.ObterCarteira(boleto.CarteiraComVariacao);
            return carteira.FormataCodigoBarraCampoLivre(boleto);
        }

        public string GerarHeaderRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                var header = Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB400:
                        header += GerarHeaderRemessaCNAB400(numeroArquivoRemessa, ref numeroRegistroGeral);
                        break;
                    default:
                        throw new Exception("Tipo de arquivo inexistente.");
                }
                return header;
            }
            catch (Exception ex)
            {
                throw Boleto2NetException.ErroAoGerarRegistroHeaderDoArquivoRemessa(ex);
            }
        }

        public string GerarDetalheRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            try
            {
                string detalhe = Empty, strline = Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB400:
                        detalhe += GerarDetalheRemessaCNAB400Registro1(boleto, ref numeroRegistro);
                        strline = GerarDetalheRemessaCNAB400Registro2(boleto, ref numeroRegistro);
                        if (!IsNullOrWhiteSpace(strline))
                        {
                            detalhe += Environment.NewLine;
                            detalhe += strline;
                        }
                        strline = GerarDetalheRemessaCNAB400Registro5(boleto, ref numeroRegistro);
                        if (!IsNullOrWhiteSpace(strline))
                        {
                            detalhe += Environment.NewLine;
                            detalhe += strline;
                        }
                        break;
                    default:
                        throw new Exception("Tipo de arquivo inexistente.");
                }
                return detalhe;
            }
            catch (Exception ex)
            {
                throw Boleto2NetException.ErroAoGerarRegistroDetalheDoArquivoRemessa(ex);
            }
        }

        public string GerarTrailerRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa,
            ref int numeroRegistroGeral, decimal valorBoletoGeral,
            int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples,
            int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada,
            int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada,
            int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                var trailer = Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB400:
                        trailer = GerarTrailerRemessaCNAB400(ref numeroRegistroGeral);
                        break;
                    default:
                        throw new Exception("Tipo de arquivo inexistente.");
                }
                return trailer;
            }
            catch (Exception ex)
            {
                throw Boleto2NetException.ErroAoGerrarRegistroTrailerDoArquivoRemessa(ex);
            }
        }

        public void LerHeaderRetornoCNAB240(ArquivoRetorno arquivoRetorno, string registro)
        {
            throw new NotImplementedException();
        }

        public void LerDetalheRetornoCNAB240SegmentoT(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException();
        }

        public void LerDetalheRetornoCNAB240SegmentoU(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException();
        }

        public void LerHeaderRetornoCNAB400(string registro)
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
                boleto.NumeroControleParticipante = registro.Substring(37, 25);

                //Carteira
                boleto.Carteira = registro.Substring(82, 3);
                boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;

                //Identificação do Título no Banco
                boleto.NossoNumero = registro.Substring(85, 8);
                boleto.NossoNumeroDV = registro.Substring(93, 1); //DV
                boleto.NossoNumeroFormatado = $"{boleto.Carteira}/{boleto.NossoNumero}-{boleto.NossoNumeroDV}";

                //Identificação de Ocorrência
                boleto.CodigoOcorrencia = registro.Substring(108, 2);
                boleto.DescricaoOcorrencia = DescricaoOcorrenciaCnab400(boleto.CodigoOcorrencia);
                boleto.CodigoOcorrenciaAuxiliar = registro.Substring(377, 8);

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

        public string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            return "";
        }

        private string GerarHeaderRemessaCNAB400(int numeroArquivoRemessa, ref int numeroRegistroGeral)
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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 004, 0, Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0031, 002, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0033, 005, 0, Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0038, 001, 0, Cedente.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0039, 008, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 030, 0, Cedente.Nome, ' ');
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

        private string GerarDetalheRemessaCNAB400Registro1(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 002, 0, boleto.Banco.Cedente.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 014, 0, boleto.Banco.Cedente.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 004, 0, boleto.Banco.Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0022, 002, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 005, 0, boleto.Banco.Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0029, 001, 0, boleto.Banco.Cedente.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0030, 004, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0034, 004, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0038, 025, 0, boleto.NumeroControleParticipante, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0063, 008, 0, boleto.NossoNumero, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0071, 013, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0084, 003, 0, boleto.Carteira, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0087, 021, 0, Empty, ' ');

                switch (boleto.Carteira)
                {
                    case "109":
                    case "112":
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 108, 001, 0, "I", ' ');
                        break;

                    default:
                        reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 108, 001, 0, "?", ' ');
                        break;
                }

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, boleto.CodigoOcorrencia, ' ');
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

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0219, 002, 0, boleto.Sacado.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 014, 0, boleto.Sacado.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0235, 030, 0, boleto.Sacado.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0265, 010, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0275, 040, 0, boleto.Sacado.Endereco.FormataLogradouro(40), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0315, 012, 0, boleto.Sacado.Endereco.Bairro, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0327, 008, 0, boleto.Sacado.Endereco.CEP.Replace("-", ""), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0335, 015, 0, boleto.Sacado.Endereco.Cidade, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0350, 002, 0, boleto.Sacado.Endereco.UF, ' ');
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
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0386, 006, 0, "0", '0');
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0392, 002, 0, "0", '0');
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
                if (boleto.PercentualMulta == 0)
                    return "";

                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "2", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 001, 0, "2", '0');

                var dataMulta = boleto.DataMulta >= boleto.DataVencimento ? boleto.DataMulta : boleto.DataVencimento;
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0003, 008, 0, dataMulta, '0');

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0011, 013, 2, boleto.PercentualMulta, '0');

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

        private string GerarTrailerRemessaCNAB400(ref int numeroRegistroGeral)
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
    }
}


