using System;
using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using BoletoNetCore.Extensions;

namespace BoletoNetCore
{
    internal sealed partial class BancoInter : BancoFebraban<BancoInter>, IBanco
    {
        public BancoInter()
        {
            Codigo = 77;
            Nome = "Inter";
            Digito = "9";
            IdsRetornoCnab400RegistroDetalhe = new List<string> { "1" };
            RemoveAcentosArquivoRemessa = true;
        }

        public void FormataBeneficiario()
        {
            var contaBancaria = Beneficiario.ContaBancaria;

            contaBancaria.FormatarDados("PAGÁVEL EM QUALQUER BANCO ATÉ O VENCIMENTO.", "", "", 10);

            Beneficiario.CodigoFormatado = $"{contaBancaria.Agencia}{contaBancaria.DigitoAgencia}/{Beneficiario.Codigo}{Beneficiario.CodigoDV}";

        }

        public override string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            return $"CI400_001_{numeroSequencial.ToString("D7")}.REM";
        }

        public string GerarMensagemRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            return null;
        }
    }
}