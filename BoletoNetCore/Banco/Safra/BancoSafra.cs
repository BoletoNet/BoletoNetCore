using System;
using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using static System.String;

namespace BoletoNetCore
{
    internal sealed partial class BancoSafra : BancoFebraban<BancoSafra>, IBanco
    {
        public BancoSafra()
        {
            Codigo = 422;
            Nome = "Safra";
            Digito = "7";
            IdsRetornoCnab400RegistroDetalhe = new List<string> { "1" };
            RemoveAcentosArquivoRemessa = true;
        }

        public void FormataBeneficiario()
        {
            var contaBancaria = Beneficiario.ContaBancaria;

            if (!CarteiraFactory<BancoSafra>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw BoletoNetCoreException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);
            
            contaBancaria.FormatarDados("Pagável em qualquer Banco do Sistema de Compensação", "", "", 6);

            Beneficiario.CodigoFormatado = $"{contaBancaria.Agencia}-{contaBancaria.DigitoAgencia} / {contaBancaria.Conta}-{contaBancaria.DigitoConta}";
        }

        public string GerarMensagemRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            return null;
        }
    }
}


