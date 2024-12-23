using System;
using BoletoNetCore.Extensions;
using static System.String;

namespace BoletoNetCore
{
    [CarteiraCodigo("121")]
    internal class BancoDaycovalCarteira121 : ICarteira<BancoDaycoval>
    {
        internal static Lazy<ICarteira<BancoDaycoval>> Instance { get; } = new Lazy<ICarteira<BancoDaycoval>>(() => new BancoDaycovalCarteira121());

        private BancoDaycovalCarteira121()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {
            boleto.CarteiraImpressaoBoleto = "121";

            // O nosso número não pode ser em branco.
            if (IsNullOrWhiteSpace(boleto.NossoNumero))
                throw new Exception("Nosso Número não informado.");

            if (boleto.NossoNumero.Length != 08)
            {
                // Se o Nosso Número tem 08 dígitos, nosso numero invalido
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve conter 08 dígitos.");
            }

            boleto.NossoNumero = boleto.NossoNumero.PadLeft(8, '0');
            boleto.NossoNumeroDV = (boleto.Banco.Beneficiario.ContaBancaria.Agencia + boleto.Carteira + boleto.NossoNumero).CalcularDVDaycoval();
            boleto.NossoNumeroFormatado = $"{boleto.Banco.Beneficiario.ContaBancaria.Agencia}{boleto.Banco.Beneficiario.ContaBancaria.DigitoAgencia}/{boleto.Carteira}/00{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
            boleto.ImprimirMensagemInstrucao = true;
            boleto.ImprimirValoresAuxiliares = false;
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var beneficiario = boleto.Banco.Beneficiario;

            return $"{beneficiario.ContaBancaria.Agencia}{beneficiario.ContaBancaria.CarteiraPadrao}{beneficiario.ContaBancaria.OperacaoConta}00{boleto.NossoNumero}{boleto.NossoNumeroDV}";
        }
    }
}
