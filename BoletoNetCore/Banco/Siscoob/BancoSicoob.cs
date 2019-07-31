using System;
using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using static System.String;

namespace BoletoNetCore
{
    internal sealed partial class BancoSicoob : BancoFebraban<BancoSicoob> , IBanco
    {
        public BancoSicoob()
        {
            Codigo = 756;
            Nome = "Sicoob";
            Digito = "0";
            IdsRetornoCnab400RegistroDetalhe = new List<string> { "1" };
            RemoveAcentosArquivoRemessa = true;
        }

        public void FormataBeneficiario()
        {
            var contaBancaria = Beneficiario.ContaBancaria;

            if (!CarteiraFactory<BancoSicoob>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw BoletoNetCoreException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            var codigoBeneficiario = Beneficiario.Codigo;
            if (Beneficiario.CodigoDV == Empty)
                throw new Exception($"Dígito do código do beneficiário ({codigoBeneficiario}) não foi informado.");

            contaBancaria.FormatarDados("PAGÁVEL EM QUALQUER BANCO ATÉ A DATA DE VENCIMENTO.", "", "", 8);

            Beneficiario.Codigo = codigoBeneficiario.Length <= 6 ? codigoBeneficiario.PadLeft(6, '0'): throw BoletoNetCoreException.CodigoBeneficiarioInvalido(codigoBeneficiario, 6);

            Beneficiario.CodigoFormatado = $"{contaBancaria.Agencia} / {codigoBeneficiario}-{Beneficiario.CodigoDV}";
        }

        public override string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            return numeroSequencial.ToString();
        }

    }
}
