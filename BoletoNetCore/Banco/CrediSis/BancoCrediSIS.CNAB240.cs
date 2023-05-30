using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.String;

namespace BoletoNetCore
{
    partial class BancoCrediSIS : IBancoCNAB240
    {
        public string GerarHeaderRemessaCNAB240(ref int numeroArquivoRemessa, ref int numeroRegistro)
        {
            try
            {
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "097", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0000", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, Beneficiario.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 014, 0, Beneficiario.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0033, 020, 0, Beneficiario.ContaBancaria.CodigoConvenio, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0053, 005, 0, Beneficiario.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0058, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0059, 012, 0, Beneficiario.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0071, 001, 0, Beneficiario.ContaBancaria.DigitoConta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0072, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0073, 030, 0, Beneficiario.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0103, 030, 0, "097CENTRALCREDI", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0133, 010, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0144, 008, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediHoraHHMMSS___________, 0152, 006, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0158, 006, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0164, 003, 0, "101", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0167, 005, 0, Empty, ' ');
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

        public string GerarHeaderLoteRemessaCNAB240(ref int numeroArquivoRemessa, ref int numeroRegistro)
        {
            try
            {
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "097", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0000", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 001, 0, "R", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0010, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0012, 002, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0014, 003, 0, "060", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0017, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, Beneficiario.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 015, 0, Beneficiario.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0034, 020, 0, Beneficiario.ContaBancaria.CodigoConvenio, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0054, 005, 0, Beneficiario.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0059, 001, 0, Empty, ' '); /*Beneficiario.ContaBancaria.DigitoAgencia, ' ');*/
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0060, 012, 0, Beneficiario.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0072, 001, 0, Beneficiario.ContaBancaria.DigitoConta, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0073, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0074, 030, 0, Beneficiario.Nome, ' ');
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

        public string GerarTrailerLoteRemessaCNAB240(ref int numeroArquivoRemessa, int numeroRegistroGeral, int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples, int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada, int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada, int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                // O número de registros no lote é igual ao número de registros gerados + 2 (header e trailler do lote)
                int numeroRegistrosNoLote = numeroRegistroGeral + 2;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "097", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "5", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 006, 0, numeroRegistrosNoLote, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 006, 0, numeroRegistroCobrancaSimples, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0030, 017, 2, valorCobrancaSimples, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0047, 006, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0053, 017, 2, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0070, 006, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0076, 017, 2, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0093, 006, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0099, 017, 2, Empty, ' ');
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

        public string GerarTrailerRemessaCNAB240(int numeroRegistroGeral, decimal valorBoletoGeral, int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples, int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada, int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada, int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                // O número de registros no arquivo é igual ao número de registros gerados + 4 (header e trailler do lote / header e trailler do arquivo)
                int numeroRegistrosNoArquivo = numeroRegistroGeral + 4;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "097", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "9999", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "9", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 006, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 006, 0, numeroRegistrosNoArquivo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0030, 006, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0036, 205, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB400.", ex);
            }
        }

        public string GerarDetalheRemessaCNAB240(Boleto boleto, ref int numeroRegistro)
        {
            //string str1 = this.GerarDetalheSegmentoPRemessaCNAB240(boleto, ref numeroRegistro) + Environment.NewLine + this.GerarDetalheSegmentoQRemessaCNAB240(boleto, ref numeroRegistro);
            //string str2 = this.GerarDetalheSegmentoRRemessaCNAB240(boleto, ref numeroRegistro);
            //if (!IsNullOrWhiteSpace(str2))
            //    str1 = str1 + Environment.NewLine + str2;
            //string str3 = this.GerarDetalheSegmentoSRemessaCNAB240(boleto, ref numeroRegistro);
            //if (!IsNullOrWhiteSpace(str3))
            //    str1 = str1 + Environment.NewLine + str3;
            //return str1;
            // Segmento P (Obrigatório)
            var detalhe = GerarDetalheSegmentoPRemessaCNAB240(boleto, ref numeroRegistro);

            // Segmento Q (Obrigatório)
            detalhe += Environment.NewLine;
            detalhe += GerarDetalheSegmentoQRemessaCNAB240(boleto, ref numeroRegistro);

            // Segmento R (Opcional)
            var strline = GerarDetalheSegmentoRRemessaCNAB240(boleto, ref numeroRegistro);
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
            return detalhe;
        }

        private string GerarDetalheSegmentoPRemessaCNAB240(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "097", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0000", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, numeroRegistroGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "P", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 002, 0, boleto.CodigoMovimentoRetorno, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 005, 0, boleto.Banco.Beneficiario.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0023, 001, 0, Empty, ' ')/*boleto.Banco.Beneficiario.ContaBancaria.DigitoAgencia, ' ')*/;
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 012, 0, boleto.Banco.Beneficiario.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0036, 001, 0, boleto.Banco.Beneficiario.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0037, 001, 0, Empty, ' ');

                //if (boleto.Banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto == TipoImpressaoBoleto.Banco)
                //{
                //    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0038, 009, 0, "0", '0');
                //    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0047, 001, 0, "0", '0');
                //}
                //else
                //{
                //    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0038, 009, 0, boleto.NossoNumero, '0');
                //    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0047, 001, 0, boleto.NossoNumeroDV, '0');
                //}
                //reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0048, 002, 0, "01", '0');
                //reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0050, 002, 0, boleto.VariacaoCarteira, '0');
                //reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0052, 001, 0, "4", '0');
                //reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0053, 005, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0038, 20, 0, boleto.NossoNumeroFormatado, '0');

                //// NOSSO NUMERO
                //reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0038, 003, 0, "097", '0');
                //reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0041, 001, 0, Utils.FormatCode(boleto.Banco.Beneficiario.CPFCNPJ, 1), '0');
                //reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0042, 004, 0, Utils.FormatCode(boleto.Banco.Beneficiario.ContaBancaria.Agencia, 4), '0');
                //reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0046, 006, 0, Utils.FormatCode(boleto.Banco.Beneficiario.ContaBancaria.CodigoConvenio, 6), '0');
                //reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0052, 006, 0, Utils.FormatCode(boleto.NossoNumero, 6), '0');
                //reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0047, 001, 0, boleto.NossoNumeroDV, '0');


                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0058, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0059, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0060, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0061, 001, 0, "2", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0062, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0063, 015, 0, boleto.NumeroDocumento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0078, 008, 0, boleto.DataVencimento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0086, 015, 2, boleto.ValorTitulo, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0101, 005, 0, boleto.Banco.Beneficiario.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0106, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0107, 002, 0, (int)boleto.EspecieDocumento, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 001, 0, boleto.Aceite, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0110, 008, 0, boleto.DataEmissao, '0');

                if (boleto.ValorJurosDia == 0 && boleto.PercentualJurosDia == 0)
                {
                    // Sem Juros Mora
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0118, 001, 0, "3", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0119, 008, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 015, 2, 0, '0');
                }
                else
                {
                    // Com Juros Mora ($)
                    var codigoMora = (boleto.ValorJurosDia != 0) ? "1" : ((boleto.PercentualJurosDia != 0) ? "2" : "0");
                    var valor = (boleto.ValorJurosDia != 0) ? boleto.ValorJurosDia : ((boleto.PercentualJurosDia != 0) ? boleto.PercentualJurosDia : 0);

                    // Banco CrediSIS pega taxa mensal, e não diaria
                    valor = valor * 30;

                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0118, 001, 0, codigoMora, '0');
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0119, 008, 0, boleto.DataJuros, '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 015, 2, valor, '0');
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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0166, 015, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0181, 015, 0, Empty, ' ');
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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0224, 001, 0, Empty, ' ');
                #endregion
                //reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0224, 001, 0, Empty, ' ');

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0225, 003, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0228, 002, 0, "09", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0230, 010, 2, Empty, ' ');
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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "097", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0000", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, numeroRegistroGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "Q", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 002, 0, boleto.CodigoMovimentoRetorno, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, boleto.Pagador.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 015, 0, boleto.Pagador.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0034, 040, 0, boleto.Pagador.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0074, 040, 0, boleto.Pagador.Endereco.FormataLogradouro(40), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0114, 015, 0, boleto.Pagador.Endereco.Bairro, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0129, 008, 0, boleto.Pagador.Endereco.CEP.Replace("-", ""), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0137, 015, 0, boleto.Pagador.Endereco.Cidade, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0152, 002, 0, boleto.Pagador.Endereco.UF, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0154, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0155, 015, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0170, 040, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0210, 003, 0, "097", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0213, 020, 0, boleto.Banco.Beneficiario.ContaBancaria.NossoNumeroBancoCorrespondente, ' ');
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
                else if (boleto.PercentualMulta > 0)
                    codMulta = "2";

                if (codMulta == "0")
                {
                    // Se não tiver informação sobre Multa, não precisa gerar o registro.
                    return "";
                }

                var valorMulta = (codMulta == "1") ? boleto.ValorMulta : ((codMulta == "2") ? boleto.PercentualMulta : 0);

                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "097", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, numeroRegistroGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "R", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 002, 0, boleto.CodigoMovimentoRetorno, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 008, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 015, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0042, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0043, 008, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0051, 015, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0066, 001, 0, codMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0067, 008, 0, boleto.DataMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0075, 015, 2, valorMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0090, 010, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0100, 040, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 040, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0180, 020, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0200, 008, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0208, 003, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0211, 005, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0216, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0217, 012, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0229, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0230, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0231, 001, 0, Empty, ' ');
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
                var msg5A9 = boleto.MensagemArquivoRemessa.PadRight(500, ' ').Substring(0, 140).FitStringLength(140, ' ');
                if (IsNullOrWhiteSpace(msg5A9))
                    return "";

                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "097", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0000", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, numeroRegistroGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "S", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, "1", '0');


                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 002, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0021, 140, 0, msg5A9, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0161, 002, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0163, 078,  0, Empty, ' ');


                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do Segmento S no arquivo de remessa do CNAB240.", ex);
            }
        }

        public override void LerHeaderRetornoCNAB240(ArquivoRetorno arquivoRetorno, string registro)
        {
            arquivoRetorno.Banco.Beneficiario = new Beneficiario();
            arquivoRetorno.Banco.Beneficiario.CPFCNPJ = registro.Substring(17, 1) == "1" ? registro.Substring(21, 11) : registro.Substring(18, 14);
            arquivoRetorno.Banco.Beneficiario.Nome = registro.Substring(72, 30).Trim();
            arquivoRetorno.Banco.Beneficiario.ContaBancaria = new ContaBancaria();
            arquivoRetorno.Banco.Beneficiario.ContaBancaria.Agencia = registro.Substring(52, 5);
            arquivoRetorno.Banco.Beneficiario.ContaBancaria.DigitoAgencia = registro.Substring(57, 1);
            arquivoRetorno.Banco.Beneficiario.ContaBancaria.Conta = registro.Substring(58, 12);
            arquivoRetorno.Banco.Beneficiario.ContaBancaria.DigitoConta = registro.Substring(70, 1);
            arquivoRetorno.DataGeracao = new DateTime?(Utils.ToDateTime(Utils.ToInt32(registro.Substring(143, 8)).ToString("##-##-####")));
            arquivoRetorno.NumeroSequencial = new int?(Utils.ToInt32(registro.Substring(157, 6)));
        }

        public override void LerDetalheRetornoCNAB240SegmentoT(ref Boleto boleto, string registro)
        {
            try
            {
                boleto.NumeroControleParticipante = registro.Substring(105, 25);
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
                boleto.NossoNumero = registro.Substring(37, 18);
                boleto.NossoNumeroDV = registro.Substring(54, 1);
                boleto.NossoNumeroFormatado = boleto.NossoNumero + "-" + boleto.NossoNumeroDV;
                boleto.CodigoMovimentoRetorno = registro.Substring(15, 2);
                boleto.DescricaoMovimentoRetorno = Cnab.MovimentoRetornoCnab240(boleto.CodigoMovimentoRetorno);
                boleto.CodigoMotivoOcorrencia = registro.Substring(213, 10);
                boleto.ListMotivosOcorrencia = Cnab.MotivoOcorrenciaCnab240(boleto.CodigoMotivoOcorrencia, boleto.CodigoMovimentoRetorno);
                boleto.NumeroDocumento = registro.Substring(58, 15);
                boleto.EspecieDocumento = TipoEspecieDocumento.NaoDefinido;
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(81, 15)) / 100M;
                boleto.ValorTarifas = Convert.ToDecimal(registro.Substring(198, 15)) / 100M;
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(73, 8)).ToString("##-##-####"));
                boleto.BancoCobradorRecebedor = registro.Substring(96, 3);
                boleto.AgenciaCobradoraRecebedora = registro.Substring(99, 6);
                boleto.Pagador = new Pagador();
                string str = registro.Substring(133, 15);
                boleto.Pagador.CPFCNPJ = str.Substring(str.Length - 14, 14);
                boleto.Pagador.Nome = registro.Substring(148, 40);
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 240 / T.", ex);
            }
        }

        public override void LerDetalheRetornoCNAB240SegmentoU(ref Boleto boleto, string registro)
        {
            try
            {
                boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(17, 15)) / 100M;
                boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(32, 15)) / 100M;
                boleto.ValorAbatimento = Convert.ToDecimal(registro.Substring(47, 15)) / 100M;
                boleto.ValorIOF = Convert.ToDecimal(registro.Substring(62, 15)) / 100M;
                boleto.ValorPago = Convert.ToDecimal(registro.Substring(77, 15)) / 100M;
                boleto.ValorPagoCredito = Convert.ToDecimal(registro.Substring(92, 15)) / 100M;
                boleto.ValorOutrasDespesas = Convert.ToDecimal(registro.Substring(107, 15)) / 100M;
                boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(122, 15)) / 100M;
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(137, 8)).ToString("##-##-####"));
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(145, 8)).ToString("##-##-####"));
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 240 / U.", ex);
            }
        }

    }
}
