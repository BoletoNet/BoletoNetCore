using BoletoNetCore.Extensions;
using System;

namespace BoletoNetCore
{
    [CarteiraCodigo("1/A")]
    internal class BancoSicrediCarteira1 : ICarteira<BancoSicredi>
    {
        internal static Lazy<ICarteira<BancoSicredi>> Instance { get; } = new Lazy<ICarteira<BancoSicredi>>(() => new BancoSicrediCarteira1());

        private BancoSicrediCarteira1()
        {

        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            string CampoLivre = boleto.Carteira + "1" +
                boleto.NossoNumero + boleto.NossoNumeroDV +
                boleto.Banco.Beneficiario.ContaBancaria.Agencia +
                boleto.Banco.Beneficiario.ContaBancaria.OperacaoConta +
                boleto.Banco.Beneficiario.Codigo + "10";

            CampoLivre += CampoLivre.CalcularDVSicredi();

            return CampoLivre;
        }

        public void FormataNossoNumero(Boleto boleto)
        {
            if (string.IsNullOrWhiteSpace(boleto.NossoNumero))
                throw new Exception("Nosso Número não informado.");

            //Formato Nosso Número
            //AA/BXXXXX-D, onde:
            //AA = Ano(pode ser diferente do ano corrente)
            //B = Byte de geração(0 a 9). O Byte 1 só poderá ser informado pela Cooperativa
            //XXXXX = Número livre de 00000 a 99999
            //D = Dígito verificador pelo módulo 11
            
            // Nosso número não pode ter mais de 8 dígitos
            if (boleto.NossoNumero.Length == 7 || boleto.NossoNumero.Length > 8)
                throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve conter até 5 dígitos ou exatamente 6 ou 8 dígitos.");
            else if (boleto.NossoNumero.Length <= 5)
            {
                boleto.NossoNumero = string.Format("{0}2{1}", boleto.DataEmissao.ToString("yy"), boleto.NossoNumero.PadLeft(5, '0'));
            }
            else if (boleto.NossoNumero.Length == 6)
            {
                if (boleto.NossoNumero.Substring(0, 1) == "1")
                {
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) de 6 dígitos não pode começar com 1 (Reservado para Cooperativa).");
                }
                boleto.NossoNumero = string.Format("{0}{1}", boleto.DataEmissao.ToString("yy"), boleto.NossoNumero);
            }
            else
            {
                if (boleto.NossoNumero.Substring(2, 1) == "1")
                {
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) de 8 dígitos não pode ter o Byte (3a. posição) como 1 (Reservado para Cooperativa).");
                }
            }

            if (string.IsNullOrEmpty(boleto.Banco.Beneficiario.ContaBancaria.OperacaoConta))
            {
                throw new Exception($"Posto beneficiário não preenchido");
            }

            boleto.NossoNumeroDV = (boleto.Banco.Beneficiario.ContaBancaria.Agencia + boleto.Banco.Beneficiario.ContaBancaria.OperacaoConta + boleto.Banco.Beneficiario.Codigo + boleto.NossoNumero).CalcularDVSicredi();

            boleto.NossoNumeroFormatado = string.Format("{0}/{1}-{2}", boleto.NossoNumero.Substring(0, 2), boleto.NossoNumero.Substring(2, 6), boleto.NossoNumeroDV);
        }
    }
}
