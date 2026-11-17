using System;
using BoletoNetCore.Extensions;
using static System.String;

namespace BoletoNetCore
{
    /// <summary>
    /// Carteira CS (Cobrança Simples) do Banese.
    /// Campo livre (25 posições): Agência (2) + Conta (9) + Nosso Número (8) + DV Nosso Número (1) + Banco (047) + NC1 + NC2
    /// </summary>
    [CarteiraCodigo("CS")]
    internal class BancoBaneseCarteiraCS : ICarteira<BancoBanese>
    {
        internal static Lazy<ICarteira<BancoBanese>> Instance { get; } = new Lazy<ICarteira<BancoBanese>>(() => new BancoBaneseCarteiraCS());

        private BancoBaneseCarteiraCS()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {
            boleto.CarteiraImpressaoBoleto = "CS";

            if (IsNullOrWhiteSpace(boleto.NossoNumero))
                throw new Exception("Nosso Número não informado.");

            if (boleto.NossoNumero.Length > 8)
                throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve conter até 8 dígitos.");

            boleto.NossoNumero = boleto.NossoNumero.PadLeft(8, '0');
            boleto.NossoNumeroDV = (boleto.Banco.Beneficiario.ContaBancaria.Agencia + boleto.NossoNumero).CalcularDVBanese();
            boleto.NossoNumeroFormatado = $"{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
            boleto.ImprimirMensagemInstrucao = true;
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var contaBancaria = boleto.Banco.Beneficiario.ContaBancaria;

            if (contaBancaria.Agencia.Length != 4)
                throw new Exception($"Número da agência ({contaBancaria.Agencia}) deve conter 4 dígitos");
            if (contaBancaria.Conta.Length != 9)
                throw new Exception($"Número da conta ({contaBancaria.Conta}) deve conter 9 dígitos");

            var campoLivre = $"{contaBancaria.Agencia.Substring(2, 2)}{contaBancaria.Conta}{boleto.NossoNumero}{boleto.NossoNumeroDV}047";
            return campoLivre + campoLivre.CalcularNCsBanese();
        }
    }
}
