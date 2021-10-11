using System;
using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using BoletoNetCore.Extensions;
using Microsoft.VisualBasic;

namespace BoletoNetCore
{
    public static class Banco
    {
        private static readonly Dictionary<int, Lazy<IBanco>> Bancos = new Dictionary<int, Lazy<IBanco>>
        {
            [001] = BancoBrasil.Instance,
            [004] = BancoNordeste.Instance,
            [033] = BancoSantander.Instance,
            [041] = BancoBanrisul.Instance,
            [085] = BancoCecred.Instance,
            [104] = BancoCaixa.Instance,
            [237] = BancoBradesco.Instance,
            [341] = BancoItau.Instance,
            [422] = BancoSafra.Instance,
            [748] = BancoSicredi.Instance,
            [756] = BancoSicoob.Instance
        };

        public static IBanco Instancia(int codigoBanco)
            => (Bancos.ContainsKey(codigoBanco) ? Bancos[codigoBanco] : throw BoletoNetCoreException.BancoNaoImplementado(codigoBanco)).Value;

        public static IBanco Instancia(Bancos codigoBanco)
            => Instancia((int)codigoBanco);

        /// <summary>
        ///     Formata c�digo de barras
        ///     O c�digo de barra para cobran�a cont�m 44 posi��es dispostas da seguinte forma:
        ///     01 a 03 - 3 - Identifica��o  do  Banco
        ///     04 a 04 - 1 - C�digo da Moeda
        ///     05 a 05 � 1 - D�gito verificador do C�digo de Barras
        ///     06 a 09 - 4 - Fator de vencimento
        ///     10 a 19 - 10 - Valor
        ///     20 a 44 � 25 - Campo Livre
        /// </summary>
        public static void FormataCodigoBarra(Boleto boleto)
        {
            var banco = boleto.Banco;
            var codigoBarra = boleto.CodigoBarra;
            codigoBarra.CampoLivre = banco.FormataCodigoBarraCampoLivre(boleto);

            if (codigoBarra.CampoLivre.Length != 25)
                throw new Exception($"Campo Livre ({codigoBarra.CampoLivre}) deve conter 25 d�gitos.");

            // Formata C�digo de Barras do Boleto
            codigoBarra.CodigoBanco = banco.Codigo.ToString().FitStringLength(3, '0');
            codigoBarra.Moeda = boleto.CodigoMoeda;
            codigoBarra.FatorVencimento = boleto.DataVencimento.FatorVencimento();
            codigoBarra.ValorDocumento = boleto.ValorTitulo.ToString("N2").Replace(",", "").Replace(".", "").PadLeft(10, '0');
        }

        /// <summary>
        /// Formata Mensagens de Juros e Multa e Desconto nas instru��es do Caixa
        /// </summary>
        /// <param name="boleto"></param>
        public static void FormataMensagemInstrucao(Boleto boleto)
        {
            boleto.MensagemInstrucoesCaixaFormatado = "";

            //JUROS
            if (boleto.ImprimirValoresAuxiliares == true && boleto.ValorJurosDia > 0)
            {
                boleto.MensagemInstrucoesCaixaFormatado += $"Cobrar juros de R$ {boleto.ValorJurosDia.ToString("N2")} por dia de atraso AP�S {boleto.DataJuros.ToString("dd/MM/yyyy")}{Environment.NewLine}";
            }
            else if (boleto.ImprimirValoresAuxiliares == true && boleto.PercentualJurosDia > 0)
            {
                boleto.MensagemInstrucoesCaixaFormatado += $"Cobrar juros de {boleto.PercentualJurosDia.ToString("N2")}% por dia de atraso AP�S {boleto.DataJuros.ToString("dd/MM/yyyy")}{Environment.NewLine}";
            }

            //MULTA
            if (boleto.ImprimirValoresAuxiliares == true && boleto.ValorMulta > 0)
            {
                boleto.MensagemInstrucoesCaixaFormatado += $"Cobrar multa de R$ {boleto.ValorMulta.ToString("N2")} a partir DE {boleto.DataMulta.ToString("dd/MM/yyyy")}{Environment.NewLine}";
            }
            else if (boleto.ImprimirValoresAuxiliares == true && boleto.PercentualMulta > 0)
            {
                boleto.MensagemInstrucoesCaixaFormatado += $"Cobrar multa de {boleto.PercentualMulta.ToString("N2")}% a partir DE {boleto.DataMulta.ToString("dd/MM/yyyy")}{Environment.NewLine}";
            }

            //DESCONTO
            if (boleto.ImprimirValoresAuxiliares == true && boleto.ValorDesconto > 0)
            {
                boleto.MensagemInstrucoesCaixaFormatado += $"Conceder desconto de R$ {boleto.ValorDesconto.ToString("N2")} AT� {boleto.DataDesconto.ToString("dd/MM/yyyy")}{Environment.NewLine}";
            }

            //Aqui, define se a mensagem de instru��o manual deve ser impressa, 
            //na minha vis�o se o usu�rio passou uma instru��o, esta deveria ser impressa sempre.
            //Entretanto, para manter o comportamento atual sem quebrar nenhuma aplica��o, foi criado um par�metro com valor "false"
            //https://github.com/BoletoNet/BoletoNetCore/pull/91
            if (boleto.ImprimirMensagemInstrucao && boleto.MensagemInstrucoesCaixa?.Length > 0)
            {
                boleto.MensagemInstrucoesCaixaFormatado += Environment.NewLine;
                boleto.MensagemInstrucoesCaixaFormatado += boleto.MensagemInstrucoesCaixa;
            }

        }

        /// <summary>
        ///     A linha digit�vel ser� composta por cinco campos:
        ///     1� campo
        ///     composto pelo c�digo de Banco, c�digo da moeda, as cinco primeiras posi��es do campo
        ///     livre e o d�gito verificador deste campo;
        ///     2� campo
        ///     composto pelas posi��es 6� a 15� do campo livre e o d�gito verificador deste campo;
        ///     3� campo
        ///     composto pelas posi��es 16� a 25� do campo livre e o d�gito verificador deste campo;
        ///     4� campo
        ///     composto pelo d�gito verificador do c�digo de barras, ou seja, a 5� posi��o do c�digo de
        ///     barras;
        ///     5� campo
        ///     Composto pelo fator de vencimento com 4(quatro) caracteres e o valor do documento com 10(dez) caracteres, sem
        ///     separadores e sem edi��o.
        /// </summary>
        public static void FormataLinhaDigitavel(Boleto boleto)
        {
            var codigoBarra = boleto.CodigoBarra;
            if (string.IsNullOrWhiteSpace(codigoBarra.CampoLivre))
            {
                codigoBarra.LinhaDigitavel = "";
                return;
            }
            //BBBMC.CCCCD1 CCCCC.CCCCCD2 CCCCC.CCCCCD3 D4 FFFFVVVVVVVVVV

            var codigoDeBarras = codigoBarra.CodigoDeBarras;

            #region Campo 1

            // POSI��O 1 A 3 DO CODIGO DE BARRAS
            var bbb = codigoDeBarras.Substring(0, 3);
            // POSI��O 4 DO CODIGO DE BARRAS
            var m = codigoDeBarras.Substring(3, 1);
            // POSI��O 20 A 24 DO CODIGO DE BARRAS
            var ccccc = codigoDeBarras.Substring(19, 5);
            // Calculo do D�gito
            var d1 = CalcularDvModulo10(bbb + m + ccccc);
            // Formata Grupo 1
            var grupo1 = $"{bbb}{m}{ccccc.Substring(0, 1)}.{ccccc.Substring(1, 4)}{d1} ";

            #endregion Campo 1

            #region Campo 2

            //POSI��O 25 A 34 DO COD DE BARRAS
            var d2A = codigoDeBarras.Substring(24, 10);
            // Calculo do D�gito
            var d2B = CalcularDvModulo10(d2A).ToString();
            // Formata Grupo 2
            var grupo2 = $"{d2A.Substring(0, 5)}.{d2A.Substring(5, 5)}{d2B} ";

            #endregion Campo 2

            #region Campo 3

            //POSI��O 35 A 44 DO CODIGO DE BARRAS
            var d3A = codigoDeBarras.Substring(34, 10);
            // Calculo do D�gito
            var d3B = CalcularDvModulo10(d3A).ToString();
            // Formata Grupo 3
            var grupo3 = $"{d3A.Substring(0, 5)}.{d3A.Substring(5, 5)}{d3B} ";

            #endregion Campo 3

            #region Campo 4

            // D�gito Verificador do C�digo de Barras
            var grupo4 = $"{codigoBarra.DigitoVerificador} ";

            #endregion Campo 4

            #region Campo 5

            //POSICAO 6 A 9 DO CODIGO DE BARRAS
            var d5A = codigoDeBarras.Substring(5, 4);
            //POSICAO 10 A 19 DO CODIGO DE BARRAS
            var d5B = codigoDeBarras.Substring(9, 10);
            // Formata Grupo 5
            var grupo5 = $"{d5A}{d5B}";

            #endregion Campo 5

            codigoBarra.LinhaDigitavel = $"{grupo1}{grupo2}{grupo3}{grupo4}{grupo5}";
        }

        private static int CalcularDvModulo10(string texto)
        {
            int soma = 0, peso = 2;
            for (var i = texto.Length; i > 0; i--)
            {
                var resto = Convert.ToInt32(texto.MidVB(i, 1)) * peso;
                if (resto > 9)
                    resto = resto / 10 + resto % 10;
                soma += resto;
                if (peso == 2)
                    peso = 1;
                else
                    peso = peso + 1;
            }
            var digito = (10 - soma % 10) % 10;
            return digito;
        }
    }
}