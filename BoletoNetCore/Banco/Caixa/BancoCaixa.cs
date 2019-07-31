using System;
using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using BoletoNetCore.Extensions;
using static System.String;

namespace BoletoNetCore
{
    internal sealed partial class BancoCaixa : BancoFebraban<BancoCaixa>, IBanco
    {
        public BancoCaixa()
        {
            Codigo = 104;
            Nome = "Caixa Econômica Federal";
            Digito = "0";
            IdsRetornoCnab400RegistroDetalhe = new List<string> { "1" };
            RemoveAcentosArquivoRemessa = true;
        }

        public void FormataBeneficiario()
        {
            var contaBancaria = Beneficiario.ContaBancaria;
            if (!CarteiraFactory<BancoCaixa>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw BoletoNetCoreException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados("PREFERENCIALMENTE NAS CASAS LOTERICAS ATE O VALOR LIMITE", "", "SAC CAIXA: 0800 726 0101 (informações, reclamações, sugestões e elogios)<br>Para pessoas com deficiência auditiva ou de fala: 0800 726 2492<br>Ouvidoria: 0800 725 7474<br>caixa.gov.br<br>", 6);

            var codigoBeneficiario = Beneficiario.Codigo;
            Beneficiario.Codigo = codigoBeneficiario.Length <= 6 ? codigoBeneficiario.PadLeft(6, '0') : throw BoletoNetCoreException.CodigoBeneficiarioInvalido(codigoBeneficiario, 6);

            if (Beneficiario.CodigoDV == Empty)
                throw new Exception($"Dígito do código do beneficiário ({codigoBeneficiario}) não foi informado.");

            Beneficiario.CodigoFormatado = $"{contaBancaria.Agencia} / {codigoBeneficiario}-{Beneficiario.CodigoDV}";
        }

        public override string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            return numeroSequencial.ToString();
        }


    }
}