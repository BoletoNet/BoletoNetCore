using System;
using Boleto2Net.Extensions;
using static System.String;

namespace Boleto2Net
{
    [CarteiraCodigo("SIG14")]
    internal class BancoCaixaCarteiraSIG14 : ICarteira<BancoCaixa>
    {
        internal static Lazy<ICarteira<BancoCaixa>> Instance { get; } = new Lazy<ICarteira<BancoCaixa>>(() => new BancoCaixaCarteiraSIG14());

        private BancoCaixaCarteiraSIG14()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {
            boleto.CarteiraImpressaoBoleto = "RG";

            // Carteira SIG14: Dúvida: Se o Cliente SEMPRE emite o boleto, pois o nosso número começa com 14, o que significa Título Registrado emissão Empresa:
            // O nosso número não pode ser em branco.
            if (IsNullOrWhiteSpace(boleto.NossoNumero))
                throw new Exception("Nosso Número não informado.");

            if (boleto.NossoNumero.Length == 17)
            {
                // Se o Nosso Número tem 17 dígitos, obrigatoriamente deve iniciar com "14"
                if (!boleto.NossoNumero.StartsWith("14"))
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve iniciar com \"14\" e conter 17 dígitos.");
            }
            else
            {
                // Se o Nosso Número não tem 17 dígitos, deve ser informado com até 15 posições, para ser adicionado automaticamente o "14" no inicio totalizando 17 dígitos.
                if (boleto.NossoNumero.Length > 15)
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve iniciar com \"14\" e conter 17 dígitos.");
                boleto.NossoNumero = $"14{boleto.NossoNumero.PadLeft(15, '0')}";
            }

            boleto.NossoNumeroDV = boleto.NossoNumero.CalcularDVCaixa();
            boleto.NossoNumeroFormatado = $"{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var cedente = boleto.Banco.Cedente;
            var contaBancaria = cedente.ContaBancaria;
            var formataCampoLivre = $"{cedente.Codigo}{cedente.CodigoDV}{boleto.NossoNumero.Substring(2, 3)}1{boleto.NossoNumero.Substring(5, 3)}4{boleto.NossoNumero.Substring(8, 9)}";
            formataCampoLivre += formataCampoLivre.CalcularDVCaixa();
            return formataCampoLivre;
        }
    }
}
