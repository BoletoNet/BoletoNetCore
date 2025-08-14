using BoletoNetCore.Exceptions;
using BoletoNetCore.Extensions;
using System;
using System.Collections.Generic;

namespace BoletoNetCore
{
    internal sealed partial class BancoDaycoval : BancoFebraban<BancoDaycoval>, IBanco
    {
        public BancoDaycoval()
        {
            Codigo = 707;
            Nome = "Banco Daycoval";
            Digito = "2";
            IdsRetornoCnab400RegistroDetalhe = new List<string> { "1" };
            RemoveAcentosArquivoRemessa = true;
        }

        public void FormataBeneficiario()
        {
            var contaBancaria = Beneficiario.ContaBancaria;

            if (!CarteiraFactory<BancoDaycoval>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw BoletoNetCoreException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados("PAGAVEL EM QUALQUER AGÊNCIA BANCÁRIA, MESMO APÓS VENCIMENTO", "", "", 7);

            Beneficiario.CodigoFormatado = $"{contaBancaria.Agencia}{(string.IsNullOrEmpty(contaBancaria.DigitoAgencia) ? "" : "-" + contaBancaria.DigitoAgencia)} / {contaBancaria.Conta}{(string.IsNullOrEmpty(contaBancaria.DigitoConta) ? "" : "-" + contaBancaria.DigitoConta)}";
        }

        public override string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            return numeroSequencial.ToString();
        }

        public string GerarMensagemRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            return null;
        }

    }
}