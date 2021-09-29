using System;
using static System.String;

namespace BoletoNetCore
{
    partial class BancoNordeste : IBancoCNAB240
    {
        public string GerarHeaderRemessaCNAB240(ref int numeroArquivoRemessa, ref int numeroRegistro)
        {
            try
            {
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "004", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0000", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, Beneficiario.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 014, 0, Beneficiario.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0033, 020, 0, Beneficiario.Codigo, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0053, 005, 0, Beneficiario.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0058, 001, 0, Beneficiario.ContaBancaria.DigitoAgencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0059, 012, 0, Beneficiario.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0071, 001, 0, Beneficiario.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0072, 001, 0, Beneficiario.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0073, 030, 0, Beneficiario.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0103, 030, 0, "BANCO NORDESTE", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0133, 010, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0144, 008, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediHoraHHMMSS___________, 0152, 006, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0158, 006, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0164, 003, 0, "091", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0167, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0172, 020, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0192, 049, 0, Empty, ' ');
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
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "004", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 001, 0, "D", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0010, 002, 0, "05", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0012, 002, 0, "50", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 003, 0, "030", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0017, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, Beneficiario.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 014, 0, Beneficiario.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0033, 020, 0, Beneficiario.Codigo, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0053, 005, 0, Beneficiario.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0058, 001, 0, Beneficiario.ContaBancaria.DigitoAgencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0059, 012, 0, Beneficiario.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0071, 001, 0, Beneficiario.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0072, 001, 0, Beneficiario.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0073, 030, 0, Beneficiario.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0103, 040, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0143, 030, 0, Beneficiario.Endereco.LogradouroEndereco, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0173, 005, 0, Beneficiario.Endereco.LogradouroNumero, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0178, 015, 0, Beneficiario.Endereco.LogradouroComplemento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0193, 020, 0, Beneficiario.Endereco.Cidade, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0213, 008, 0, Beneficiario.Endereco.CEP, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0221, 002, 0, Beneficiario.Endereco.UF, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0223, 008, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0231, 010, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do lote no arquivo de remessa do CNAB240.", ex);
            }
        }

        public string GerarTrailerLoteRemessaCNAB240(ref int numeroArquivoRemessa, int numeroRegistroGeral, int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples, int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada, int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada, int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                // O número de registros no lote é igual ao número de registros gerados + 2 (header e trailler do lote)
                var numeroRegistrosNoLote = numeroRegistroGeral + 2;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "004", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "5", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 006, 0, numeroRegistrosNoLote, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 018, 2, valorCobrancaSimples, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0042, 018, 5, valorCobrancaDescontada, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0060, 006, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0066, 165, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0231, 010, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar TRAILER do lote no arquivo de remessa do CNAB240.", ex);
            }
        }

        public string GerarTrailerRemessaCNAB240(int numeroRegistroGeral, decimal valorBoletoGeral, int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples, int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada, int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada, int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                // O número de registros no arquivo é igual ao número de registros gerados + 4 (header e trailler do lote / header e trailler do arquivo)
                var numeroRegistrosNoArquivo = numeroRegistroGeral + 4;
                var qtLotes = 1;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "004", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "9999", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "9", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 006, 0, qtLotes, '0'); //Qt Lotes
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 006, 0, numeroRegistrosNoArquivo, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0030, 006, 0, "0", '0'); //Qt Lotes
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0036, 205, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar TRAILER do arquivo de remessa do CNAB240.", ex);
            }
        }

        public string GerarDetalheRemessaCNAB240(Boleto boleto, ref int numeroRegistro)
        {
            string detalhe = Empty, strline = Empty;
            // Segmento A
            detalhe += GerarDetalheSegmentoARemessaCNAB240(boleto, ref numeroRegistro);
            return detalhe;
        }

        private string GerarDetalheSegmentoARemessaCNAB240(Boleto boleto, ref int registro)
        {
            var reg = new TRegistroEDI();
            var codigoMovimentoRetorno = int.Parse(boleto.CodigoMovimentoRetorno).ToString();
            registro++;
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "004", '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, registro, '0');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "A", '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0015, 001, 0, codigoMovimentoRetorno, '0'); ;
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 002, 0, "00", '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 003, 0, Empty, ' ');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0021, 003, 0, "004", '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 005, 0, boleto.AgenciaDebitada, '0');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0029, 001, 0, boleto.DigitoVerificadorAgenciaDebitada, '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0030, 012, 0, boleto.ContaDebitada, '0');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0042, 001, 0, boleto.DigitoVerificadorAgenciaContaDebitada, '0');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0043, 001, 0, boleto.DigitoVerificadorAgenciaContaDebitada, '0'); // Dac ????
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0044, 030, 0, boleto.Pagador.Nome, ' ');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0074, 020, 0, boleto.NumeroControleParticipante, ' ');
            reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0094, 008, 0, boleto.DataVencimento, ' ');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0102, 003, 0, boleto.TipoMoeda, ' ');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0105, 015, 5, boleto.ValorIOF, '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0120, 015, 2, boleto.ValorTitulo, '0');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0135, 020, 0, Empty, ' ');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0155, 008, 0, "0", '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0163, 015, 0, "0", '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0178, 040, 0, Empty, ' ');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0218, 002, 0, Empty, ' ');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0220, 010, 0, Empty, ' ');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0230, 001, 0, "0", '0');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0231, 010, 0, Empty, ' ');
            reg.CodificarLinha();
            var vLinha = reg.LinhaRegistro;
            return vLinha;

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
