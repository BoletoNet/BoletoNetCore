using System;
using BoletoNetCore.Extensions;
using static System.String;

namespace BoletoNetCore
{
    public abstract class BancoBradescoCarteiraBase
    {
        public virtual void FormataNossoNumero(Boleto boleto)
        {

            // Nosso número não pode ter mais de 11 dígitos

            if (IsNullOrWhiteSpace(boleto.NossoNumero) || boleto.NossoNumero == "00000000000")
            {
                // Banco irá gerar Nosso Número
                boleto.NossoNumero = new String('0', 11);
                boleto.NossoNumeroDV = "0";
                boleto.NossoNumeroFormatado = "000/00000000000-0";
            }
            else
            {
                // Nosso Número informado pela empresa
                if (boleto.NossoNumero.Length > 11)
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve conter 11 dígitos.");
                boleto.NossoNumero = boleto.NossoNumero.PadLeft(11, '0');
                boleto.NossoNumeroDV = (boleto.Carteira + boleto.NossoNumero).CalcularDVBradesco();
                boleto.NossoNumeroFormatado = $"{boleto.Carteira.PadLeft(3, '0')}/{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
            }

        }

        public virtual string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var contaBancaria = boleto.Banco.Beneficiario.ContaBancaria;


            if (contaBancaria.Conta.Length != 7)
                throw new NotImplementedException($"Não foi possível formatar o campo livre: Número da conta ({contaBancaria.Conta}) não possui 7 dígitos.");
            if (contaBancaria.Agencia.Length != 4)
                throw new NotImplementedException($"Não foi possível formatar o campo livre: Número da Agência ({contaBancaria.Agencia}) não possui 4 dígitos.");

            return $"{contaBancaria.Agencia}{boleto.Carteira}{boleto.NossoNumero}{contaBancaria.Conta}{"0"}";
        }
    }
}
