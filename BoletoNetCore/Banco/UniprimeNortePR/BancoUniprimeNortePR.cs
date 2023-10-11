using System;
using System.Collections.Generic;
using BoletoNetCore.Extensions;

namespace BoletoNetCore
{
    internal sealed partial class BancoUniprimeNortePR : BancoFebraban<BancoUniprimeNortePR>, IBanco
    {
        public BancoUniprimeNortePR()
        {
            Codigo = 084;
            Nome = "Uniprime";
            Digito = "1";
            IdsRetornoCnab400RegistroDetalhe = new List<string> { "1" };
            RemoveAcentosArquivoRemessa = true;
        }

        public void FormataBeneficiario()
        {
            var contaBancaria = this.Beneficiario.ContaBancaria;
            contaBancaria.FormatarDados("PAGÁVEL EM QUALQUER BANCO ATÉ O VENCIMENTO.", "", "", 7);
            this.Beneficiario.CodigoFormatado = $"{contaBancaria.Agencia}-{contaBancaria.DigitoAgencia}/{contaBancaria.Conta}-{contaBancaria.DigitoConta}";
        }

        public override string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            return $"cb{DateTime.Now.ToString("ddMM")}{numeroSequencial.ToString("00")}.rem"; ;
        }

        public string GerarMensagemRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
                return null;       
        }
    }
}