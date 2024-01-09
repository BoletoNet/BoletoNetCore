using System;
using BoletoNetCore.Extensions;
using static System.String;

namespace BoletoNetCore
{
    [CarteiraCodigo("101/2")]
    internal class BancoSantanderCarteira1012 : ICarteira<BancoSantander>
    {
        internal static Lazy<ICarteira<BancoSantander>> Instance { get; } = new Lazy<ICarteira<BancoSantander>>(() => new BancoSantanderCarteira1012());

        private BancoSantanderCarteira1012()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {
            //COBRANÇA SIMPLES - RCR(Rápida com Registro)
            boleto.CarteiraImpressaoBoleto = "RCR";

            if (IsNullOrWhiteSpace(boleto.NossoNumero))
                throw new Exception("Nosso Número não informado.");

            // Nosso número não pode ter mais de 13 dígitos
            if (boleto.NossoNumero.Length > 13)
                throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve conter 13 dígitos.");

            boleto.NossoNumero = boleto.NossoNumero.PadLeft(13, '0');
            boleto.NossoNumeroDV = boleto.NossoNumero.CalcularDVSantander();
            boleto.NossoNumeroFormatado = $"{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            return $"9{boleto.Banco.Beneficiario.Codigo}{boleto.NossoNumero}0101";
        }
    }
}
