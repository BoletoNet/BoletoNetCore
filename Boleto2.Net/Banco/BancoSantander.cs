using System;
using System.Collections.Generic;
using Boleto2Net.Exceptions;
using static System.String;

namespace Boleto2Net
{
    internal sealed class BancoSantander : IBanco
    {
        internal static Lazy<IBanco> Instance { get; } = new Lazy<IBanco>(() => new BancoSantander());

        public Cedente Cedente { get; set; }
        public int Codigo { get; } = 033;
        public string Nome { get; } = "Santander";
        public string Digito { get; } = "7";
        public List<string> IdsRetornoCnab400RegistroDetalhe { get; } = new List<string> { };
        public bool RemoveAcentosArquivoRemessa { get; } = true;

        public void FormataCedente()
        {
            var contaBancaria = Cedente.ContaBancaria;

            if (!CarteiraFactory<BancoSantander>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw Boleto2NetException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados("PAGÁVEL PREFERENCIALMENTE NO BANCO SANTANDER.", "", "", digitosConta: 9);

            var codigoCedente = Cedente.Codigo;
            Cedente.Codigo = codigoCedente.Length <= 7 ? codigoCedente.PadLeft(7, '0') : throw Boleto2NetException.CodigoCedenteInvalido(codigoCedente, 7);

            Cedente.CodigoFormatado = $"{contaBancaria.Agencia} / {Cedente.Codigo}";
        }

        public void ValidaBoleto(Boleto boleto)
        {
        }

        public void FormataNossoNumero(Boleto boleto)
        {
            var carteira = CarteiraFactory<BancoSantander>.ObterCarteira(boleto.CarteiraComVariacao);
            carteira.FormataNossoNumero(boleto);
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var carteira = CarteiraFactory<BancoSantander>.ObterCarteira(boleto.CarteiraComVariacao);
            return carteira.FormataCodigoBarraCampoLivre(boleto);
        }

        public string GerarHeaderRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                var header = Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        // Cabeçalho do Arquivo
                        header += GerarHeaderRemessaCNAB240(numeroArquivoRemessa, ref numeroRegistroGeral);
                        // Cabeçalho do Lote
                        header += Environment.NewLine;
                        header += GerarHeaderLoteRemessaCNAB240(numeroArquivoRemessa, ref numeroRegistroGeral);
                        break;
                    default:
                        throw new Exception("Santander - Header - Tipo de arquivo inexistente.");
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
                var detalhe = Empty;
                var strline = "";
                //base.GerarDetalheRemessa(boleto, numeroRegistro, tipoArquivo);
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
                    default:
                        throw new Exception("Santander - Detalhe - Tipo de arquivo inexistente.");
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
                var trailler = Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        // Trailler do Lote
                        trailler += GerarTrailerLoteRemessaCNAC240(ref numeroRegistroGeral,
                            numeroRegistroCobrancaSimples, valorCobrancaSimples,
                            numeroRegistroCobrancaCaucionada, valorCobrancaCaucionada,
                            numeroRegistroCobrancaDescontada, valorCobrancaDescontada);
                        // Trailler do Arquivo
                        trailler += Environment.NewLine;
                        trailler += GerarTrailerRemessaCNAB240(ref numeroRegistroGeral);
                        break;
                    default:
                        throw new Exception("Santander - Trailler - Tipo de arquivo inexistente.");
                }
                return trailler;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do Trailler do arquivo de REMESSA.", ex);
            }
        }

        public void LerHeaderRetornoCNAB240(ArquivoRetorno arquivoRetorno, string registro)
        {

            //LAYOUT V 2.8 Fevereiro/2017


            arquivoRetorno.Banco.Cedente = new Cedente();
            //017 - 017 Tipo de inscrição da empresa N 001 1 = CPF, 2 = CNPJ
            //018 - 032 Nº de inscrição da empresa N 015
            arquivoRetorno.Banco.Cedente.CPFCNPJ = registro.Substring(16, 1) == "1" ? registro.Substring(21, 11) : registro.Substring(18, 14);
            //053 - 061 Código do Beneficiário N 009
            arquivoRetorno.Banco.Cedente.Codigo = registro.Substring(54, 7);
            //073 - 102 Nome da empresa A 030
            arquivoRetorno.Banco.Cedente.Nome = registro.Substring(72, 30).Trim();


            ////103 - 132 Nome do Banco A 030
            //arquivoRetorno.Banco.Nome = registro.Substring(102, 30);

            arquivoRetorno.Banco.Cedente.ContaBancaria = new ContaBancaria();
            //033 - 036 Agência do Beneficiário N 004 3 
            arquivoRetorno.Banco.Cedente.ContaBancaria.Agencia = registro.Substring(32, 4);
            //037 - 037 Dígito da Agência do Beneficiário N 001 3
            arquivoRetorno.Banco.Cedente.ContaBancaria.DigitoAgencia = registro.Substring(36, 1);
            //038 - 046 Número da conta corrente N 009 3
            arquivoRetorno.Banco.Cedente.ContaBancaria.Conta = registro.Substring(37, 9);
            //047 - 047 Dígito verificador da conta N 001 3
            arquivoRetorno.Banco.Cedente.ContaBancaria.DigitoConta = registro.Substring(46, 1);


            //144 - 151 Data de geração do arquivo N 008 DDMMAAAA
            arquivoRetorno.DataGeracao = Utils.ToDateTime(Utils.ToInt32(registro.Substring(143, 8)).ToString("##-##-####"));
            //158 - 163 Nº seqüencial do arquivo N 006
            arquivoRetorno.NumeroSequencial = Utils.ToInt32(registro.Substring(157, 6));
        }

        public void LerDetalheRetornoCNAB240SegmentoT(ref Boleto boleto, string registro)
        {
            try
            {
                //Nº Controle do Participante
                boleto.NumeroControleParticipante = registro.Substring(100, 25);

                //Carteira
                boleto.Carteira = registro.Substring(53, 1);
                switch (boleto.Carteira)
                {
                    case "3":
                    case "6":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaCaucionada;
                        break;
                    case "4":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaDescontada;
                        break;
                    default:
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;
                        break;
                }

                //Identificação do Título no Banco
                boleto.NossoNumero = registro.Substring(40, 12);
                boleto.NossoNumeroDV = registro.Substring(52, 1);
                boleto.NossoNumeroFormatado = Format("{0}-{1}", boleto.NossoNumero, boleto.NossoNumeroDV);

                //Identificação de Ocorrência
                boleto.CodigoOcorrencia = registro.Substring(15, 2);
                boleto.DescricaoOcorrencia = Cnab.OcorrenciaCnab240(boleto.CodigoOcorrencia);
                boleto.CodigoOcorrenciaAuxiliar = registro.Substring(208, 10);

                //Número do Documento
                boleto.NumeroDocumento = registro.Substring(54, 15);
                boleto.EspecieDocumento = TipoEspecieDocumento.NaoDefinido;

                //Valor do Título
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(77, 15)) / 100;

                //Data Vencimento do Título
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(69, 8)).ToString("##-##-####"));

                //093 – 095 Nº do Banco Cobrador / Recebedor N 003 - LAYOUT V 2.8 Fevereiro/2017 Pág 9
                boleto.BancoCobradorRecebedor = registro.Substring(92, 3);

                //096 – 099 Agência Cobradora / Recebedora N 004 - LAYOUT V 2.8 Fevereiro/2017 Pág 9
                //100 – 100 Dígito da Agência do Beneficiário N 001 - LAYOUT V 2.8 Fevereiro/2017 Pág 9
                boleto.AgenciaCobradoraRecebedora = registro.Substring(95, 5);

                //129 – 143 Número de inscrição Pagador N 015 30 - LAYOUT V 2.8 Fevereiro/2017 Pág 9
                //aqui, apesar de haver 15 caracteres no layout, pegamos apenas os últimos 14(o necessário) pois há uma validação no momento da atribuição(set) do CPFCNPJ
                boleto.Sacado = new Sacado();
                boleto.Sacado.CPFCNPJ = registro.Substring(129, 14);
                
                //144 - 183 Nome do Pagador A 040 - LAYOUT V 2.8 Fevereiro/2017 Pág 9
                boleto.Sacado.Nome = registro.Substring(143, 40);

                //194 – 208 Valor da Tarifa / Custas N 015 2 - LAYOUT V 2.8 Fevereiro/2017 Pág 9
                boleto.ValorTarifas = Convert.ToDecimal(registro.Substring(193, 15)) / 100;

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

        public void LerHeaderRetornoCNAB400(string registro)
        {
            throw new NotImplementedException();
        }

        public void LerDetalheRetornoCNAB400Segmento1(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException();
        }

        public void LerDetalheRetornoCNAB400Segmento7(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException();
        }

        public void LerTrailerRetornoCNAB400(string registro)
        {
            throw new NotImplementedException();
        }

        public string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            return "";
        }

        private string GerarHeaderRemessaCNAB240(int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "033", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0000", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 008, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0017, 001, 0, Cedente.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 015, 0, Cedente.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0033, 015, 0, Cedente.CodigoTransmissao, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0048, 025, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0073, 030, 0, Cedente.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0103, 030, 0, "BANCO SANTANDER", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0133, 010, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0144, 008, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0152, 006, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0158, 006, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0164, 003, 0, "040", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0167, 074, 0, Empty, ' ');
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
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "033", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 001, 0, "R", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0010, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0012, 002, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0014, 003, 0, "030", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0017, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, Cedente.TipoCPFCNPJ("0"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 015, 0, Cedente.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0034, 020, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0054, 015, 0, Cedente.CodigoTransmissao, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0069, 005, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0074, 030, 0, Cedente.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0104, 040, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0144, 040, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0184, 008, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0192, 008, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0200, 041, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do lote no arquivo de remessa do CNAB240 SIGCB.", ex);
            }
        }

        private string GerarDetalheSegmentoPRemessaCNAB240(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "033", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, numeroRegistroGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "P", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 002, 0, boleto.CodigoOcorrencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 004, 0, boleto.Banco.Cedente.ContaBancaria.Agencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0022, 001, 0, boleto.Banco.Cedente.ContaBancaria.DigitoAgencia, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0023, 009, 0, boleto.Banco.Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0032, 001, 0, boleto.Banco.Cedente.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0033, 009, 0, boleto.Banco.Cedente.ContaBancaria.Conta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0042, 001, 0, boleto.Banco.Cedente.ContaBancaria.DigitoConta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0043, 002, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0045, 013, 0, boleto.NossoNumero + boleto.NossoNumeroDV, '0');

                if (boleto.TipoCarteira == TipoCarteira.CarteiraCobrancaSimples)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 058, 001, 0, "5", '0');
                if (boleto.TipoCarteira == TipoCarteira.CarteiraCobrancaCaucionada)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 058, 001, 0, "6", '0');
                if (boleto.TipoCarteira == TipoCarteira.CarteiraCobrancaDescontada)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 058, 001, 0, "4", '0');

                if (boleto.Banco.Cedente.ContaBancaria.TipoFormaCadastramento == TipoFormaCadastramento.ComRegistro)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 059, 001, 0, "1", '0');
                if (boleto.Banco.Cedente.ContaBancaria.TipoFormaCadastramento == TipoFormaCadastramento.SemRegistro)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 059, 001, 0, "2", '0');

                if (boleto.Banco.Cedente.ContaBancaria.TipoDocumento == TipoDocumento.Tradicional)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 060, 001, 0, "1", '0');
                if (boleto.Banco.Cedente.ContaBancaria.TipoDocumento == TipoDocumento.Escritural)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 060, 001, 0, "2", '0');

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0061, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0062, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0063, 015, 0, boleto.NumeroDocumento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0078, 008, 0, boleto.DataVencimento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0086, 015, 2, boleto.ValorTitulo, '0');

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0101, 004, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0105, 001, 0, "0", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0106, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0107, 002, 0, (int)boleto.EspecieDocumento, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 001, 0, boleto.Aceite, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0110, 008, 0, boleto.DataEmissao, '0');
                if (boleto.ValorJurosDia == 0)
                {
                    // Sem Juros Mora
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0118, 001, 2, "3", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0119, 008, 0, "0", '0');
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 015, 2, 0, '0');
                }
                else
                {
                    // Com Juros Mora ($)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0118, 001, 2, "1", '0');
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
                    case TipoCodigoProtesto.NaoProtestar:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 001, 0, 0, '0');
                        break;
                    case TipoCodigoProtesto.ProtestarDiasCorridos:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 001, 0, 1, '0');
                        break;
                    case TipoCodigoProtesto.ProtestarDiasUteis:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 001, 0, 2, '0');
                        break;
                    case TipoCodigoProtesto.UtilizarPerfilBeneficiario:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 001, 0, 3, '0');
                        break;
                    case TipoCodigoProtesto.CancelamentoProtestoAutomatico:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 001, 0, 9, '0');
                        break;
                    default:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 001, 0, 0, '0');
                        break;
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0222, 002, 0, boleto.DiasProtesto, '0');
                switch (boleto.CodigoBaixaDevolucao)
                {
                    case TipoCodigoBaixaDevolucao.NaoBaixarNaoDevolver:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0224, 001, 0, 2, '0');
                        break;
                    case TipoCodigoBaixaDevolucao.BaixarDevolver:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0224, 001, 0, 1, '0');
                        break;
                    default:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0224, 001, 0, 0, '0');
                        break;
                }
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0225, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0226, 002, 0, boleto.DiasBaixaDevolucao, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0228, 002, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0230, 011, 0, Empty, ' ');
                reg.CodificarLinha();
                var vLinha = reg.LinhaRegistro;
                return vLinha;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do Segmento P no arquivo de remessa do CNAB240 SIGCB.", ex);
            }
        }

        private string GerarDetalheSegmentoQRemessaCNAB240(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "033", '0');
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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0210, 003, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0213, 003, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0216, 003, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0219, 003, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0222, 019, 0, Empty, ' ');
                reg.CodificarLinha();
                var vLinha = reg.LinhaRegistro;
                return vLinha;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do Segmento Q no arquivo de remessa do CNAB240 SIGCB.", ex);
            }
        }

        private string GerarDetalheSegmentoRRemessaCNAB240(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                string codMulta;
                if (boleto.ValorMulta > 0)
                    codMulta = "1";
                else
                    codMulta = "0";
                var msg = boleto.MensagemArquivoRemessa.PadRight(500, ' ');
                var msg3 = msg.Substring(00, 40).FitStringLength(40, ' ');
                var msg4 = msg.Substring(40, 40).FitStringLength(40, ' ');
                if ((codMulta == "0") & IsNullOrWhiteSpace(msg3) & IsNullOrWhiteSpace(msg4))
                    return "";

                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "033", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, numeroRegistroGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "R", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 002, 0, boleto.CodigoOcorrencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 008, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 015, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0042, 024, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0066, 001, 0, codMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0067, 008, 0, boleto.DataMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0075, 015, 2, boleto.ValorMulta, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0090, 010, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0100, 040, 0, msg3, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 040, 0, msg4, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0180, 061, 0, Empty, ' ');
                reg.CodificarLinha();
                var vLinha = reg.LinhaRegistro;
                return vLinha;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do Segmento Q no arquivo de remessa do CNAB240 SIGCB.", ex);
            }
        }

        private string GerarDetalheSegmentoSRemessaCNAB240(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                var msg5A7 = boleto.MensagemArquivoRemessa.PadRight(500, ' ').Substring(80, 200).FitStringLength(200, ' ');
                if (IsNullOrWhiteSpace(msg5A7))
                    return "";

                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "033", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "3", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0009, 005, 0, numeroRegistroGeral, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "S", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 002, 0, boleto.CodigoOcorrencia, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, "2", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0019, 120, 0, msg5A7, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0139, 040, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0179, 040, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0219, 022, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do Segmento Q no arquivo de remessa do CNAB240 SIGCB.", ex);
            }
        }

        private string GerarTrailerLoteRemessaCNAC240(ref int numeroRegistroGeral,
            int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples,
            int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada,
            int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                // O número de registros no lote é igual ao número de registros gerados + 2 (header e trailler do lote)
                var numeroRegistrosNoLote = numeroRegistroGeral + 2;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "033", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "0001", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "5", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 006, 0, numeroRegistrosNoLote, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 217, 0, Empty, ' ');
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
                var numeroRegistrosNoArquivo = numeroRegistroGeral + 4;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "033", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "9999", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "9", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 006, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 006, 0, numeroRegistrosNoArquivo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0030, 211, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB400.", ex);
            }
        }



    }
}