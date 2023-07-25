using BoletoNetCore.Exceptions;
using System;
using System.Collections.Generic;

namespace BoletoNetCore
{
    public abstract partial class BancoFebraban<T> where T : class, IBanco, new()
    {
        internal static Lazy<IBanco> Instance { get; } = new Lazy<IBanco>(() => new T());

        public Beneficiario Beneficiario { get; set; }
        public int Codigo { get; protected set; }
        public string Nome { get; protected set; }
        public string Digito { get; protected set; }
        public int TamanhoAgencia => 4;
        public int TamanhoConta => 10;

        public List<string> IdsRetornoCnab400RegistroDetalhe { get; protected set; }
        public bool RemoveAcentosArquivoRemessa { get; protected set; }
        public bool DescontoDuplicatas { get; protected set; }

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

        public virtual string GerarDetalheRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            try
            {
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        return ((IBancoCNAB240)this)?.GerarDetalheRemessaCNAB240(boleto, ref numeroRegistro);
                    case TipoArquivo.CNAB400:
                        return ((IBancoCNAB400)this)?.GerarDetalheRemessaCNAB400(boleto, ref numeroRegistro);
                    case TipoArquivo.CNAB150:
                        return ((IBancoCNAB150)this)?.GerarDetalheRemessaCNAB150(boleto, ref numeroRegistro);
                    default:
                        throw new Exception("Tipo de arquivo inexistente.");
                }
            }
            catch (Exception ex)
            {
                throw BoletoNetCoreException.ErroAoGerarRegistroDetalheDoArquivoRemessa(ex);
            }
        }


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
        public virtual string GerarHeaderRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa, ref int numeroRegistro)
        {
            try
            {
                var header = String.Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        // Cabeçalho do Arquivo
                        header += ((IBancoCNAB240)this).GerarHeaderRemessaCNAB240(ref numeroArquivoRemessa, ref numeroRegistro);
                        // Cabeçalho do Lote
                        header += Environment.NewLine;
                        header += ((IBancoCNAB240)this).GerarHeaderLoteRemessaCNAB240(ref numeroArquivoRemessa, ref numeroRegistro);
                        break;
                    case TipoArquivo.CNAB400:
                        header += ((IBancoCNAB400)this).GerarHeaderRemessaCNAB400(ref numeroArquivoRemessa, ref numeroRegistro);
                        break;
                    case TipoArquivo.CNAB150:
                        header += ((IBancoCNAB150)this).GerarHeaderRemessaCNAB150(ref numeroArquivoRemessa, ref numeroRegistro);
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

        #endregion

        #region Trailler

        public virtual string GerarTrailerRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa,
            ref int numeroRegistroGeral, decimal valorBoletoGeral,
            int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples,
            int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada,
            int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada,
            int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                var trailer = String.Empty;
                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        return ((IBancoCNAB240)this).GerarTrailerLoteRemessaCNAB240(
                                             ref numeroArquivoRemessa, numeroRegistroGeral,
                                             numeroRegistroCobrancaSimples, valorCobrancaSimples,
                                             numeroRegistroCobrancaVinculada, valorCobrancaVinculada,
                                             numeroRegistroCobrancaCaucionada, valorCobrancaCaucionada,
                                             numeroRegistroCobrancaDescontada, valorCobrancaDescontada) + Environment.NewLine
                                             + ((IBancoCNAB240)this).GerarTrailerRemessaCNAB240(
                                             numeroRegistroGeral, valorBoletoGeral,
                                             numeroRegistroCobrancaSimples, valorCobrancaSimples,
                                             numeroRegistroCobrancaVinculada, valorCobrancaVinculada,
                                             numeroRegistroCobrancaCaucionada, valorCobrancaCaucionada,
                                             numeroRegistroCobrancaDescontada, valorCobrancaDescontada);

                    case TipoArquivo.CNAB400:
                        return ((IBancoCNAB400)this).GerarTrailerRemessaCNAB400(
                                             numeroRegistroGeral, valorBoletoGeral,
                                             numeroRegistroCobrancaSimples, valorCobrancaSimples,
                                             numeroRegistroCobrancaVinculada, valorCobrancaVinculada,
                                             numeroRegistroCobrancaCaucionada, valorCobrancaCaucionada,
                                             numeroRegistroCobrancaDescontada, valorCobrancaDescontada);
                    case TipoArquivo.CNAB150:
                        return ((IBancoCNAB150)this).GerarTrailerLoteRemessaCNAB150(
                                            ref numeroArquivoRemessa, numeroRegistroGeral, valorBoletoGeral,
                                            numeroRegistroCobrancaSimples, valorCobrancaSimples,
                                            numeroRegistroCobrancaVinculada, valorCobrancaVinculada,
                                            numeroRegistroCobrancaCaucionada, valorCobrancaCaucionada,
                                            numeroRegistroCobrancaDescontada, valorCobrancaDescontada) + Environment.NewLine
                                            + ((IBancoCNAB150)this).GerarTrailerRemessaCNAB150(
                                            numeroRegistroGeral, valorBoletoGeral,
                                            numeroRegistroCobrancaSimples, valorCobrancaSimples,
                                            numeroRegistroCobrancaVinculada, valorCobrancaVinculada,
                                            numeroRegistroCobrancaCaucionada, valorCobrancaCaucionada,
                                            numeroRegistroCobrancaDescontada, valorCobrancaDescontada);
                    default:
                        throw new Exception("Tipo de arquivo inexistente.");
                }
            }
            catch (Exception ex)
            {
                throw BoletoNetCoreException.ErroAoGerrarRegistroTrailerDoArquivoRemessa(ex);
            }
        }

        #endregion

    }
}