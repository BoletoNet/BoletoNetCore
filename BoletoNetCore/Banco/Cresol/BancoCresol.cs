using BoletoNetCore.Exceptions;
using System.Collections.Generic;
using System;
using BoletoNetCore.Extensions;

namespace BoletoNetCore
{
    internal sealed partial class BancoCresol : BancoFebraban<BancoCresol>, IBanco
    {
        public BancoCresol()
        {
            Codigo = 133;
            Nome = "Cresol";
            Digito = "3";
            //IdsRetornoCnab400RegistroDetalhe = new List<string> { "1", "4" };
            RemoveAcentosArquivoRemessa = true;
        }

        public override string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            return $"CB{DateTime.Now.Date.Day:00}{DateTime.Now.Date.Month:00}{numeroSequencial.ToString().PadLeft(9, '0').Right(2)}.rem";
        }

        public void FormataBeneficiario()
        {
            var contaBancaria = Beneficiario.ContaBancaria;

            if (!CarteiraFactory<BancoCresol>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw BoletoNetCoreException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados("PAGÁVEL PREFERENCIALMENTE NA REDE BRADESCO OU BRADESCO EXPRESSO.", "", "", 7);

            var codigoBeneficiario = Beneficiario.Codigo;
            Beneficiario.Codigo = codigoBeneficiario.Length <= 20 ? codigoBeneficiario.PadLeft(20, '0') : throw BoletoNetCoreException.CodigoBeneficiarioInvalido(codigoBeneficiario, 20);

            Beneficiario.CodigoFormatado = $"{contaBancaria.Agencia}-{contaBancaria.DigitoAgencia} / {contaBancaria.Conta}-{contaBancaria.DigitoConta}";
        }

        public string GerarMensagemRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            return null;
        }
    }
}
