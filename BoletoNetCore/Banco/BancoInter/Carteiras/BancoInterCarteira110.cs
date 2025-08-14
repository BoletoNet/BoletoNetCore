using System;
using BoletoNetCore.Exceptions;
using BoletoNetCore.Extensions;
using static System.String;

namespace BoletoNetCore
{
    [CarteiraCodigo("110")]
    internal class BancoInterCarteira110 : ICarteira<BancoInter>
    {
        internal static Lazy<ICarteira<BancoInter>> Instance { get; } = new Lazy<ICarteira<BancoInter>>(() => new BancoInterCarteira110());

        private BancoInterCarteira110()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {

            // Nosso número não pode ter mais de 11 dígitos

            if (IsNullOrWhiteSpace(boleto.NossoNumero) || boleto.NossoNumero == "0000000000")
            {
                // Banco irá gerar Nosso Número
                boleto.NossoNumero = new String('0', 10);
                boleto.NossoNumeroDV = "0";
                boleto.NossoNumeroFormatado = boleto.NossoNumero + "-" + boleto.NossoNumeroDV;
            }
            else
            {
                var agencia = boleto.Banco.Beneficiario.ContaBancaria.Agencia.PadLeft(4, '0');
                var carteira = boleto.Carteira.PadLeft(3, '0');
                boleto.NossoNumero = boleto.NossoNumero.PadLeft(10, '0');
                boleto.NossoNumeroDV = (agencia + carteira + boleto.NossoNumero).CalcularDVBancoInter();
                var contaBancaria = boleto.Banco.Beneficiario.ContaBancaria;
                boleto.NossoNumeroFormatado = $"{contaBancaria.Agencia}{contaBancaria.DigitoAgencia}/{contaBancaria.CarteiraPadrao}/{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
            }
            

        }

        public virtual string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var contaBancaria = boleto.Banco.Beneficiario.ContaBancaria;
            return $"{contaBancaria.Agencia}{contaBancaria.CarteiraPadrao}{boleto.Banco.Beneficiario.CodigoTransmissao}{boleto.NossoNumero}{boleto.NossoNumeroDV}";
        }
    }
}
