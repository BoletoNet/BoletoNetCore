namespace BoletoNetCore
{
    /// <summary>
    /// Indica os tipos de registro possíveis em um arquivo EDI
    /// </summary>
    public enum TTipoRegistroEDI
    {
        /// <summary>
        /// Indicador de registro Header
        /// </summary>
        treHeader,
        /// <summary>
        /// Indica um registro detalhe
        /// </summary>
        treDetalhe,
        /// <summary>
        /// Indica um registro Trailler
        /// </summary>
        treTrailler,
        /// <summary>
        /// Indica um registro sem definições, utilizado para transmissão socket ou similar
        /// </summary>
        treLinhaUnica
    }
}