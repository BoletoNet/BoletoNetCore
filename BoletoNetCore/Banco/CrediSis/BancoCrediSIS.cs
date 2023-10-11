using System;
using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using BoletoNetCore.Extensions;
using static System.String;

namespace BoletoNetCore
{
    internal sealed partial class BancoCrediSIS : BancoFebraban<BancoCrediSIS>, IBanco
    {
        public BancoCrediSIS()
        {
            Codigo = 097;
            Nome = "CrediSIS";
            Digito = "3";
            IdsRetornoCnab400RegistroDetalhe = new List<string> { "1" };
            RemoveAcentosArquivoRemessa = true;
        }

        public void FormataBeneficiario()
        {
            var contaBancaria = Beneficiario.ContaBancaria;

            if (!CarteiraFactory<BancoCrediSIS>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw BoletoNetCoreException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados("PAGÁVEL EM QUALQUER BANCO ATÉ O VENCIMENTO. AÓS SOMENTE NAS AGÊNCIAS CREDISIS", "2 VIA PODE SER TIRADA NO SITE WWW.CREDISIS.COM.BR", "", 8);

            Beneficiario.CodigoFormatado = $"{contaBancaria.Agencia} / {contaBancaria.Conta}{contaBancaria.DigitoConta}";
        }

        public string GerarMensagemRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            return null;
        }
    }
}
