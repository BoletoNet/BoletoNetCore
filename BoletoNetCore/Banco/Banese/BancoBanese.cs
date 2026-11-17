using System;
using BoletoNetCore.Exceptions;

namespace BoletoNetCore
{
    internal sealed partial class BancoBanese : BancoFebraban<BancoBanese>, IBanco
    {
        public BancoBanese()
        {
            Codigo = 47;
            Nome = "Banese";
            Digito = "7";
            RemoveAcentosArquivoRemessa = true;
        }

        public void FormataBeneficiario()
        {
            var contaBancaria = Beneficiario.ContaBancaria;

            if (!CarteiraFactory<BancoBanese>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw BoletoNetCoreException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados("PAGÁVEL PREFERENCIALMENTE NA REDE BANESE", "", "", 9);

            var digitoConta = string.IsNullOrWhiteSpace(contaBancaria.DigitoConta) ? "" : "-" + contaBancaria.DigitoConta;
            Beneficiario.CodigoFormatado = $"{contaBancaria.Agencia} / {contaBancaria.Conta}{digitoConta}";
        }

        public override string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            return string.Empty;
        }

        public string GerarMensagemRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            return null;
        }
    }
}
