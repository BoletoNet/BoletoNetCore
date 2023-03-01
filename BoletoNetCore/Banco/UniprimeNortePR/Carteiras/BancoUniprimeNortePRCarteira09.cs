using BoletoNetCore.Extensions;
using System;

namespace BoletoNetCore
{
    [CarteiraCodigo("09")]
    public class BancoUniprimeNortePRCarteira09 : ICarteira<BancoUniprimeNortePR>
    {
        internal static Lazy<ICarteira<BancoUniprimeNortePR>> Instance { get; } = new Lazy<ICarteira<BancoUniprimeNortePR>>(() => new BancoUniprimeNortePRCarteira09());

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var contaBancaria = boleto.Banco.Beneficiario.ContaBancaria;
            var agencia = contaBancaria.Agencia.PadLeft(4, '0');
            var nrconta = contaBancaria.Conta.PadLeft(7, '0');
            //return $"{agencia}{boleto.Carteira}{boleto.NossoNumero}{boleto.NossoNumeroDV}{nrconta}0";
            var ret = $"{agencia}{boleto.Carteira}{boleto.NossoNumero}{nrconta}0";
            return ret;
        }

        public void FormataNossoNumero(Boleto boleto)
        {
            boleto.Carteira = boleto.Carteira.PadLeft(2, '0');
            boleto.NossoNumero = boleto.NossoNumero.PadLeft(11, '0');
            boleto.NossoNumeroDV = $"{boleto.Carteira}{boleto.NossoNumero}".CalcularDVUniprimeNortePR();
            boleto.NossoNumeroFormatado = $"{boleto.Carteira}/{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
        }
    }
}