using System;
using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using BoletoNetCore.Extensions;

namespace BoletoNetCore
{
    internal sealed partial class BancoBrasil : BancoFebraban<BancoBrasil>, IBanco
    {
        public BancoBrasil()
        {
            Codigo = 1;
            Nome = "Banco do Brasil";
            Digito = "9";
            IdsRetornoCnab400RegistroDetalhe = new List<string> { "7" };
            RemoveAcentosArquivoRemessa = true;
        }

        public override string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            return $"CB{DateTime.Now.Date.Day:00}{DateTime.Now.Date.Month:00}{numeroSequencial.ToString().PadLeft(9, '0').Right(2)}.rem";
        }

        public void FormataBeneficiario()
        {
            var contaBancaria = Beneficiario.ContaBancaria;

            if (!CarteiraFactory<BancoBrasil>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw BoletoNetCoreException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados("PAGÁVEL EM QUALQUER BANCO.", "", "", 8);

            if (Beneficiario.Codigo.Length != 7)
                throw BoletoNetCoreException.CodigoBeneficiarioInvalido(Beneficiario.Codigo, 7);

            Beneficiario.CodigoFormatado = $"{contaBancaria.Agencia}{(string.IsNullOrEmpty(contaBancaria.DigitoAgencia) ? "" : "-" + contaBancaria.DigitoAgencia)} / {contaBancaria.Conta}{(string.IsNullOrEmpty(contaBancaria.DigitoConta) ? "" : "-" + contaBancaria.DigitoConta)}";
        }

        public string GerarMensagemRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            return null;
        }

    }
}