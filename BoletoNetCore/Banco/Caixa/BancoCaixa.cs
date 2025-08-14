using BoletoNetCore.Exceptions;
using BoletoNetCore.Extensions;
using System;
using System.Collections.Generic;

namespace BoletoNetCore
{
    internal sealed partial class BancoCaixa : BancoFebraban<BancoCaixa>, IBanco
    {
        public BancoCaixa()
        {
            Codigo = 104;
            Nome = "Caixa Econ�mica Federal";
            Digito = "0";
            IdsRetornoCnab400RegistroDetalhe = new List<string> { "1" };
            RemoveAcentosArquivoRemessa = true;
        }

        public void FormataBeneficiario()
        {
            var contaBancaria = Beneficiario.ContaBancaria;
            if (!CarteiraFactory<BancoCaixa>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw BoletoNetCoreException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados("EM TODA A REDE BANC�RIA E SEUS CORRESPONDENTES AT� O VALOR LIMITE", "", "SAC CAIXA: 0800 726 0101 (informa��es, reclama��es, sugest�es e elogios)<br>Para pessoas com defici�ncia auditiva ou de fala: 0800 726 2492<br>Ouvidoria: 0800 725 7474<br>caixa.gov.br<br>", 6);

            var codigoBeneficiario = Beneficiario.Codigo;
            if (codigoBeneficiario.Length <= 6)
            {
                Beneficiario.Codigo = codigoBeneficiario.PadLeft(6, '0');

                if (String.IsNullOrEmpty(Beneficiario.CodigoDV))
                    throw new Exception($"D�gito do c�digo do benefici�rio ({codigoBeneficiario}) n�o foi informado.");

                Beneficiario.CodigoFormatado = $"{contaBancaria.Agencia} / {codigoBeneficiario}-{Beneficiario.CodigoDV}";
            }
            else if (codigoBeneficiario.Length == 7)
            {
                Beneficiario.Codigo = codigoBeneficiario;

                if (!String.IsNullOrEmpty(Beneficiario.CodigoDV))
                    throw new Exception($"D�gito do c�digo do benefici�rio ({codigoBeneficiario}) n�o deve ser informado quando codigo beneficiario tiver 7 d�gitos.");

                Beneficiario.CodigoFormatado = $"{contaBancaria.Agencia} / {codigoBeneficiario}-{codigoBeneficiario.CalcularDVCaixa()}";
            }
            else
            {
                throw BoletoNetCoreException.CodigoBeneficiarioInvalido(codigoBeneficiario, "6 ou 7");
            }
        }

        public override string FormatarNomeArquivoRemessa(int numeroSequencial)
        {
            return numeroSequencial.ToString();
        }

        public string GerarMensagemRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            return null;
        }
    }
}