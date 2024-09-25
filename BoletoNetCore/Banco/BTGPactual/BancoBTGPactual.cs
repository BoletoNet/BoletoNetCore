using System;
using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using BoletoNetCore.Extensions;

namespace BoletoNetCore.BTGPactual
{
    public class BancoBTGPactual: BancoFebraban<BancoBTGPactual>, IBanco
    {
        public BancoBTGPactual()
        {
            Codigo = 208;
            Nome = "BTG Pactual";
            Digito = "1";
            IdsRetornoCnab400RegistroDetalhe = new List<string> { "1" };
            RemoveAcentosArquivoRemessa = true;
        }
        public void FormataBeneficiario()
        {
            var contaBancaria = Beneficiario.ContaBancaria;
            if (!CarteiraFactory<BancoBTGPactual>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw BoletoNetCoreException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados(
                "EM TODA A REDE BANCÁRIA E SEUS CORRESPONDENTES ATÉ O VALOR LIMITE E DATA DO VENCIMENTO", 
                "", 
                "", 
                8
            );
            Beneficiario.CodigoFormatado = Beneficiario.ContaBancaria.Agencia;
        }

        public string GerarMensagemRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            return "";
        }
    }
}