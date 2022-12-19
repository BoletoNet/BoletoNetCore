using System;
using BoletoNetCore.Exceptions;
using BoletoNetCore.Extensions;
using static System.String;

namespace BoletoNetCore
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
            if (boleto.Banco.Beneficiario.ContaBancaria.Agencia.Length != 4)
            {
                throw BoletoNetCoreException.AgenciaInvalida(boleto.Banco.Beneficiario.ContaBancaria.Agencia, 4);
            }

            if (boleto.Banco.Beneficiario.ContaBancaria.DigitoAgencia.Length != 1)
            {
                throw BoletoNetCoreException.AgenciaDigitoInvalido(boleto.Banco.Beneficiario.ContaBancaria.DigitoAgencia, 1);
            }

            if (boleto.Banco.Beneficiario.ContaBancaria.Conta.Length != 6)
            {
                throw BoletoNetCoreException.ContaInvalida(boleto.Banco.Beneficiario.ContaBancaria.Conta, 6);
            }

            if (boleto.Banco.Beneficiario.ContaBancaria.DigitoConta.Length != 1)
            {
                throw BoletoNetCoreException.ContaDigitoInvalido(boleto.Banco.Beneficiario.ContaBancaria.DigitoConta, 1);
            }

            if (boleto.NossoNumero.Length != 8)
            {
                throw BoletoNetCoreException.NossoNumeroInvalido(boleto.NossoNumero, 8);
            }

            if (boleto.NossoNumeroDV.Length != 1)
            {
                throw BoletoNetCoreException.NossoNumeroInvalido(boleto.NossoNumeroDV, 1);
            }
            
            return $"{boleto.Banco.Digito}{boleto.Banco.Beneficiario.ContaBancaria.Agencia}{boleto.Banco.Beneficiario.ContaBancaria.DigitoAgencia}00{boleto.Banco.Beneficiario.ContaBancaria.Conta}{boleto.Banco.Beneficiario.ContaBancaria.DigitoConta}{boleto.NossoNumero}{boleto.NossoNumeroDV}2";
        }
    }
}