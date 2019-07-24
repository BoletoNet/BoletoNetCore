using System;
using Boleto2Net.Extensions;
using static System.String;

namespace Boleto2Net
{
    [CarteiraCodigo("1")]
    internal class BancoSafraCarteira1 : ICarteira<BancoSafra>
    {
        internal static Lazy<ICarteira<BancoSafra>> Instance { get; } = new Lazy<ICarteira<BancoSafra>>(() => new BancoSafraCarteira1());

        private BancoSafraCarteira1()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {
            if (IsNullOrWhiteSpace(boleto.NossoNumero))
                throw new Exception("Nosso Número não informado.");

            // Nosso número não pode ter mais de 8 dígitos
            if (boleto.NossoNumero.Length > 8)
                throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve conter 8 dígitos.");

            boleto.NossoNumero = boleto.NossoNumero.PadLeft(8, '0');
            boleto.NossoNumeroDV = boleto.NossoNumero.CalcularDVSafra();
            boleto.NossoNumeroFormatado = $"{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            return $"{boleto.Banco.Digito}{boleto.Banco.Cedente.ContaBancaria.Agencia}{boleto.Banco.Cedente.ContaBancaria.DigitoAgencia}00{boleto.Banco.Cedente.ContaBancaria.Conta}{boleto.Banco.Cedente.ContaBancaria.DigitoConta}{boleto.NossoNumero}{boleto.NossoNumeroDV}2";
        }
    }
}