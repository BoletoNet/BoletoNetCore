namespace BoletoNetCore
{
    public enum TipoEspecieDocumento
    {
        /// <summary>Não Informado</summary>
        NaoDefinido = 00,

        /// <summary>Cheque</summary>
        CH = 01,

        /// <summary>Duplicata Mercantil</summary>
        DM = 02,

        /// <summary> Duplicata Mercantil p/ Indicação</summary>
        DMI = 03,

        /// <summary>Duplicata de Serviço</summary>
        DS = 04,

        /// <summary>Duplicata de Serviço p/ Indicação</summary>
        DSI = 05,

        /// <summary>Duplicata Rural</summary>
        DR = 06,

        /// <summary>Letra de Câmbio</summary>
        LC = 07,

        /// <summary>Nota de Crédito Comercial</summary>
        NCC = 08,

        /// <summary> Nota de Crédito a Exportação</summary>
        NCE = 09, //

        /// <summary>Nota de Crédito Industrial</summary>
        NCI = 10,

        /// <summary>Nota de Crédito Rural</summary>
        NCR = 11,

        /// <summary>Nota Promissória</summary>
        NP = 12,

        /// <summary>Nota Promissória Rural</summary>
        NPR = 13,

        /// <summary>Triplicata Mercantil</summary>
        TM = 14,

        /// <summary>Triplicata de Serviço</summary>
        TS = 15,

        /// <summary>Nota de Seguro</summary>
        NS = 16,

        /// <summary>Recibo</summary>
        RC = 17,

        /// <summary>Fatura</summary>
        FAT = 18,

        /// <summary>Nota de Débito</summary>
        ND = 19,

        /// <summary>Apólice de Seguro</summary>
        AP = 20,

        /// <summary>Mensalidade Escolar</summary>
        ME = 21,

        /// <summary>Parcela de Consórcio</summary>
        PC = 22,

        /// <summary>Nota Fiscal</summary>
        NF = 23,

        /// <summary>Documento de Dívida</summary>
        DD = 24,

        /// <summary>Cédula de Produto Rural</summary>
        CPR = 25,

        /// <summary>Warrant</summary>
        WAR = 26,

        /// <summary>Dívida Ativa Estado</summary>
        DAE = 27,

        /// <summary>Dívida Ativa Município</summary>
        DAM = 28,

        /// <summary>Dívida Ativa União</summary>
        DAU = 29,

        /// <summary>Encargos Condominiais</summary>
        EC = 30,

        /// <summary>Cartão de Crédito</summary>
        CC = 31,

        /// <summary>Boleto Proposta</summary>
        BP = 32,

        /// <summary>Outros</summary>
        OU = 99
    }
}