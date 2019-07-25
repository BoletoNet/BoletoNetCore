using System;
using Boleto2Net.Extensions;
using static System.String;

namespace Boleto2Net
{
    [CarteiraCodigo("1/01")]
    internal class BancoSincoobCarteira1: ICarteira<BancoSicoob>
    {
        internal static Lazy<ICarteira<BancoSicoob>> Instance { get; } = new Lazy<ICarteira<BancoSicoob>>(() => new BancoSincoobCarteira1());

        private BancoSincoobCarteira1()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {
            var cedente = boleto.Banco.Cedente;
            if (cedente.ContaBancaria.TipoImpressaoBoleto == TipoImpressaoBoleto.Empresa & boleto.NossoNumero == Empty)
                throw new Exception("Nosso Número não informado.");
            
            // Nosso número não pode ter mais de 7 dígitos
            if (boleto.NossoNumero.Length > 7)
                throw new Exception("Nosso Número (" + boleto.NossoNumero + ") deve conter 7 dígitos.");

            boleto.NossoNumero = boleto.NossoNumero.PadLeft(7, '0');

            // Base para calcular DV: Agencia (4 caracteres) Código do Cedente com dígito (10 caracteres) Nosso Número (7 caracteres)
            var baseCalculoDV = $"{cedente.ContaBancaria.Agencia}{cedente.Codigo.PadLeft(9, '0')}{cedente.CodigoDV}{boleto.NossoNumero}";
            boleto.NossoNumeroDV = baseCalculoDV.CalcularDVSicoob();
            boleto.NossoNumeroFormatado = $"{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var cedente = boleto.Banco.Cedente;
            var contaBancaria = cedente.ContaBancaria;
            return $"{boleto.Carteira}{contaBancaria.Agencia}{boleto.VariacaoCarteira}{cedente.Codigo}{cedente.CodigoDV}{boleto.NossoNumero}{boleto.NossoNumeroDV}001";
        }
    }
}
