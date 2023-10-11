using System;
using BoletoNetCore.Extensions;
using static System.String;

namespace BoletoNetCore
{
    [CarteiraCodigo("112")]
    internal class BancoItauCarteira112 : ICarteira<BancoItau>
    {
        internal static Lazy<ICarteira<BancoItau>> Instance { get; } = new Lazy<ICarteira<BancoItau>>(() => new BancoItauCarteira112());

        private BancoItauCarteira112()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {
            if (IsNullOrWhiteSpace(boleto.NossoNumero) || boleto.NossoNumero == "00000000")
            {
                // Banco irá gerar Nosso Número
                boleto.NossoNumero = new String('0', 8);
                boleto.NossoNumeroDV = "0";
                boleto.NossoNumeroFormatado = $"{boleto.Carteira}/{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
            }
            else
            {
                // Nosso Número informado pela empresa
                // Nosso número não pode ter mais de 8 dígitos
                if (boleto.NossoNumero.Length > 8)
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve conter 8 dígitos.");

                var contaBancaria = boleto.Banco.Beneficiario.ContaBancaria;
                if (contaBancaria.Conta.Length != 5)
                    throw new NotImplementedException($"Não foi possível formatar o campo livre: Número da conta ({contaBancaria.Conta}) não possui 5 dígitos.");

                if (contaBancaria.DigitoConta.Length != 1)
                    throw new NotImplementedException($"Não foi possível formatar o campo livre: Digito da conta ({contaBancaria.DigitoConta}) não possui 1 dígito.");

                if (contaBancaria.Agencia.Length != 4)
                    throw new NotImplementedException($"Não foi possível formatar o campo livre: Numero de Agencia ({contaBancaria.Agencia}) não possui 4 dígitos.");


                boleto.NossoNumero = boleto.NossoNumero.PadLeft(8, '0');
                boleto.NossoNumeroDV = (boleto.Banco.Beneficiario.ContaBancaria.Agencia + boleto.Banco.Beneficiario.ContaBancaria.Conta + boleto.Banco.Beneficiario.ContaBancaria.DigitoConta + boleto.Carteira + boleto.NossoNumero).CalcularDVItau();
                boleto.NossoNumeroFormatado = $"{boleto.Carteira}/{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
            }
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            return $"{boleto.Carteira}{boleto.NossoNumero}{boleto.NossoNumeroDV}{boleto.Banco.Beneficiario.ContaBancaria.Agencia}{boleto.Banco.Beneficiario.ContaBancaria.Conta}{boleto.Banco.Beneficiario.ContaBancaria.DigitoConta}000";
        }
    }
}