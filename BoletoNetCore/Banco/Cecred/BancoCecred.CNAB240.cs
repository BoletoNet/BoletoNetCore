using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.String;

namespace BoletoNetCore
{
    partial class BancoCecred : IBancoCNAB240
    {
        public string GerarHeaderRemessaCNAB240(ref int numeroArquivoRemessa, ref int numeroRegistro)
        {
            try
            {
                TRegistroEDI tregistroEdi = new TRegistroEDI();
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 1, 3, 0, "085", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 4, 4, 0, "0000", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 8, 1, 0, "0", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 9, 9, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 18, 1, 0, Beneficiario.TipoCPFCNPJ("0"), '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 19, 14, 0, Beneficiario.CPFCNPJ, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 33, 20, 0, Beneficiario.Codigo, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 53, 5, 0, Beneficiario.ContaBancaria.Agencia, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 58, 1, 0, Beneficiario.ContaBancaria.DigitoAgencia, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 59, 12, 0, Beneficiario.ContaBancaria.Conta, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 71, 1, 0, Beneficiario.ContaBancaria.DigitoConta, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 72, 1, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 73, 30, 0, Beneficiario.Nome, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 103, 30, 0, "VIACREDI", ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 133, 10, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 143, 1, 0, "1", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 144, 8, 0, DateTime.Now, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediHoraHHMMSS___________, 152, 6, 0, DateTime.Now, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 158, 6, 0, numeroArquivoRemessa, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 164, 3, 0, "087", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 167, 5, 0, "0", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 172, 20, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 192, 20, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 212, 29, 0, Empty, ' ');
                tregistroEdi.CodificarLinha();
                return tregistroEdi.LinhaRegistro;
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
                TRegistroEDI tregistroEdi = new TRegistroEDI();
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 1, 3, 0, "085", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 4, 4, 0, "0001", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 8, 1, 0, "1", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 9, 1, 0, "R", ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 10, 2, 0, "01", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 12, 2, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 14, 3, 0, "045", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 17, 1, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 18, 1, 0, Beneficiario.TipoCPFCNPJ("0"), '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 19, 15, 0, Beneficiario.CPFCNPJ, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 34, 20, 0, Beneficiario.ContaBancaria.CodigoConvenio, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 54, 5, 0, Beneficiario.ContaBancaria.Agencia, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 59, 1, 0, Beneficiario.ContaBancaria.DigitoAgencia, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 60, 12, 0, Beneficiario.ContaBancaria.Conta, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 72, 1, 0, Beneficiario.ContaBancaria.DigitoConta, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 73, 1, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 74, 30, 0, Beneficiario.Nome, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 104, 40, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 144, 40, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 184, 8, 0, numeroArquivoRemessa, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 192, 8, 0, DateTime.Now, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 200, 8, 0, "0", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 208, 33, 0, Empty, ' ');
                tregistroEdi.CodificarLinha();
                return tregistroEdi.LinhaRegistro;
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
                int num = numeroRegistroGeral + 2;
                TRegistroEDI tregistroEdi = new TRegistroEDI();
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 1, 3, 0, "085", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 4, 4, 0, "0001", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 8, 1, 0, "5", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 9, 9, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 18, 6, 0, num, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 24, 6, 0, numeroRegistroCobrancaSimples, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 30, 17, 2, valorCobrancaSimples, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 47, 6, 0, numeroRegistroCobrancaVinculada, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 53, 17, 2, valorCobrancaVinculada, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 70, 6, 0, numeroRegistroCobrancaCaucionada, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 76, 17, 2, valorCobrancaCaucionada, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 93, 6, 0, numeroRegistroCobrancaDescontada, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 99, 17, 2, valorCobrancaDescontada, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 116, 8, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 124, 117, 0, Empty, ' ');
                tregistroEdi.CodificarLinha();
                return tregistroEdi.LinhaRegistro;
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
                int num = numeroRegistroGeral + 4;
                TRegistroEDI tregistroEdi = new TRegistroEDI();
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 1, 3, 0, "085", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 4, 4, 0, "9999", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 8, 1, 0, "9", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 9, 9, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 18, 6, 0, "1", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 24, 6, 0, num, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 30, 6, 0, "0", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 36, 205, 0, Empty, ' ');
                tregistroEdi.CodificarLinha();
                return tregistroEdi.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB400.", ex);
            }
        }

        public string GerarDetalheRemessaCNAB240(Boleto boleto, ref int numeroRegistro)
        {
            string str1 = this.GerarDetalheSegmentoPRemessaCNAB240(boleto, ref numeroRegistro) + Environment.NewLine + this.GerarDetalheSegmentoQRemessaCNAB240(boleto, ref numeroRegistro);
            string str2 = this.GerarDetalheSegmentoRRemessaCNAB240(boleto, ref numeroRegistro);
            if (!IsNullOrWhiteSpace(str2))
                str1 = str1 + Environment.NewLine + str2;
            string str3 = this.GerarDetalheSegmentoSRemessaCNAB240(boleto, ref numeroRegistro);
            if (!IsNullOrWhiteSpace(str3))
                str1 = str1 + Environment.NewLine + str3;
            return str1;
        }

        private string GerarDetalheSegmentoPRemessaCNAB240(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                ++numeroRegistroGeral;
                TRegistroEDI tregistroEdi = new TRegistroEDI();
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 1, 3, 0, "085", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 4, 4, 0, "1", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 8, 1, 0, "3", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 9, 5, 0, numeroRegistroGeral, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 14, 1, 0, "P", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 15, 1, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 16, 2, 0, boleto.CodigoMovimentoRetorno, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 18, 5, 0, boleto.Banco.Beneficiario.ContaBancaria.Agencia, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 23, 1, 0, boleto.Banco.Beneficiario.ContaBancaria.DigitoAgencia, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 24, 12, 0, boleto.Banco.Beneficiario.ContaBancaria.Conta, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 36, 1, 0, boleto.Banco.Beneficiario.ContaBancaria.DigitoConta, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 37, 1, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 37, 17, 0, Utils.FormatCode(boleto.NossoNumero, 17), '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 55, 3, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 58, 1, 0, boleto.Carteira, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 59, 1, 0, "1", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 60, 1, 0, "1", ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 61, 1, 0, (int)boleto.Banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 62, 1, 0, boleto.Distribuicao, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 63, 15, 0, boleto.NumeroDocumento, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 78, 8, 0, boleto.DataVencimento, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 86, 15, 2, boleto.ValorTitulo, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 101, 5, 0, "0", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 106, 1, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 107, 2, 0, (int)boleto.EspecieDocumento, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 109, 1, 0, boleto.Aceite, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 110, 8, 0, boleto.DataEmissao, '0');
                if (boleto.ValorJurosDia == 0M)
                {
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 118, 1, 0, "3", '0');
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 119, 8, 0, "0", '0');
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, sbyte.MaxValue, 15, 2, 0, '0');
                }
                else
                {
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 118, 1, 0, "1", '0');
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 119, 8, 0, boleto.DataJuros, '0');
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, sbyte.MaxValue, 15, 2, boleto.ValorJurosDia, '0');
                }
                if (boleto.ValorDesconto == 0M)
                {
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 142, 1, 0, "0", '0');
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 143, 8, 0, "0", '0');
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 151, 15, 2, "0", '0');
                }
                else
                {
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 142, 1, 0, "1", '0');
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 143, 8, 0, boleto.DataDesconto, '0');
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 151, 15, 2, boleto.ValorDesconto, '0');
                }
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 166, 15, 2, boleto.ValorIOF, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 181, 15, 2, boleto.ValorAbatimento, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 196, 25, 0, boleto.NumeroControleParticipante, ' ');
                switch (boleto.CodigoProtesto)
                {
                    case TipoCodigoProtesto.NaoProtestar:
                        tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 221, 1, 0, 3, '0');
                        break;
                    case TipoCodigoProtesto.ProtestarDiasCorridos:
                        tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 221, 1, 0, 1, '0');
                        break;
                    case TipoCodigoProtesto.ProtestarDiasUteis:
                        tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 221, 1, 0, 2, '0');
                        break;
                    default:
                        tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 221, 1, 0, 0, '0');
                        break;
                }
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 222, 2, 0, boleto.DiasProtesto, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 224, 1, 0, "2", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 225, 3, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 228, 2, 0, "09", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 230, 10, 2, "0", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 240, 1, 0, Empty, ' ');
                tregistroEdi.CodificarLinha();
                return tregistroEdi.LinhaRegistro;
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
                ++numeroRegistroGeral;
                TRegistroEDI tregistroEdi = new TRegistroEDI();
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 1, 3, 0, "085", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 4, 4, 0, "0001", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 8, 1, 0, "3", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 9, 5, 0, numeroRegistroGeral, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 14, 1, 0, "Q", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 15, 1, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 16, 2, 0, boleto.CodigoMovimentoRetorno, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 18, 1, 0, boleto.Pagador.TipoCPFCNPJ("0"), '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 19, 15, 0, boleto.Pagador.CPFCNPJ, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 34, 40, 0, boleto.Pagador.Nome, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 74, 40, 0, boleto.Pagador.Endereco.FormataLogradouro(40), ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 114, 15, 0, boleto.Pagador.Endereco.Bairro, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 129, 8, 0, boleto.Pagador.Endereco.CEP.Replace("-", ""), ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 137, 15, 0, boleto.Pagador.Endereco.Cidade, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 152, 2, 0, boleto.Pagador.Endereco.UF, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 154, 1, 0, boleto.Avalista.TipoCPFCNPJ("0"), '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 155, 15, 0, boleto.Avalista.CPFCNPJ, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 170, 40, 0, boleto.Avalista.Nome, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 210, 3, 0, boleto.Banco.Beneficiario.ContaBancaria.CodigoBancoCorrespondente, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 213, 20, 0, boleto.Banco.Beneficiario.ContaBancaria.NossoNumeroBancoCorrespondente, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 233, 8, 0, Empty, ' ');
                tregistroEdi.CodificarLinha();
                return tregistroEdi.LinhaRegistro;
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
                string str = "0";
                if (boleto.ValorMulta > 0M)
                    str = "1";
                if (boleto.PercentualMulta > 0M)
                    str = "2";
                if (str == "0" && boleto.ValorDesconto2 == 0 && boleto.ValorDesconto3 == 0)
                    return "";
                ++numeroRegistroGeral;
                TRegistroEDI tregistroEdi = new TRegistroEDI();
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 1, 3, 0, "085", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 4, 4, 0, "0001", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 8, 1, 0, "3", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 9, 5, 0, numeroRegistroGeral, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 14, 1, 0, "R", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 15, 1, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 16, 2, 0, boleto.CodigoMovimentoRetorno, '0');
                if (boleto.ValorDesconto2 == 0)
                {
                    // Sem Desconto 2
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 18, 1, 0, "0", '0');
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 19, 8, 0, "0", '0');
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 27, 15, 0, "0", '0');
                }
                else
                {
                    // Com Desconto 2
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 18, 1, 0, "1", '0');
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 19, 8, 0, boleto.DataDesconto2, '0');
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 27, 15, 2, boleto.ValorDesconto2, '0');
                }
                if (boleto.ValorDesconto3 == 0)
                {
                    // Sem Desconto 3
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 42, 1, 0, "0", '0');
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 43, 8, 0, "0", '0');
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 51, 15, 0, "0", '0');
                }
                else
                {
                    // Com Desconto 3
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 42, 1, 0, "1", '0');
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 43, 8, 0, boleto.DataDesconto3, '0');
                    tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 51, 15, 2, boleto.ValorDesconto3, '0');
                }
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 66, 1, 0, str, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 67, 8, 0, boleto.DataMulta, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 75, 15, 2, boleto.ValorMulta, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 90, 10, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 100, 40, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 140, 40, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 180, 20, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 200, 8, 0, "0", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 208, 3, 0, "0", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 211, 5, 0, "0", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 216, 1, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 217, 12, 0, "0", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 229, 1, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 230, 1, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 231, 1, 0, "0", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 232, 9, 0, Empty, ' ');
                tregistroEdi.CodificarLinha();
                return tregistroEdi.LinhaRegistro;
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
                string str = boleto.MensagemArquivoRemessa.PadRight(500, ' ').Substring(0, 200).FitStringLength(200, ' ');
                if (string.IsNullOrWhiteSpace(str))
                    return "";
                ++numeroRegistroGeral;
                TRegistroEDI tregistroEdi = new TRegistroEDI();
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 1, 3, 0, "085", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 4, 4, 0, "0001", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 8, 1, 0, "3", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 9, 5, 0, numeroRegistroGeral, '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 14, 1, 0, "S", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 15, 1, 0, Empty, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 16, 2, 0, "01", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 18, 1, 0, "3", '0');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 19, 200, 0, str, ' ');
                tregistroEdi.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 219, 22, 0, Empty, ' ');
                tregistroEdi.CodificarLinha();
                return tregistroEdi.LinhaRegistro;
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
