using System;
using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using BoletoNetCore.Extensions;
using static System.String;

namespace BoletoNetCore
{
    internal sealed partial class BancoItau : BancoFebraban<BancoItau>, IBanco
    {
        public BancoItau()
        {
            Codigo = 341;
            Nome = "Itaú";
            Digito = "7";
            IdsRetornoCnab400RegistroDetalhe = new List<string> { "1" };
            RemoveAcentosArquivoRemessa = true;
        }

        public void FormataBeneficiario()
        {
            var contaBancaria = Beneficiario.ContaBancaria;

            if (!CarteiraFactory<BancoItau>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw BoletoNetCoreException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados("ATÉ O VENCIMENTO EM QUALQUER BANCO. APÓS O VENCIMENTO SOMENTE NO ITAÚ.", "", "", 5);

            Beneficiario.CodigoFormatado = $"{contaBancaria.Agencia} / {contaBancaria.Conta}-{contaBancaria.DigitoConta}";
        }

        public override string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            return $"{DateTime.Now.ToString("ddMMyy")}{numeroSequencial.ToString().PadLeft(9, '0').Right(2)}.rem"; ;
        }

        public string GerarMensagemRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            return null;
        }
    }
}


