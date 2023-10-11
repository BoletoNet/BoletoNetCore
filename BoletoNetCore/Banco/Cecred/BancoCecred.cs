using BoletoNetCore.Exceptions;
using System;
using System.Collections.Generic;

namespace BoletoNetCore
{
    internal sealed partial class BancoCecred : BancoFebraban<BancoCecred>, IBanco
    {
        public BancoCecred()
        {
            Codigo = 85;
            Nome = "VIACREDI";
            Digito = "0";
            IdsRetornoCnab400RegistroDetalhe = new List<string> { "1" };
            RemoveAcentosArquivoRemessa = true;
        }

        public override string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            return $"CB{DateTime.Now.Date.Day:00}{DateTime.Now.Date.Month:00}{numeroSequencial.ToString().PadLeft(9, '0').PadRight(2)}.rem";
        }

        public void FormataBeneficiario()
        {
            ContaBancaria contaBancaria = Beneficiario.ContaBancaria;
            if (!CarteiraFactory<BancoCecred>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw BoletoNetCoreException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);
            contaBancaria.FormatarDados("Pagar preferencialmente nas cooperativas do Sistema Ailos.", "", "", 7);
            Beneficiario.CodigoFormatado = contaBancaria.Agencia + "-" + contaBancaria.DigitoAgencia + "    " + Beneficiario.ContaBancaria.Conta + "-" + Beneficiario.ContaBancaria.DigitoConta;
        }

        public string GerarMensagemRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            return null;
        }
    }
}
