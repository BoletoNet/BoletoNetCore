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

        ///
        /// Como o retorno CNAB400 não tem todos os dados na linha do header
        /// Utiliza o primeiro registro de titulo para pegar mais dados do beneficiario
        ///
        public override void CompletarHeaderRetornoCNAB400(string registro)
        {
            //021 a 037 - Identifica��es da Empresa Benefici�ria no Banco
            //Dever� ser preenchido(esquerda para direita), da seguinte maneira:
            //21 a 21 - Zero
            //22 a 24 - c�digos da carteira
            //25 a 29 - c�digos da Ag�ncia Benefici�rios, sem o d�gito.
            //30 a 36 - Contas Corrente
            //37 a 37 - d�gitos da Conta

            this.Beneficiario.ContaBancaria = new ContaBancaria();
            this.Beneficiario.ContaBancaria.Agencia = registro.Substring(24, 5);

            var conta = registro.Substring(29, 8).Trim();
            this.Beneficiario.ContaBancaria.Conta = conta.Substring(0, 7);
            this.Beneficiario.ContaBancaria.DigitoConta = conta.Substring(7, 1);

            // 01 - cpf / 02 - cnpj
            if (registro.Substring(1, 2) == "01")
                this.Beneficiario.CPFCNPJ = registro.Substring(6, 11);
            else
                this.Beneficiario.CPFCNPJ = registro.Substring(3, 14);
        }

        private string DescricaoOcorrenciaCnab400(string codigo)
        {
            switch (codigo)
            {
                case "02":
                    return "Entrada Confirmada";
                case "03":
                    return "Entrada Rejeitada";
                case "06":
                    return "Liquidação normal";
                case "09":
                    return "Baixado Automaticamente via Arquivo";
                case "10":
                    return "Baixado conforme instruções da Agência";
                case "11":
                    return "Em Ser - Arquivo de Títulos pendentes";
                case "12":
                    return "Abatimento Concedido";
                case "13":
                    return "Abatimento Cancelado";
                case "14":
                    return "Vencimento Alterado";
                case "15":
                    return "Liquidação em Cartório";
                case "16":
                    return "Título Pago em Cheque � Vinculado";
                case "17":
                    return "Liquidação após baixa ou Título não registrado";
                case "18":
                    return "Acerto de Depositária";
                case "19":
                    return "Confirmação Recebimento Instrução de Protesto";
                case "20":
                    return "Confirmação Recebimento Instrução Sustação de Protesto";
                case "21":
                    return "Acerto do Controle do Participante";
                case "23":
                    return "Entrada do Título em Cartório";
                case "24":
                    return "Entrada rejeitada por CEP Irregular";
                case "27":
                    return "Baixa Rejeitada";
                case "28":
                    return "Débito de tarifas/custas";
                case "30":
                    return "Alteração de Outros Dados Rejeitados";
                case "32":
                    return "Instruçõo Rejeitada";
                case "33":
                    return "Confirmação Pedido Alteração Outros Dados";
                case "34":
                    return "Retirado de Cartório e Manutenção Carteira";
                case "35":
                    return "Desagendamento ) débito automático";
                case "68":
                    return "Acerto dos dados ) rateio de Crédito";
                case "69":
                    return "Cancelamento dos dados ) rateio";
                default:
                    return "";
            }
        }
    }
}