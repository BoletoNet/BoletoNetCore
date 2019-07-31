using System;
using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using static System.String;

namespace BoletoNetCore
{
    internal sealed partial class BancoSantander : BancoFebraban<BancoSantander>, IBanco
    {
        public BancoSantander()
        {
            Codigo = 033;
            Nome = "Santander";
            Digito = "7";
            IdsRetornoCnab400RegistroDetalhe = new List<string> { };
            RemoveAcentosArquivoRemessa = true;
        }

        public void FormataCedente()
        {
            var contaBancaria = Cedente.ContaBancaria;

            if (!CarteiraFactory<BancoSantander>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw BoletoNetCoreException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados("PAGÁVEL PREFERENCIALMENTE NO BANCO SANTANDER.", "", "", digitosConta: 9);

            var codigoCedente = Cedente.Codigo;
            Cedente.Codigo = codigoCedente.Length <= 7 ? codigoCedente.PadLeft(7, '0') : throw BoletoNetCoreException.CodigoCedenteInvalido(codigoCedente, 7);

            Cedente.CodigoFormatado = $"{contaBancaria.Agencia} / {Cedente.Codigo}";
        }

        public override string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            return "";
        }

       



    }
}