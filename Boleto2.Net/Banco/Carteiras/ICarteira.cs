namespace Boleto2Net
{
    internal interface ICarteira<T>
        where T : IBanco
    {
        void FormataNossoNumero(Boleto boleto);
        string FormataCodigoBarraCampoLivre(Boleto boleto);
    }
}
