using System;
using BoletoNetCore.Extensions;

namespace BoletoNetCore.BTGPactual.Carteiras
{
    [CarteiraCodigo("1")]
    public class BancoBTGPactualCarteira01:  ICarteira<BancoBTGPactual>
    {
        internal static Lazy<ICarteira<BancoBTGPactual>> Instance { get; } = new Lazy<ICarteira<BancoBTGPactual>>(() => new BancoBTGPactualCarteira01());
        
        public BancoBTGPactualCarteira01()
        {
            
        }
        
        public void FormataNossoNumero(Boleto boleto)
        {
            if (string.IsNullOrWhiteSpace(boleto.NossoNumero))
                throw new Exception("Nosso Número não informado.");

            // Nosso número não pode ter mais de 8 dígitos
            if (boleto.NossoNumero.Length > 8)
                throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve conter 8 dígitos.");

            var contaBancaria = boleto.Banco.Beneficiario.ContaBancaria;
            
            if (contaBancaria.DigitoConta.Length != 1)
                throw new NotImplementedException($"Não foi possível formatar o campo livre: Digito da conta ({contaBancaria.DigitoConta}) não possui 1 dígito.");

            if (contaBancaria.Agencia.Length != 4)
                throw new NotImplementedException($"Não foi possível formatar o campo livre: Numero de Agencia ({contaBancaria.Agencia}) não possui 4 dígitos.");

            boleto.NossoNumero = boleto.NossoNumero.PadLeft(8, '0');
            boleto.NossoNumeroDV = (boleto.Banco.Beneficiario.ContaBancaria.Agencia + boleto.Banco.Beneficiario.ContaBancaria.Conta + boleto.Carteira + boleto.NossoNumero).CalcularDVBancoBTGPactual();
            boleto.NossoNumeroFormatado = $"{boleto.Carteira}/{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
             return
                $"00" +
                $"{boleto.Carteira}" +
                $"{boleto.NossoNumero}" +
                $"{boleto.NossoNumeroDV}" +
                $"{boleto.Banco.Beneficiario.ContaBancaria.Agencia}" +
                $"{boleto.Banco.Beneficiario.ContaBancaria.Conta}" +
                $"{boleto.Banco.Beneficiario.ContaBancaria.DigitoConta}" +
                $"";
        }
    }
}