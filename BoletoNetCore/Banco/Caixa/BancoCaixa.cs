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

        public void FormataCedente()
        {
            var contaBancaria = Cedente.ContaBancaria;
            if (!CarteiraFactory<BancoCaixa>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw BoletoNetCoreException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados("PREFERENCIALMENTE NAS CASAS LOTERICAS ATE O VALOR LIMITE", "", "SAC CAIXA: 0800 726 0101 (informações, reclamações, sugestões e elogios)<br>Para pessoas com deficiência auditiva ou de fala: 0800 726 2492<br>Ouvidoria: 0800 725 7474<br>caixa.gov.br<br>", 6);

            var codigoCedente = Cedente.Codigo;
            Cedente.Codigo = codigoCedente.Length <= 6 ? codigoCedente.PadLeft(6, '0') : throw BoletoNetCoreException.CodigoCedenteInvalido(codigoCedente, 6);

            if (Cedente.CodigoDV == Empty)
                throw new Exception($"Dígito do código do cedente ({codigoCedente}) não foi informado.");

            Cedente.CodigoFormatado = $"{contaBancaria.Agencia} / {codigoCedente}-{Cedente.CodigoDV}";
        }

        public override string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            return numeroSequencial.ToString();
        }


    }
}