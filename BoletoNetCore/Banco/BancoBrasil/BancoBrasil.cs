using System;
using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using BoletoNetCore.Extensions;

namespace BoletoNetCore
{
    internal sealed partial class BancoBrasil : BancoFebraban<BancoBrasil> , IBanco
    {
        public BancoBrasil()
        {
            Codigo = 1;
            Nome = "Banco do Brasil";
            Digito = "9";
            IdsRetornoCnab400RegistroDetalhe = new List<string> { "7" };
            RemoveAcentosArquivoRemessa = true;
        }

        public void FormataBeneficiario()
        {
            var contaBancaria = Beneficiario.ContaBancaria;

            if (!CarteiraFactory<BancoBrasil>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw BoletoNetCoreException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados("PAGÁVEL EM QUALQUER BANCO.", "", "", 8);

            if (Beneficiario.Codigo.Length != 7)
                throw BoletoNetCoreException.CodigoBeneficiarioInvalido(Beneficiario.Codigo, 7);

            Beneficiario.CodigoFormatado = $"{contaBancaria.Agencia}-{contaBancaria.DigitoAgencia} / {contaBancaria.Conta}-{contaBancaria.DigitoConta}";
        }

    }
}