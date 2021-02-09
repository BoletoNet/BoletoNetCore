using System;
using static System.String;

namespace BoletoNetCore
{
    partial class BancoItau : IBancoCNAB240
    {
        public string GerarHeaderRemessaCNAB240(ref int numeroArquivoRemessa, ref int numeroRegistro)
        {
            try
            {
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "341", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0000", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, Beneficiario.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 014, 0, Beneficiario.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0033, 013, 0, Beneficiario.Codigo, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0046, 007, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0053, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0054, 004, 0, Beneficiario.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0058, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0059, 007, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0066, 005, 0, Beneficiario.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0071, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0072, 001, 0, Beneficiario.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0073, 030, 0, Beneficiario.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0103, 030, 0, "BANCO ITAU", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0133, 010, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0144, 008, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediHoraHHMMSS___________, 0152, 006, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0158, 006, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0164, 003, 0, "040", '0');
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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "341", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "1", '0'); 
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 001, 0, "D", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0010, 002, 0, "05", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0012, 002, 0, "50", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 003, 0, "030", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0017, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, Beneficiario.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 014, 0, Beneficiario.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0033, 013, 0, Beneficiario.Codigo, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0046, 007, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0053, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0054, 004, 0, Beneficiario.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0058, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0059, 007, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0066, 005, 0, Beneficiario.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0071, 001, 0, Empty, ' ');
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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "341", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "5", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 006, 0, numeroRegistrosNoLote, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 018, 2, valorCobrancaSimples, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0042, 018, 2, valorCobrancaDescontada, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0060, 171, 0, Empty, ' ');
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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "341", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "9999", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "9", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 006, 0, qtLotes, '0'); //Qt Lotes
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 006, 0, numeroRegistrosNoArquivo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0030, 211, 0, Empty, ' ');
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
            registro++;
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "341", '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, registro, '0');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "A", '0');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 003, 0, boleto.CodigoMovimentoRetorno, ' ');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 003, 0, "000", '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0021, 003, 0, "341", '0');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0024, 001, 0, "0", '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 004, 0, boleto.AgenciaDebitada, '0');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0029, 001, 0, Empty, ' ');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0030, 007, 0, "0", '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0037, 005, 0, boleto.ContaDebitada, '0');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0042, 001, 0, Empty, ' ');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0043, 001, 0, boleto.DigitoVerificadorAgenciaContaDebitada, '0'); // Dac ????
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0044, 030, 0, boleto.Pagador.Nome, ' ');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0074, 015, 0, boleto.NumeroControleParticipante, ' ');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0089, 005, 0, Empty, ' ');
            reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0094, 008, 0, boleto.DataVencimento, ' ');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0102, 003, 0, boleto.TipoMoeda, ' ');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0105, 015, 2, boleto.ValorIOF, '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0120, 015, 2, boleto.ValorTitulo, '0');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0135, 020, 0, Empty, ' ');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0155, 008, 0, Empty, ' ');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0163, 015, 0, Empty, ' ');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0178, 002, 0, (int)boleto.TipoJuros, '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0180, 017, 2, boleto.ValorJurosDia, '0');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0197, 016, 0, boleto.ComplementoInstrucao1, ' ');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0213, 004, 0, Empty, ' ');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0217, 014, 0, boleto.Pagador.CPFCNPJ, '0');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0231, 010, 0, Empty, ' ');
            reg.CodificarLinha();
            var vLinha = reg.LinhaRegistro;
            return vLinha;

        }

        public override void LerHeaderRetornoCNAB240(ArquivoRetorno arquivoRetorno, string registro)
        {
            arquivoRetorno.Banco.Beneficiario = new Beneficiario();

            if (registro.Substring(17, 1) == "1")
                arquivoRetorno.Banco.Beneficiario.CPFCNPJ = registro.Substring(21, 11);
            else
                arquivoRetorno.Banco.Beneficiario.CPFCNPJ = registro.Substring(18, 14);

            arquivoRetorno.Banco.Beneficiario.Nome = registro.Substring(72, 30).Trim();

            arquivoRetorno.Banco.Beneficiario.ContaBancaria = new ContaBancaria();

            arquivoRetorno.Banco.Beneficiario.Codigo = registro.Substring(32, 13);
            arquivoRetorno.Banco.Beneficiario.ContaBancaria.Agencia = registro.Substring(52, 5);
            arquivoRetorno.Banco.Beneficiario.ContaBancaria.Conta = registro.Substring(58, 12);
            arquivoRetorno.Banco.Beneficiario.ContaBancaria.DigitoConta = registro.Substring(71, 1);

            arquivoRetorno.DataGeracao = Utils.ToDateTime(Utils.ToInt32(registro.Substring(143, 8)).ToString("##-##-####"));
            arquivoRetorno.NumeroSequencial = Utils.ToInt32(registro.Substring(157, 6));
        }

        public override void LerDetalheRetornoCNAB240SegmentoA(ref Boleto boleto, string registro)
        {
            try
            {
                boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaDebito;

                boleto.CodigoMovimentoRetorno = registro.Substring(14, 3);

                boleto.AgenciaDebitada = registro.Substring(24, 4);

                boleto.ContaDebitada = registro.Substring(36, 5);

                boleto.DigitoVerificadorAgenciaContaDebitada = registro.Substring(42, 1);

                boleto.NumeroDocumento = registro.Substring(73, 15).Trim();

                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(93, 8)).ToString("##-##-####"));
                
                boleto.ValorIOF = Convert.ToDecimal(registro.Substring(104, 15)) / 100;

                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(119, 15)) / 100;

                boleto.NossoNumero = registro.Substring(134, 20).Trim();

                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(154, 8)).ToString("##-##-####"));
                
                string _valorPagoCredito = registro.Substring(162, 15).Trim(); // ? 

                boleto.ValorPagoCredito = _valorPagoCredito == string.Empty ? 0 : Convert.ToDecimal(_valorPagoCredito);

                boleto.TipoJuros = (TipoJuros)Utils.ToInt32(registro.Substring(177, 2));

                boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(179, 17)) / 100; // ?

                boleto.CodigoMotivoOcorrencia = registro.Substring(230, 10).Trim();

                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 240 / A.", ex);
            }
        }
    }
}
