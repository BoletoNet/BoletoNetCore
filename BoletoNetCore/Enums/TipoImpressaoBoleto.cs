namespace BoletoNetCore
{
    public enum TipoImpressaoBoleto
    {
        Banco = 1,
        Empresa = 2,
        BancoPreEmiteClienteComplementa = 3,
        BancoReemite = 4,
        BancoNaoReemite = 5,
        BancoEmitenteAberta = 7,
        BancoEmitenteAutoEnvolopavel = 8
    }
}