using System;
using BoletoNetCore.Extensions;
using static System.String;

namespace BoletoNetCore
{
    [CarteiraCodigo("109")]
    internal class BancoItauCarteira109 : ICarteira<BancoItau>
    {
        internal static Lazy<ICarteira<BancoItau>> Instance { get; } = new Lazy<ICarteira<BancoItau>>(() => new BancoItauCarteira109());

        private BancoItauCarteira109()
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
            boleto.NossoNumeroDV = (boleto.Banco.Beneficiario.ContaBancaria.Agencia + boleto.Banco.Beneficiario.ContaBancaria.Conta + boleto.Carteira + boleto.NossoNumero).CalcularDVItau();
            boleto.NossoNumeroFormatado = $"{boleto.Carteira}/{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            return $"{boleto.Carteira}{boleto.NossoNumero}{boleto.NossoNumeroDV}{boleto.Banco.Beneficiario.ContaBancaria.Agencia}{boleto.Banco.Beneficiario.ContaBancaria.Conta}{boleto.Banco.Beneficiario.ContaBancaria.DigitoConta}000";
        }
    }
}