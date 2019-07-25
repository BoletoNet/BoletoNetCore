using BoletoNetCore.Exceptions;
using System;
using System.Collections.Generic;

namespace BoletoNetCore
{
    public abstract class BancoFebraban<T> where T : class, IBanco, new()
    {
        internal static Lazy<IBanco> Instance { get; } = new Lazy<IBanco>(() => new T());

        public Cedente Cedente { get; set; }
        public int Codigo { get; protected set; }
        public string Nome { get; protected set; }
        public string Digito { get; protected set; }
        public List<string> IdsRetornoCnab400RegistroDetalhe { get; protected set; }
        public bool RemoveAcentosArquivoRemessa { get; protected set; }

        public void ValidaBoleto(Boleto boleto)
        {
        }

        public virtual string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var carteira = CarteiraFactory<T>.ObterCarteira(boleto.CarteiraComVariacao);
            return carteira.FormataCodigoBarraCampoLivre(boleto);
        }

        public virtual void FormataNossoNumero(Boleto boleto)
        {
            var carteira = CarteiraFactory<T>.ObterCarteira(boleto.CarteiraComVariacao);
            carteira.FormataNossoNumero(boleto);
        }

        public virtual string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            return string.Empty;
        }

        //public virtual string GerarDetalheRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        //{
        //    try
        //    {
        //        string detalhe = String.Empty, strline = String.Empty;
        //        switch (tipoArquivo)
        //        {
        //            case TipoArquivo.CNAB240:
        //                // Segmento P (Obrigatório)
        //                detalhe += GerarDetalheSegmentoPRemessaCNAB240(boleto, ref numeroRegistro);

        //                // Segmento Q (Obrigatório)
        //                detalhe += Environment.NewLine;
        //                detalhe += GerarDetalheSegmentoQRemessaCNAB240(boleto, ref numeroRegistro);

        //                // Segmento R (Opcional)
        //                strline = GerarDetalheSegmentoRRemessaCNAB240(boleto, ref numeroRegistro);
        //                if (!string.IsNullOrWhiteSpace(strline))
        //                {
        //                    detalhe += Environment.NewLine;
        //                    detalhe += strline;
        //                }
        //                // Segmento S (Opcional)
        //                strline = GerarDetalheSegmentoSRemessaCNAB240(boleto, ref numeroRegistro);
        //                if (!string.IsNullOrWhiteSpace(strline))
        //                {
        //                    detalhe += Environment.NewLine;
        //                    detalhe += strline;
        //                }
        //                break;
        //            case TipoArquivo.CNAB400:
        //                strline = GerarDetalheRemessaCNAB400Registro1(boleto, ref numeroRegistro);
        //                if (!string.IsNullOrWhiteSpace(strline))
        //                {
        //                    detalhe += strline;
        //                }
        //                strline = GerarDetalheRemessaCNAB400Registro7(boleto, ref numeroRegistro);
        //                if (!string.IsNullOrWhiteSpace(strline))
        //                {
        //                    detalhe += strline;
        //                }
        //                strline = GerarDetalheRemessaCNAB400Registro2(boleto, ref numeroRegistro);
        //                if (!string.IsNullOrWhiteSpace(strline))
        //                {
        //                    detalhe += Environment.NewLine;
        //                    detalhe += strline;
        //                }
        //                strline = GerarDetalheRemessaCNAB400Registro5(boleto, ref numeroRegistro);
        //                if (!string.IsNullOrWhiteSpace(strline))
        //                {
        //                    detalhe += Environment.NewLine;
        //                    detalhe += strline;
        //                }
        //                strline = GerarDetalheRemessaCNAB400Registro5Multa(boleto, ref numeroRegistro);
        //                if (!string.IsNullOrWhiteSpace(strline))
        //                {
        //                    detalhe += Environment.NewLine;
        //                    detalhe += strline;
        //                }
        //                break;
        //            default:
        //                throw new Exception("Tipo de arquivo inexistente.");
        //        }
        //        return detalhe;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw BoletoNetCoreException.ErroAoGerarRegistroDetalheDoArquivoRemessa(ex);
        //    }
        //}

        //protected virtual string GerarDetalheRemessaCNAB400Registro5Multa(Boleto boleto, ref int numeroRegistro)
        //{
        //    return String.Empty;
        //}

        //protected virtual string GerarDetalheRemessaCNAB400Registro5(Boleto boleto, ref int numeroRegistro)
        //{
        //    return String.Empty;
        //}

        //protected virtual string GerarDetalheRemessaCNAB400Registro2(Boleto boleto, ref int numeroRegistro)
        //{
        //    return String.Empty;
        //}

        //protected virtual string GerarDetalheRemessaCNAB400Registro1(Boleto boleto, ref int numeroRegistro)
        //{
        //    return String.Empty;
        //}

        //protected virtual string GerarDetalheRemessaCNAB400Registro7(Boleto boleto, ref int numeroRegistro)
        //{
        //    return String.Empty;
        //}

        //protected virtual string GerarDetalheSegmentoSRemessaCNAB240(Boleto boleto, ref int numeroRegistro)
        //{
        //    return string.Empty;
        //}

        //protected virtual string GerarDetalheSegmentoRRemessaCNAB240(Boleto boleto, ref int numeroRegistro)
        //{
        //    return String.Empty;
        //}

        //protected virtual string GerarDetalheSegmentoQRemessaCNAB240(Boleto boleto, ref int numeroRegistro)
        //{
        //    return String.Empty;
        //}

        //protected virtual string GerarDetalheSegmentoPRemessaCNAB240(Boleto boleto, ref int numeroRegistro)
        //{
        //    return String.Empty;
        //}

        #region Header

        /// <summary>
        /// Construção padrão para Leitura do Header da Remessa. Implementações da classe não precisam necessariamente fazer override deste método
        /// </summary>
        /// <remarks>
        /// A implementação original conta com a necessidade de implementação dos métodos:
        ///     GerarHeaderRemessaCNAB240 e GerarHeaderLoteRemessaCNAB240 para CNAB240
        ///     GerarHeaderRemessaCNAB400 para CNAB400
        /// </remarks>
        /// <param name="tipoArquivo"></param>
        /// <param name="numeroArquivoRemessa"></param>
        /// <param name="numeroRegistro"></param>
        /// <returns></returns>
        public virtual string GerarHeaderRemessa( TipoArquivo tipoArquivo, int numeroArquivoRemessa, ref int numeroRegistro)
        {
            try
            {
                var header = String.Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        // Cabeçalho do Arquivo
                        header += GerarHeaderRemessaCNAB240(numeroArquivoRemessa, ref numeroRegistro);
                        // Cabeçalho do Lote
                        header += Environment.NewLine;
                        header += GerarHeaderLoteRemessaCNAB240(numeroArquivoRemessa, ref numeroRegistro);
                        break;
                    case TipoArquivo.CNAB400:
                        header += GerarHeaderRemessaCNAB400(numeroArquivoRemessa, ref numeroRegistro);
                        break;
                    default:
                        throw new Exception("Tipo de arquivo inexistente.");
                }
                return header;
            }
            catch (Exception ex)
            {
                throw BoletoNetCoreException.ErroAoGerarRegistroHeaderDoArquivoRemessa(ex);
            }

        }

        protected virtual string GerarHeaderRemessaCNAB400(int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            throw new System.NotImplementedException();
        }

        protected virtual string GerarHeaderLoteRemessaCNAB240(int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            throw new System.NotImplementedException();
        }
        protected virtual string GerarHeaderRemessaCNAB240(int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Trailler

        public virtual string GerarTrailerRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa, ref int numeroRegistroGeral, decimal valorBoletoGeral, int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples, int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada, int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada, int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                var trailer = String.Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        // Trailler do Lote
                        trailer += GerarTrailerLoteRemessaCNAB240(ref numeroRegistroGeral,
                            numeroRegistroCobrancaSimples, valorCobrancaSimples,
                            numeroRegistroCobrancaCaucionada, valorCobrancaCaucionada,
                            numeroRegistroCobrancaDescontada, valorCobrancaDescontada);
                        // Trailler do Arquivo
                        trailer += Environment.NewLine;
                        trailer += GerarTrailerRemessaCNAB240(ref numeroRegistroGeral);
                        break;

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
                throw BoletoNetCoreException.ErroAoGerrarRegistroTrailerDoArquivoRemessa(ex);
            }
        }

        protected virtual string GerarTrailerRemessaCNAB400(ref int numeroRegistroGeral)
        {
            throw new NotImplementedException();
        }

        protected virtual string GerarTrailerRemessaCNAB240(ref int numeroRegistroGeral)
        {
            throw new NotImplementedException();
        }

        protected virtual string GerarTrailerLoteRemessaCNAB240(ref int numeroRegistroGeral,
            int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples,
            int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada,
            int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            throw new NotImplementedException();
        }

        #endregion

            //public virtual void LerDetalheRetornoCNAB400Segmento1(ref Boleto boleto, string registro)
            //{
            //    try
            //    {
            //        //Nº Controle do Participante
            //        boleto.NumeroControleParticipante = registro.Substring(37, 25);

            //        //Carteira (no arquivo retorno, vem com 1 caracter. Ajustamos para 2 caracteres, como no manual do Bradesco.
            //        boleto.Carteira = registro.Substring(107, 1).PadLeft(2, '0');
            //        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;

            //        //Identificação do Título no Banco
            //        boleto.NossoNumero = registro.Substring(70, 11); //Sem o DV
            //        boleto.NossoNumeroDV = registro.Substring(81, 1); //DV
            //        boleto.NossoNumeroFormatado = $"{boleto.Carteira}/{boleto.NossoNumero}-{boleto.NossoNumeroDV}";

            //        //Identificação de Ocorrência
            //        boleto.CodigoMovimentoRetorno = registro.Substring(108, 2);
            //        boleto.DescricaoMovimentoRetorno = DescricaoOcorrenciaCnab400(boleto.CodigoMovimentoRetorno);
            //        boleto.CodigoMotivoOcorrencia = registro.Substring(318, 10);

            //        //Número do Documento
            //        boleto.NumeroDocumento = registro.Substring(116, 10);
            //        boleto.EspecieDocumento = AjustaEspecieCnab400(registro.Substring(173, 2));

            //        //Valores do Título
            //        boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(152, 13)) / 100;
            //        boleto.ValorTarifas = Convert.ToDecimal(registro.Substring(175, 13)) / 100;
            //        boleto.ValorOutrasDespesas = Convert.ToDecimal(registro.Substring(188, 13)) / 100;
            //        boleto.ValorIOF = Convert.ToDecimal(registro.Substring(214, 13)) / 100;
            //        boleto.ValorAbatimento = Convert.ToDecimal(registro.Substring(227, 13)) / 100;
            //        boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(240, 13)) / 100;
            //        boleto.ValorPago = Convert.ToDecimal(registro.Substring(253, 13)) / 100;
            //        boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(266, 13)) / 100;
            //        boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(279, 13)) / 100;

            //        //Data Ocorrência no Banco
            //        boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));

            //        //Data Vencimento do Título
            //        boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(146, 6)).ToString("##-##-##"));

            //        // Data do Crédito
            //        boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(295, 6)).ToString("##-##-##"));

            //        // Registro Retorno
            //        boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 400.", ex);
            //    }
            //}

            //protected abstract TipoEspecieDocumento AjustaEspecieCnab400(TipoEspecieDocumento especieDocumento);
            //protected abstract string DescricaoOcorrenciaCnab400(string codigoMovimentoRetorno);

            //public virtual void LerDetalheRetornoCNAB400Segmento7(ref Boleto boleto, string registro)
            //{
            //    throw new System.NotImplementedException();
            //}

        public virtual void LerHeaderRetornoCNAB240(ArquivoRetorno arquivoRetorno, string registro)
        {
            //Manual de Procedimentos Nº 4008.524.0339 - Versão 04 - Elaborado em: 12 / 08 / 2015
            arquivoRetorno.Banco.Cedente = new Cedente();
            //05.0 Tipo de inscrição da empresa 18 - 18 (1)
            //06.0 Número de incrição da empresa 19 - 32 (14)
            arquivoRetorno.Banco.Cedente.CPFCNPJ = registro.Substring(17, 1) == "1" ? registro.Substring(21, 11) : registro.Substring(18, 14);
            //07.0 Código do convênio no banco 33 - 52 (20)
            arquivoRetorno.Banco.Cedente.Codigo = registro.Substring(32, 20).Trim();
            //13.0 Nome da Empresa 73 - 102 (30)
            arquivoRetorno.Banco.Cedente.Nome = registro.Substring(72, 30).Trim();

            ////14.0 Nome do Banco 103 - 132 (30)
            //arquivoRetorno.Banco.Nome = registro.Substring(102, 30);

            arquivoRetorno.Banco.Cedente.ContaBancaria = new ContaBancaria();
            //08.0 Agência mantenedora da conta 53 - 57 (5)
            arquivoRetorno.Banco.Cedente.ContaBancaria.Agencia = registro.Substring(52, 5);
            //09.0 Dígito verificador da agência 58 - 58 (1)
            arquivoRetorno.Banco.Cedente.ContaBancaria.DigitoAgencia = registro.Substring(57, 1);
            //10.0 Número da conta corrente 59 - 70 (12)
            arquivoRetorno.Banco.Cedente.ContaBancaria.Conta = registro.Substring(58, 12);
            //11.0 Dígito verificador da conta 71 - 71 (1)
            arquivoRetorno.Banco.Cedente.ContaBancaria.DigitoConta = registro.Substring(70, 1);

            //17.0 Data de geração do arquivo 144 - 151 (8)
            arquivoRetorno.DataGeracao = Utils.ToDateTime(Utils.ToInt32(registro.Substring(143, 8)).ToString("##-##-####"));
            //19.0 Número seqüencial do arquivo NSA 158 - 163 (6)
            arquivoRetorno.NumeroSequencial = Utils.ToInt32(registro.Substring(157, 6));
        }

        public virtual void LerDetalheRetornoCNAB240SegmentoT(ref Boleto boleto, string registro)
        {
            try
            {
                //Nº Controle do Participante
                boleto.NumeroControleParticipante = registro.Substring(105, 25);

                //Carteira
                boleto.Carteira = registro.Substring(57, 1);
                switch (boleto.Carteira)
                {
                    case "1":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;
                        break;
                    case "2":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaVinculada;
                        break;
                    case "3":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaCaucionada;
                        break;
                    case "4":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaDescontada;
                        break;
                    case "5":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaVendor;
                        break;
                    default:
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;
                        break;
                }

                //Identificação do Título no Banco
                string tmp = registro.Substring(37, 20);
                boleto.NossoNumero = tmp.Substring(8, 11);
                boleto.NossoNumeroDV = tmp.Substring(19, 1);
                boleto.NossoNumeroFormatado = boleto.NossoNumero;

                //Identificação de Ocorrência
                boleto.CodigoMovimentoRetorno = registro.Substring(15, 2);
                boleto.DescricaoMovimentoRetorno = Cnab.MovimentoRetornoCnab240(boleto.CodigoMovimentoRetorno);
                boleto.CodigoMotivoOcorrencia = registro.Substring(213, 10);
                boleto.ListMotivosOcorrencia = Cnab.MotivoOcorrenciaCnab240(boleto.CodigoMotivoOcorrencia, boleto.CodigoMovimentoRetorno);

                //Número do Documento
                boleto.NumeroDocumento = registro.Substring(58, 15);
                boleto.EspecieDocumento = TipoEspecieDocumento.NaoDefinido;

                //Valor do Título
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(81, 15)) / 100;
                boleto.ValorTarifas = Convert.ToDecimal(registro.Substring(198, 15)) / 100;

                //Data Vencimento do Título
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(73, 8)).ToString("##-##-####"));

                //Dados Sacado
                boleto.Sacado = new Sacado();
                string str = registro.Substring(133, 15);
                boleto.Sacado.CPFCNPJ = str.Substring(str.Length - 14, 14);
                boleto.Sacado.Nome = registro.Substring(148, 40);

                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;

                //////
                //18.3T 97 99 3 - Num Banco Cobr./Receb.
                boleto.BancoCobradorRecebedor = registro.Substring(96, 3);
                //19.3T 100 104 5 - Num	Ag. Cobradora
                boleto.AgenciaCobradoraRecebedora = registro.Substring(99, 6);

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 240 / T.", ex);
            }

        }

        public virtual void LerDetalheRetornoCNAB240SegmentoU(ref Boleto boleto, string registro)
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

        public virtual void LerHeaderRetornoCNAB400(string registro)
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

        public virtual void LerTrailerRetornoCNAB400(string registro)
        {
          
        }

        //public virtual void ValidaBoleto(Boleto boleto)
        //{
            
        //}
    }
}