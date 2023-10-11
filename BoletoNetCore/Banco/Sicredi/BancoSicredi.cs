using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using System;

namespace BoletoNetCore
{
    internal sealed partial class BancoSicredi : BancoFebraban<BancoSicredi>, IBanco
    {
        public BancoSicredi()
        {
            Codigo = 748;
            Nome = "Sicredi";
            Digito = "X";
            IdsRetornoCnab400RegistroDetalhe = new List<string> { "1" };
            RemoveAcentosArquivoRemessa = true;
        }

        public void FormataBeneficiario()
        {
            var contaBancaria = Beneficiario.ContaBancaria;

            if (!CarteiraFactory<BancoSicredi>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw BoletoNetCoreException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados("PAGÁVEL EM QUALQUER BANCO ATÉ O VENCIMENTO.", "", "", 9);

            Beneficiario.CodigoFormatado = $"{contaBancaria.Agencia}.{contaBancaria.OperacaoConta}.{Beneficiario.Codigo}";
        }

        public override string FormatarNomeArquivoRemessa(int sequencial)
        {
            var agora = DateTime.Now;

            var mes = agora.Month.ToString();
            if (mes == "10") mes = "O";
            if (mes == "11") mes = "N";
            if (mes == "12") mes = "D";
            var dia = agora.Day.ToString().PadLeft(2, '0');

            if (sequencial < 0 || sequencial > 10)
                throw BoletoNetCoreException.NumeroSequencialInvalido(sequencial);

            if (sequencial < 1) // se 0 ou 1 é o primeiro arquivo do dia
                return string.Format("{0}{1}{2}.{3}", Beneficiario.Codigo, mes, dia, "CRM");

            //número máximos de arquivos enviados no dia são 10 
            return string.Format("{0}{1}{2}.{3}", Beneficiario.Codigo, mes, dia, $"RM{(sequencial == 10 ? 0 : sequencial)}");

        }

        public string GerarMensagemRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            if (tipoArquivo is TipoArquivo.CNAB400)
            {
                return GerarMensagemRemessa(boleto, ref numeroRegistro);
            }
            else
            {
                return null;
            }
        }
    }
}