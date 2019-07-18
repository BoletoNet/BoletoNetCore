using System;
using System.Collections.Generic;
using Boleto2Net.Exceptions;
using static System.String;

namespace Boleto2Net
{
    internal sealed class BancoSicoob : IBanco
    {
        internal static Lazy<IBanco> Instance { get; } = new Lazy<IBanco>(() => new BancoSicoob());

        private BancoSicoob()
        {
            IdsRetornoCnab400RegistroDetalhe.Add("1");
        }

        public Cedente Cedente { get; set; }

        public int Codigo { get; } = 756;
        public string Nome { get; } = "Sicoob";
        public string Digito { get; } = "0";
        public List<string> IdsRetornoCnab400RegistroDetalhe { get; } = new List<string> { "1" };
        public bool RemoveAcentosArquivoRemessa { get; } = true;

        public void FormataCedente()
        {
            var contaBancaria = Cedente.ContaBancaria;

            if (!CarteiraFactory<BancoSicoob>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw Boleto2NetException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            var codigoCedente = Cedente.Codigo;
            if (Cedente.CodigoDV == Empty)
                throw new Exception($"Dígito do código do cedente ({codigoCedente}) não foi informado.");

            contaBancaria.FormatarDados("PAGÁVEL EM QUALQUER BANCO ATÉ A DATA DE VENCIMENTO.", "", "", 8);

            Cedente.Codigo = codigoCedente.Length <= 6 ? codigoCedente.PadLeft(6, '0'): throw Boleto2NetException.CodigoCedenteInvalido(codigoCedente, 6);

            Cedente.CodigoFormatado = $"{contaBancaria.Agencia} / {codigoCedente}-{Cedente.CodigoDV}";
        }

        public void ValidaBoleto(Boleto boleto)
        {
        }

        public void FormataNossoNumero(Boleto boleto)
        {
            var carteira = CarteiraFactory<BancoSicoob>.ObterCarteira(boleto.CarteiraComVariacao);
            carteira.FormataNossoNumero(boleto);
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var carteira = CarteiraFactory<BancoSicoob>.ObterCarteira(boleto.CarteiraComVariacao);
            return carteira.FormataCodigoBarraCampoLivre(boleto);
        }

        public string GerarHeaderRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                string header = Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        // Cabeçalho do Arquivo
                        header += GerarHeaderRemessaCNAB240(numeroArquivoRemessa, ref numeroRegistroGeral);
                        // Cabeçalho do Lote
                        header += Environment.NewLine;
                        header += GerarHeaderLoteRemessaCNAB240(numeroArquivoRemessa, ref numeroRegistroGeral);
                        break;
                    case TipoArquivo.CNAB400:
                        header += GerarHeaderRemessaCNAB400(numeroArquivoRemessa, ref numeroRegistroGeral);
                        break;
                    default:
                        throw new Exception("Header - Tipo de arquivo inexistente.");
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
                string detalhe = Empty;
                string strline = Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        // Segmento P (Obrigatório)
                        detalhe += GerarDetalheSegmentoPRemessaCNAB240(boleto, ref numeroRegistro);

                        // Segmento Q (Obrigatório)
                        detalhe += Environment.NewLine;
                        detalhe += GerarDetalheSegmentoQRemessaCNAB240(boleto, ref numeroRegistro);

                        // Segmento R (Opcional)
                        strline = GerarDetalheSegmentoRRemessaCNAB240(boleto, ref numeroRegistro);
                        if (!IsNullOrWhiteSpace(strline))
                        {
                            detalhe += Environment.NewLine;
                            detalhe += strline;
                        }
                        // Segmento S (Opcional)
                        strline = GerarDetalheSegmentoSRemessaCNAB240(boleto, ref numeroRegistro);
                        if (!IsNullOrWhiteSpace(strline))
                        {
                            detalhe += Environment.NewLine;
                            detalhe += strline;
                        }


                        break;

                    case TipoArquivo.CNAB400:

                        // Registro 1 - Obrigatório
                        detalhe += GerarDetalheRemessaCNAB400Registro1(boleto, ref numeroRegistro);

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
                string trailer = Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        // Trailler do Lote
                        trailer += GerarTrailerLoteRemessaCNAB240(ref numeroRegistroGeral,
                                                                        numeroRegistroCobrancaSimples, valorCobrancaSimples,
                                                                        numeroRegistroCobrancaVinculada, valorCobrancaVinculada,
                                                                        numeroRegistroCobrancaCaucionada, valorCobrancaCaucionada,
                                                                        numeroRegistroCobrancaDescontada, valorCobrancaDescontada);
                        // Trailler do Arquivo
                        trailer += Environment.NewLine;
                        trailer += GerarTrailerRemessaCNAB240(ref numeroRegistroGeral);
                        break;

                    case TipoArquivo.CNAB400:
                        trailer += GerarTrailerRemessaCNAB400(ref numeroRegistroGeral);
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


        #region Remessa - CNAB240
        private string GerarHeaderRemessaCNAB240(int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "756", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0000", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, Cedente.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 014, 0, Cedente.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0033, 020, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0053, 005, 0, Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0058, 001, 0, Cedente.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0059, 012, 0, Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0071, 001, 0, Cedente.ContaBancaria.DigitoConta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0072, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0073, 030, 0, Cedente.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0103, 030, 0, "SICOOB", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0133, 010, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0144, 008, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediHoraHHMMSS___________, 0152, 006, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0158, 006, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0164, 003, 0, "081", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0167, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0172, 020, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0192, 020, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0212, 029, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB240.", ex);
            }
        }
        private string GerarHeaderLoteRemessaCNAB240(int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "756", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 001, 0, "R", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0010, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0012, 002, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0014, 003, 0, "040", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0017, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, Cedente.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 015, 0, Cedente.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0034, 020, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0054, 005, 0, Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0059, 001, 0, Cedente.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0060, 012, 0, Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0072, 001, 0, Cedente.ContaBancaria.DigitoConta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0073, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0074, 030, 0, Cedente.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0104, 040, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0144, 040, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0184, 008, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0192, 008, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0200, 008, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0208, 033, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do lote no arquivo de remessa do CNAB400.", ex);
            }
        }
        private string GerarDetalheSegmentoPRemessaCNAB240(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "756", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, numeroRegistroGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "P", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 002, 0, boleto.CodigoOcorrencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 005, 0, boleto.Banco.Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0023, 001, 0, boleto.Banco.Cedente.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 012, 0, boleto.Banco.Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0036, 001, 0, boleto.Banco.Cedente.ContaBancaria.DigitoConta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0037, 001, 0, Empty, ' ');
                if (boleto.Banco.Cedente.ContaBancaria.TipoImpressaoBoleto == TipoImpressaoBoleto.Banco)
                {
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0038, 009, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0047, 001, 0, "0", '0');
                }
                else
                {
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0038, 009, 0, boleto.NossoNumero, '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0047, 001, 0, boleto.NossoNumeroDV, '0');
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0048, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0050, 002, 0, boleto.VariacaoCarteira, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0052, 001, 0, "4", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0053, 005, 0, Empty, ' ');

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0058, 001, 0, boleto.Carteira, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0059, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0060, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0061, 001, 0, (int)boleto.Banco.Cedente.ContaBancaria.TipoImpressaoBoleto, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0062, 001, 0, (int)boleto.Banco.Cedente.ContaBancaria.TipoImpressaoBoleto, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0063, 015, 0, boleto.NumeroDocumento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0078, 008, 0, boleto.DataVencimento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0086, 015, 2, boleto.ValorTitulo, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0101, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0106, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0107, 002, 0, (int)boleto.EspecieDocumento, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 001, 0, boleto.Aceite, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0110, 008, 0, boleto.DataEmissao, '0');
                if (boleto.ValorJurosDia == 0)
                {
                    // Sem Juros Mora
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0118, 001, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0119, 008, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 015, 2, 0, '0');
                }
                else
                {
                    // Com Juros Mora ($)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0118, 001, 0, "1", '0');
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0119, 008, 0, boleto.DataJuros, '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 015, 2, boleto.ValorJurosDia, '0');
                }
                if (boleto.ValorDesconto == 0)
                {
                    // Sem Desconto
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0142, 001, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 008, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0151, 015, 2, "0", '0');
                }
                else
                {
                    // Com Desconto
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0142, 001, 0, "1", '0');
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0143, 008, 0, boleto.DataDesconto, '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0151, 015, 2, boleto.ValorDesconto, '0');
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0166, 015, 2, boleto.ValorIOF, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0181, 015, 2, boleto.ValorAbatimento, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0196, 025, 0, boleto.NumeroControleParticipante, ' ');
                switch (boleto.CodigoProtesto)
                {
                    case TipoCodigoProtesto.ProtestarDiasCorridos:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 001, 0, 1, '0');
                        break;
                    case TipoCodigoProtesto.ProtestarDiasUteis:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 001, 0, 2, '0');
                        break;
                    case TipoCodigoProtesto.NaoProtestar:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 001, 0, 3, '0');
                        break;
                    default:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 001, 0, 0, '0');
                        break;
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0222, 002, 0, boleto.DiasProtesto, '0');

                #region
                //Seq       De      Até     Dig     Dec     Form    Campo/Descrição             Conteudo
                //38.3P     224     224     001     -       Num     Código p/ Baixa/Devolução   Código para Baixa/Devolução: "0"
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0224, 001, 0, "0", '0');
                #endregion

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0225, 003, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0228, 002, 0, "09", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0230, 010, 2, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0240, 001, 0, Empty, ' ');
                reg.CodificarLinha();
                string vLinha = reg.LinhaRegistro;
                return vLinha;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do Segmento P no arquivo de remessa do CNAB240.", ex);
            }

        }
        private string GerarDetalheSegmentoQRemessaCNAB240(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "756", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, numeroRegistroGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "Q", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 002, 0, boleto.CodigoOcorrencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, boleto.Sacado.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 015, 0, boleto.Sacado.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0034, 040, 0, boleto.Sacado.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0074, 040, 0, boleto.Sacado.Endereco.FormataLogradouro(40), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0114, 015, 0, boleto.Sacado.Endereco.Bairro, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0129, 008, 0, boleto.Sacado.Endereco.CEP.Replace("-", ""), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0137, 015, 0, boleto.Sacado.Endereco.Cidade, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0152, 002, 0, boleto.Sacado.Endereco.UF, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0154, 001, 0, boleto.Avalista.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0155, 015, 0, boleto.Avalista.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0170, 040, 0, boleto.Avalista.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0210, 003, 0, boleto.Banco.Cedente.ContaBancaria.CodigoBancoCorrespondente, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0213, 020, 0, boleto.Banco.Cedente.ContaBancaria.NossoNumeroBancoCorrespondente, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0233, 008, 0, Empty, ' ');
                reg.CodificarLinha();
                string vLinha = reg.LinhaRegistro;
                return vLinha;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do Segmento Q no arquivo de remessa do CNAB240.", ex);
            }
        }
        private string GerarDetalheSegmentoRRemessaCNAB240(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                var codMulta = "0";
                if (boleto.ValorMulta > 0)
                    codMulta = "1";


                if (codMulta == "0")
                {
                    // Se não tiver informação sobre Multa, não precisa gerar o registro.
                    return "";
                }

                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "756", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, numeroRegistroGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "R", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 002, 0, boleto.CodigoOcorrencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 008, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 015, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0042, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0043, 008, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0051, 015, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0066, 001, 0, codMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0067, 008, 0, boleto.DataMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0075, 015, 2, boleto.ValorMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0090, 010, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0100, 040, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 040, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0180, 020, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0200, 008, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0208, 003, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0211, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0216, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0217, 012, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0229, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0230, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0231, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0232, 009, 0, Empty, ' ');
                reg.CodificarLinha();
                string vLinha = reg.LinhaRegistro;
                return vLinha;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do Segmento Q no arquivo de remessa do CNAB240.", ex);
            }
        }


        private string GerarDetalheSegmentoSRemessaCNAB240(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                var msg5A9 = boleto.MensagemArquivoRemessa.PadRight(500, ' ').Substring(0, 200).FitStringLength(200, ' ');
                if (IsNullOrWhiteSpace(msg5A9))
                    return "";

                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "756", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, numeroRegistroGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "S", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0019, 200, 0, msg5A9, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0219, 022, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do Segmento S no arquivo de remessa do CNAB240.", ex);
            }
        }

        private string GerarTrailerLoteRemessaCNAB240(ref int numeroRegistroGeral,
                                                                int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples,
                                                                int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada,
                                                                int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada,
                                                                int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                // O número de registros no lote é igual ao número de registros gerados + 2 (header e trailler do lote)
                int numeroRegistrosNoLote = numeroRegistroGeral + 2;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "756", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "5", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 006, 0, numeroRegistrosNoLote, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 006, 0, numeroRegistroCobrancaSimples, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0030, 017, 2, valorCobrancaSimples, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0047, 006, 0, numeroRegistroCobrancaVinculada, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0053, 017, 2, valorCobrancaVinculada, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0070, 006, 0, numeroRegistroCobrancaCaucionada, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0076, 017, 2, valorCobrancaCaucionada, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0093, 006, 0, numeroRegistroCobrancaDescontada, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0099, 017, 2, valorCobrancaDescontada, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0116, 008, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0124, 117, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do lote no arquivo de remessa do CNAB400.", ex);
            }
        }
        private string GerarTrailerRemessaCNAB240(ref int numeroRegistroGeral)
        {
            try
            {
                // O número de registros no arquivo é igual ao número de registros gerados + 4 (header e trailler do lote / header e trailler do arquivo)
                int numeroRegistrosNoArquivo = numeroRegistroGeral + 4;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "756", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "9999", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "9", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 006, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 006, 0, numeroRegistrosNoArquivo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0030, 006, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0036, 205, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB400.", ex);
            }
        }

        #endregion

        #region Remessa - CNAB400
        private string GerarHeaderRemessaCNAB400(int numeroArquivoRemessa, ref int numeroRegistroGeral)
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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 004, 0, Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0031, 001, 0, Cedente.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0032, 008, 0, Cedente.Codigo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0040, 001, 0, Cedente.CodigoDV, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0041, 006, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 030, 0, Cedente.Nome, ' ');
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
        private string GerarDetalheRemessaCNAB400Registro1(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 002, 0, boleto.Banco.Cedente.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 014, 0, boleto.Banco.Cedente.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 004, 0, boleto.Banco.Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0022, 001, 0, boleto.Banco.Cedente.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0023, 008, 0, boleto.Banco.Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0031, 001, 0, boleto.Banco.Cedente.ContaBancaria.DigitoConta, ' ');
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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0106, 001, 0, (int)boleto.Banco.Cedente.ContaBancaria.TipoImpressaoBoleto, '0');

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
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, boleto.CodigoOcorrencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0111, 010, 0, boleto.NumeroDocumento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0121, 006, 0, boleto.DataVencimento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 013, 2, boleto.ValorTitulo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 003, 0, "756", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 004, 0, boleto.Banco.Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0147, 001, 0, boleto.Banco.Cedente.ContaBancaria.DigitoAgencia, ' ');
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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0173, 001, 0, (int)boleto.Banco.Cedente.ContaBancaria.TipoImpressaoBoleto, '0');

                if (boleto.ValorDesconto == 0)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0174, 006, 0, "000000", '0');   // Sem Desconto
                else
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0174, 006, 0, boleto.DataDesconto, '0');   // Com Desconto

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0180, 013, 2, boleto.ValorDesconto, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0193, 001, 0, boleto.CodigoMoeda, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0194, 012, 2, boleto.ValorIOF, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0206, 013, 2, boleto.ValorAbatimento, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0219, 002, 0, boleto.Sacado.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 014, 0, boleto.Sacado.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0235, 040, 0, boleto.Sacado.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0275, 037, 0, boleto.Sacado.Endereco.FormataLogradouro(37), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0312, 015, 0, boleto.Sacado.Endereco.Bairro, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0327, 008, 0, boleto.Sacado.Endereco.CEP.Replace("-", ""), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0335, 015, 0, boleto.Sacado.Endereco.Cidade, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0350, 002, 0, boleto.Sacado.Endereco.UF, ' ');
                if (IsNullOrEmpty(boleto.Avalista.Nome))
                {
                    // Mensagem para o sacado
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
        private string GerarTrailerRemessaCNAB400(ref int numeroRegistroGeral)
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
        #endregion

        #region Retorno - CNAB240
        public void LerHeaderRetornoCNAB240(ArquivoRetorno arquivoRetorno, string registro)
        {
            arquivoRetorno.Banco.Cedente = new Cedente();
            //05.0	018	018	001 - Tipo de Inscrição da Empresa: '1' = CPF '2' = CGC / CNPJ
            //06.0	019	032	014 - Número de Inscrição da Empresa
            arquivoRetorno.Banco.Cedente.CPFCNPJ = registro.Substring(17, 1) == "1" ? registro.Substring(21, 11) : registro.Substring(18, 14);

            //arquivoRetorno.Banco.Cedente.Codigo = ?;
            //13.0	073	102	030 - Nome da Empresa
            arquivoRetorno.Banco.Cedente.Nome = registro.Substring(72, 30).Trim();


            arquivoRetorno.Banco.Cedente.ContaBancaria = new ContaBancaria();
            //08.0	053	057	005	- Prefixo da Cooperativa: vide planilha "Capa" deste arquivo
            arquivoRetorno.Banco.Cedente.ContaBancaria.Agencia = registro.Substring(52, 5);
            //09.0	058	058	001 - Dígito Verificador do Prefixo: vide planilha "Capa" deste arquivo
            arquivoRetorno.Banco.Cedente.ContaBancaria.DigitoAgencia = registro.Substring(57, 1);
            //10.0	059	070	012 - Conta Corrente: vide planilha "Capa" deste arquivo
            arquivoRetorno.Banco.Cedente.ContaBancaria.Conta = registro.Substring(58, 12);
            //11.0	071	071	001 - Dígito Verificador da Conta: vide planilha "Capa" deste arquivo
            arquivoRetorno.Banco.Cedente.ContaBancaria.DigitoConta = registro.Substring(70, 1);


            //17.0 144 151 008 - Num - Data de Geração do Arquivo
            arquivoRetorno.DataGeracao = Utils.ToDateTime(Utils.ToInt32(registro.Substring(143, 8)).ToString("##-##-####"));
            //19.0 158 163 006 - Num - Seqüência (NSA)
            arquivoRetorno.NumeroSequencial = Utils.ToInt32(registro.Substring(157, 6));
        }
        public void LerDetalheRetornoCNAB240SegmentoT(ref Boleto boleto, string registro)
        {
            try
            {
                //Nº Controle do Participante
                boleto.NumeroControleParticipante = registro.Substring(105, 25);

                //Carteira
                boleto.Carteira = registro.Substring(57, 1);
                switch (boleto.Carteira)
                {
                    case "3":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaCaucionada;
                        break;
                    default:
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;
                        break;
                }

                //Identificação do Título no Banco
                //Conforme Manual, Nosso Número deve ter 7 dígitos + DV.
                //No arquivo retorno, volta com 10 dígitos. Entendemos que os 2 primeiros serão sempre 00, seguido de 7 dígitos + 1 dígito DV.
                //Se isso não ocorrer, será necessário alterar o método FormatarNossoNumero (onde considera apenas 7 digitos).
                if (registro.Substring(37, 2) != "00")
                    throw new Exception("Verificar arquivo retorno:  O nosso número no arquivo retorno é maior que 7 dígitos.");
                boleto.NossoNumero = registro.Substring(39, 7); //Sem o DV
                boleto.NossoNumeroDV = registro.Substring(46, 1); //DV
                boleto.NossoNumeroFormatado = $"{boleto.NossoNumero}-{boleto.NossoNumeroDV}";

                //Identificação de Ocorrência
                boleto.CodigoOcorrencia = registro.Substring(15, 2);
                boleto.DescricaoOcorrencia = Cnab.OcorrenciaCnab240(boleto.CodigoOcorrencia);
                boleto.CodigoOcorrenciaAuxiliar = registro.Substring(213, 10);

                //Número do Documento
                boleto.NumeroDocumento = registro.Substring(58, 15);
                boleto.EspecieDocumento = TipoEspecieDocumento.NaoDefinido;

                //Valor do Título
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(81, 15)) / 100;
                boleto.ValorTarifas = Convert.ToDecimal(registro.Substring(198, 15)) / 100;

                //Data Vencimento do Título
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(73, 8)).ToString("##-##-####"));

                //18.3T 97 99 3 - Num Banco Cobr./Receb.
                boleto.BancoCobradorRecebedor = registro.Substring(96, 3);
                //19.3T 100 104 5 - Num	Ag. Cobradora
                boleto.AgenciaCobradoraRecebedora = registro.Substring(99, 6);

                //Dados Sacado
                boleto.Sacado = new Sacado();
                string str = registro.Substring(133, 15);
                boleto.Sacado.CPFCNPJ = str.Substring(str.Length - 14, 14);
                boleto.Sacado.Nome = registro.Substring(148, 40);


                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 240 / T.", ex);
            }
        }
        public void LerDetalheRetornoCNAB240SegmentoU(ref Boleto boleto, string registro)
        {
            try
            {
                //Valor do Título
                boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(17, 15)) / 100;
                boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(32, 15)) / 100;
                boleto.ValorAbatimento = Convert.ToDecimal(registro.Substring(47, 15)) / 100;
                boleto.ValorIOF = Convert.ToDecimal(registro.Substring(62, 15)) / 100;
                boleto.ValorPago = Convert.ToDecimal(registro.Substring(77, 15)) / 100;
                boleto.ValorPagoCredito = Convert.ToDecimal(registro.Substring(92, 15)) / 100;
                boleto.ValorOutrasDespesas = Convert.ToDecimal(registro.Substring(107, 15)) / 100;
                boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(122, 15)) / 100;


                //Data Ocorrência no Banco
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(137, 8)).ToString("##-##-####"));

                // Data do Crédito
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(145, 8)).ToString("##-##-####"));

                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 240 / U.", ex);
            }
        }
        #endregion

        #region Retorno - CNAB400
        public void LerHeaderRetornoCNAB400(string registro)
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
                boleto.CodigoOcorrencia = registro.Substring(108, 2);
                boleto.DescricaoOcorrencia = OcorrenciasCnab400(boleto.CodigoOcorrencia);
                boleto.CodigoOcorrenciaAuxiliar = "";

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

        public string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            return numeroSequencial.ToString();
        }

        #endregion

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
    }
}
