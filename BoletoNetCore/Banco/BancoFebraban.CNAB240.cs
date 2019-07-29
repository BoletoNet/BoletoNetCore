using BoletoNetCore.Exceptions;
using System;
using System.Collections.Generic;

namespace BoletoNetCore
{
    partial class BancoFebraban<T> 
    {
        public virtual void LerHeaderRetornoCNAB240(ArquivoRetorno arquivoRetorno, string registro)
        {
            //Manual de Procedimentos Nº 4008.524.0339 - Versão 04 - Elaborado em: 12 / 08 / 2015
            arquivoRetorno.Banco.Cedente = new Cedente();
            //05.0 Tipo de inscrição da empresa 18 - 18 (1)
            //06.0 Número de incrição da empresa 19 - 32 (14)
            arquivoRetorno.Banco.Cedente.CPFCNPJ = registro.Substring(17, 1) == "1" ? registro.Substring(21, 11) : registro.Substring(18, 14);
            //07.0 Código do convênio no banco 33 - 52 (20)
            arquivoRetorno.Banco.Cedente.Codigo = registro.Substring(32, 20).Trim();
            //13.0 Nome da Empresa 73 - 102 (30)
            arquivoRetorno.Banco.Cedente.Nome = registro.Substring(72, 30).Trim();

            ////14.0 Nome do Banco 103 - 132 (30)
            //arquivoRetorno.Banco.Nome = registro.Substring(102, 30);

            arquivoRetorno.Banco.Cedente.ContaBancaria = new ContaBancaria();
            //08.0 Agência mantenedora da conta 53 - 57 (5)
            arquivoRetorno.Banco.Cedente.ContaBancaria.Agencia = registro.Substring(52, 5);
            //09.0 Dígito verificador da agência 58 - 58 (1)
            arquivoRetorno.Banco.Cedente.ContaBancaria.DigitoAgencia = registro.Substring(57, 1);
            //10.0 Número da conta corrente 59 - 70 (12)
            arquivoRetorno.Banco.Cedente.ContaBancaria.Conta = registro.Substring(58, 12);
            //11.0 Dígito verificador da conta 71 - 71 (1)
            arquivoRetorno.Banco.Cedente.ContaBancaria.DigitoConta = registro.Substring(70, 1);

            //17.0 Data de geração do arquivo 144 - 151 (8)
            arquivoRetorno.DataGeracao = Utils.ToDateTime(Utils.ToInt32(registro.Substring(143, 8)).ToString("##-##-####"));
            //19.0 Número seqüencial do arquivo NSA 158 - 163 (6)
            arquivoRetorno.NumeroSequencial = Utils.ToInt32(registro.Substring(157, 6));
        }

        public virtual void LerDetalheRetornoCNAB240SegmentoT(ref Boleto boleto, string registro)
        {
            try
            {
                //Nº Controle do Participante
                boleto.NumeroControleParticipante = registro.Substring(105, 25);

                //Carteira
                boleto.Carteira = registro.Substring(57, 1);
                switch (boleto.Carteira)
                {
                    case "1":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;
                        break;
                    case "2":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaVinculada;
                        break;
                    case "3":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaCaucionada;
                        break;
                    case "4":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaDescontada;
                        break;
                    case "5":
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaVendor;
                        break;
                    default:
                        boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;
                        break;
                }

                //Identificação do Título no Banco
                string tmp = registro.Substring(37, 20);
                boleto.NossoNumero = tmp.Substring(8, 11);
                boleto.NossoNumeroDV = tmp.Substring(19, 1);
                boleto.NossoNumeroFormatado = boleto.NossoNumero;

                //Identificação de Ocorrência
                boleto.CodigoMovimentoRetorno = registro.Substring(15, 2);
                boleto.DescricaoMovimentoRetorno = Cnab.MovimentoRetornoCnab240(boleto.CodigoMovimentoRetorno);
                boleto.CodigoMotivoOcorrencia = registro.Substring(213, 10);
                boleto.ListMotivosOcorrencia = Cnab.MotivoOcorrenciaCnab240(boleto.CodigoMotivoOcorrencia, boleto.CodigoMovimentoRetorno);

                //Número do Documento
                boleto.NumeroDocumento = registro.Substring(58, 15);
                boleto.EspecieDocumento = TipoEspecieDocumento.NaoDefinido;

                //Valor do Título
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(81, 15)) / 100;
                boleto.ValorTarifas = Convert.ToDecimal(registro.Substring(198, 15)) / 100;

                //Data Vencimento do Título
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(73, 8)).ToString("##-##-####"));

                //Dados Sacado
                boleto.Sacado = new Sacado();
                string str = registro.Substring(133, 15);
                boleto.Sacado.CPFCNPJ = str.Substring(str.Length - 14, 14);
                boleto.Sacado.Nome = registro.Substring(148, 40);

                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;

                //////
                //18.3T 97 99 3 - Num Banco Cobr./Receb.
                boleto.BancoCobradorRecebedor = registro.Substring(96, 3);
                //19.3T 100 104 5 - Num	Ag. Cobradora
                boleto.AgenciaCobradoraRecebedora = registro.Substring(99, 6);

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 240 / T.", ex);
            }

        }

        public virtual void LerDetalheRetornoCNAB240SegmentoU(ref Boleto boleto, string registro)
        {
            try
            {
                //Valor do Título
                boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(17, 15)) / 100;
                boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(32, 15)) / 100;
                boleto.ValorAbatimento = Convert.ToDecimal(registro.Substring(47, 15)) / 100;
                boleto.ValorIOF = Convert.ToDecimal(registro.Substring(62, 15)) / 100;
                boleto.ValorPago = Convert.ToDecimal(registro.Substring(77, 15)) / 100;
                boleto.ValorPagoCredito = Convert.ToDecimal(registro.Substring(92, 15)) / 100;
                boleto.ValorOutrasDespesas = Convert.ToDecimal(registro.Substring(107, 15)) / 100;
                boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(122, 15)) / 100;


                //Data Ocorrência no Banco
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(137, 8)).ToString("##-##-####"));

                // Data do Crédito
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(145, 8)).ToString("##-##-####"));

                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 240 / U.", ex);
            }
        }

    }
}