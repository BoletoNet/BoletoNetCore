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

            // Nosso número não pode ter mais de 11 dígitos
            const int tamanhoMaximoNossoNumero = 11;
            if (boleto.NossoNumero.Length > tamanhoMaximoNossoNumero)
                throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve conter {tamanhoMaximoNossoNumero} dígitos.");

            var contaBancaria = boleto.Banco.Beneficiario.ContaBancaria;
            
            if (contaBancaria.DigitoConta.Length != 1)
                throw new NotImplementedException($"Não foi possível formatar o campo livre: Digito da conta ({contaBancaria.DigitoConta}) não possui 1 dígito.");

            if (contaBancaria.Agencia.Length != 4)
                throw new NotImplementedException($"Não foi possível formatar o campo livre: Numero de Agencia ({contaBancaria.Agencia}) não possui 4 dígitos.");

            boleto.NossoNumero = boleto.NossoNumero.PadLeft(tamanhoMaximoNossoNumero, '0');
            boleto.NossoNumeroDV = (boleto.NossoNumero).CalcularDVBancoBTGPactual();
            boleto.NossoNumeroFormatado = $"{boleto.Carteira.PadLeft(3, '0')}/{boleto.NossoNumero.PadLeft(10, '0')}-{boleto.NossoNumeroDV}";
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
             return
                $"{boleto.Banco.Beneficiario.ContaBancaria.Agencia}" +
                $"{Convert.ToInt32(boleto.Carteira).ToString().PadLeft(2, '0')}" +
                $"{boleto.NossoNumero}" +
                $"{boleto.Banco.Beneficiario.ContaBancaria.Conta.Substring(boleto.Banco.Beneficiario.ContaBancaria.Conta.Length - 7)}" +
                $"0";
        }
    }
}