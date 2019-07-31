using System;
using BoletoNetCore.Extensions;
using static System.String;

namespace BoletoNetCore
{
    [CarteiraCodigo("101")]
    internal class BancoSantanderCarteira101 : ICarteira<BancoSantander>
    {
        internal static Lazy<ICarteira<BancoSantander>> Instance { get; } = new Lazy<ICarteira<BancoSantander>>(() => new BancoSantanderCarteira101());

        private BancoSantanderCarteira101()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {
            //COBRANÇA SIMPLES - RCR(Rápida com Registro)
            boleto.CarteiraImpressaoBoleto = "RCR";

            if (IsNullOrWhiteSpace(boleto.NossoNumero))
                throw new Exception("Nosso Número não informado.");

            // Nosso número não pode ter mais de 12 dígitos
            if (boleto.NossoNumero.Length > 12)
                throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve conter 12 dígitos.");

            boleto.NossoNumero = boleto.NossoNumero.PadLeft(12, '0');
            boleto.NossoNumeroDV = boleto.NossoNumero.CalcularDVSantander();
            boleto.NossoNumeroFormatado = $"{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            return $"9{boleto.Banco.Beneficiario.Codigo}{boleto.NossoNumero}{boleto.NossoNumeroDV}0101";
        }
    }
}
