using System.Collections.Generic;

namespace BoletoNetCore
{
    public interface IBanco
    {
        /// <summary>
        /// Beneficiário de uma Cobrança
        /// </summary>
        Beneficiario Beneficiario { get; set; }
        int Codigo { get; }
        string Nome { get; }
        string Digito { get; }
        List<string> IdsRetornoCnab400RegistroDetalhe { get; } // Identifica os registros que cada banco implementa no arquivo retorno, sendo que o primeiro ID da List<> identifica um novo boleto dentro do arquivo retorno.
        bool RemoveAcentosArquivoRemessa { get; }

        /// <summary>
        /// Formata o beneficiário (Agência, Conta, Código)
        /// </summary>
        void FormataBeneficiario();
        /// <summary>
        /// Formata o campo livre do código de barras
        /// </summary>
        string FormataCodigoBarraCampoLivre(Boleto boleto);
        /// <summary>
        /// Formata o nosso número
        /// </summary>
        void FormataNossoNumero(Boleto boleto);
        /// <summary>
        /// Responsável pela validação de todos os dados referente ao banco, que serão usados no boleto
        /// </summary>
        void ValidaBoleto(Boleto boleto);

        /// <summary>
        /// Gera o header do arquivo de remessa
        /// </summary>
        string GerarHeaderRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa, ref int numeroRegistro);

        /// <summary>
        /// Gera o Trailer do arquivo de remessa
        /// </summary>
        string GerarDetalheRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro);

        /// <summary>
        /// Gera o Trailer do arquivo de remessa
        /// </summary>
        string GerarTrailerRemessa(TipoArquivo tipoArquivo, int numeroArquivoRemessa,
                                            ref int numeroRegistroGeral, decimal valorBoletoGeral,
                                            int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples,
                                            int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada,
                                            int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada,
                                            int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada);

        string FormatarNomeArquivoRemessa(int numeroSequencial);
    }

    /// <summary>
    /// Implementa Remessa e Tetorno de Cobrança no Formato CNAB400
    /// </summary>
    public interface IBancoCNAB400 : IBanco
    {
        //remessa
        string GerarHeaderRemessaCNAB400(ref int numeroArquivoRemessa, ref int numeroRegistroGeral);
        string GerarDetalheRemessaCNAB400(Boleto boleto, ref int registro);
        string GerarTrailerRemessaCNAB400(int numeroRegistroGeral, decimal valorBoletoGeral,
            int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples,
            int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada,
            int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada,
            int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada);

        //retorno
        void LerHeaderRetornoCNAB400(string registro);
        void LerDetalheRetornoCNAB400Segmento1(ref Boleto boleto, string registro);
        void LerDetalheRetornoCNAB400Segmento7(ref Boleto boleto, string registro);
        void LerTrailerRetornoCNAB400(string registro);
    }

    /// <summary>
    /// Implementa Remessa e Retorno de Cobrança no formato CNAB240 em uma Intituição Financeira
    /// </summary>
    public interface IBancoCNAB240 : IBanco
    {
        /// <summary>
        /// 1 - Header de Remessa e Lote do Arquivo de Remessa
        /// </summary>
        /// <param name="numeroArquivoRemessa"></param>
        /// <param name="numeroRegistro"></param>
        /// <returns></returns>
        string GerarHeaderRemessaCNAB240(ref int numeroArquivoRemessa, ref int numeroRegistro);
        string GerarHeaderLoteRemessaCNAB240(ref int numeroArquivoRemessa, ref int numeroRegistro);
        string GerarDetalheRemessaCNAB240(Boleto boleto, ref int registro);
        string GerarTrailerLoteRemessaCNAB240(ref int numeroArquivoRemessa, int numeroRegistroGeral,
            int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples,
            int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada,
            int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada,
            int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada);

        string GerarTrailerRemessaCNAB240(int numeroRegistroGeral, decimal valorBoletoGeral,
            int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples,
            int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada,
            int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada,
            int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada);

        void LerHeaderRetornoCNAB240(ArquivoRetorno arquivoRetorno, string registro);
        void LerDetalheRetornoCNAB240SegmentoT(ref Boleto boleto, string registro);
        void LerDetalheRetornoCNAB240SegmentoU(ref Boleto boleto, string registro);
    }

    /// <summary>
    /// Implementa Registro Online de Boleto
    /// </summary>
    public interface IBancoOnlineRest : IBanco
    {
        string GerarToken();
        void RegistrarBoleto(ref Boleto boleto, string registro);
        //StatusBoletoOnline ConsultarStatus(ref Boleto boleto, string registro);
    }
}
