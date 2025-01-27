using System;
using static System.String;

namespace BoletoNetCore
{
    partial class BancoDaycoval : IBancoCNAB400
    {
        #region Remessa

        public string GerarHeaderRemessaCNAB400(ref int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0003, 007, 0, "REMESSA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0010, 002, 0, "01", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0012, 015, 0, "COBRANCA", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0027, 020, 0, Beneficiario.ContaBancaria.CodigoConvenio, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 030, 0, Beneficiario.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0077, 003, 0, "707", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0080, 015, 0, "BANCO DAYCOVAL", ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0095, 006, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0101, 294, 0, Empty, ' ');

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');

                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB400.", ex);
            }
        }
        public string GerarDetalheRemessaCNAB400(Boleto boleto, ref int numeroRegistroGeral)
        {
            var detalhe = GerarRegistroTransacaoRemessaCNAB400(boleto, ref numeroRegistroGeral);

            // Se tiver NFe, gera o registro da Tipo 4 Conforme Manual
            if (boleto.NFe.Numero != null)
            {
                var strline = GerarNFeRemessaCNAB400(boleto, ref numeroRegistroGeral);
                if (!IsNullOrWhiteSpace(strline))
                {
                    detalhe += Environment.NewLine;
                    detalhe += strline;
                }
            }
            return detalhe;


        }
        public string GerarRegistroTransacaoRemessaCNAB400(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 002, 0, boleto.Banco.Beneficiario.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 014, 0, boleto.Banco.Beneficiario.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0018, 020, 0, boleto.Banco.Beneficiario.ContaBancaria.CodigoConvenio, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0038, 025, 0, Empty, ' ');

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0063, 008, 0, boleto.NossoNumero, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0071, 013, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0084, 024, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0108, 001, 0, "6", '0');

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, boleto.CodigoMovimentoRetorno, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0111, 010, 0, $"{boleto.NumeroDocumento}{boleto.ParcelaInformativo}", ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0121, 006, 0, boleto.DataVencimento, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 013, 2, boleto.ValorTitulo, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 003, 0, "707", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0143, 005, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0148, 002, 0, 1, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0150, 001, 0, boleto.Aceite, ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0151, 006, 0, boleto.DataEmissao, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0157, 002, 0, "00", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0159, 002, 0, "00", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0161, 013, 2, boleto.ValorJurosDia, '0');

                if (boleto.ValorDesconto == 0)
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0174, 006, 0, "0", '0'); // Sem Desconto
                else
                    reg.Adicionar(TTiposDadoEDI.ediDataDDMMAA___________, 0174, 006, 0, boleto.DataDesconto, '0'); // Com Desconto

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0180, 013, 2, boleto.ValorDesconto, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0193, 013, 2, boleto.ValorIOF, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0206, 013, 2, boleto.ValorAbatimento, '0');

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0219, 002, 0, boleto.Pagador.TipoCPFCNPJ("00"), '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 014, 0, boleto.Pagador.CPFCNPJ, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0235, 030, 0, boleto.Pagador.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0265, 010, 0, Empty, ' ');

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0275, 040, 0, boleto.Pagador.Endereco.FormataLogradouro(40), ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0315, 012, 0, boleto.Pagador.Endereco.Bairro, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0327, 008, 0, boleto.Pagador.Endereco.CEP.Replace("-", ""), '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0335, 015, 0, boleto.Pagador.Endereco.Cidade, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0350, 002, 0, boleto.Pagador.Endereco.UF, ' ');

                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0352, 030, 0, Beneficiario.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0382, 004, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0386, 006, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0392, 002, 0, boleto.DiasLimiteRecebimento.HasValue ? boleto.DiasLimiteRecebimento.Value.ToString("00") : "00", '0'); // Caso n�o for informado, ir� definir o m�ximo de dias "99".
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0394, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');

                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400 - Registro 1.", ex);
            }
        }

        public string GerarNFeRemessaCNAB400(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "4", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0002, 015, 0, boleto.NFe.Numero, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0017, 013, 2, boleto.NFe.Valor, '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0030, 008, 0, boleto.NFe.DataEmissao, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0038, 044, 0, boleto.NFe.ChaveAcesso, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0082, 313, 0, Empty, ' ');

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a gera��o do registro da NFe do arquivo de REMESSA.", ex);
            }
        }


        public string GerarTrailerRemessaCNAB400(int numeroRegistroGeral, decimal valorBoletoGeral, int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples, int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada, int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada, int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "9", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 393, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a gera��o do registro TRAILER do arquivo de REMESSA.", ex);
            }
        }

        #endregion

        #region Retorno

        public override void LerHeaderRetornoCNAB400(string registro)
        {
            try
            {
                if (registro.Substring(0, 9) != "02RETORNO")
                    throw new Exception("O arquivo n�o � do tipo \"02RETORNO\"");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler HEADER do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

        public void LerDetalheRetornoCNAB400Segmento1(ref Boleto boleto, string registro)
        {
            try
            {
                //N� Controle do Participante
                boleto.NumeroControleParticipante = registro.Substring(31, 6);

                //Carteira
                boleto.Carteira = registro.Substring(82, 3);
                boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;

                //Identifica��o do T�tulo no Banco
                boleto.NossoNumero = registro.Substring(64, 08);
                boleto.NossoNumeroDV = registro.Substring(72, 1); //DV
                boleto.NossoNumeroFormatado = $"{boleto.Banco.Beneficiario.ContaBancaria.Agencia}{boleto.Banco.Beneficiario.ContaBancaria.DigitoAgencia}/{boleto.Carteira}/00{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
                //Identifica��o de Ocorr�ncia
                boleto.CodigoMovimentoRetorno = registro.Substring(108, 2);
                boleto.CodigoMotivoOcorrencia = registro.Substring(377, 2);
                boleto.DescricaoMovimentoRetorno = DescricaoOcorrenciaCnab400(boleto.CodigoMovimentoRetorno, boleto.CodigoMotivoOcorrencia);

                //N�mero do Documento
                boleto.NumeroDocumento = registro.Substring(116, 10);
                boleto.EspecieDocumento = AjustaEspecieCnab400(registro.Substring(173, 2));

                //Valores do T�tulo
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(152, 13)) / 100;
                boleto.ValorTarifas = Convert.ToDecimal(registro.Substring(175, 13)) / 100;
                boleto.ValorOutrasDespesas = 0; // Convert.ToDecimal(registro.Substring(188, 13)) / 100;
                boleto.ValorIOF = Convert.ToDecimal(registro.Substring(214, 13)) / 100;
                boleto.ValorAbatimento = Convert.ToDecimal(registro.Substring(227, 13)) / 100;
                boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(240, 13)) / 100;
                boleto.ValorPagoCredito = Convert.ToDecimal(registro.Substring(253, 13)) / 100;
                boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(266, 13)) / 100;
                boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(279, 13)) / 100;

                //Data Ocorr�ncia no Banco
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));

                //Data Vencimento do T�tulo
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(146, 6)).ToString("##-##-##"));

                // Data do Cr�dito
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));

                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

        public void LerDetalheRetornoCNAB400Segmento7(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException();
        }

        public void LerTrailerRetornoCNAB400(string registro)
        {
        }

        private string DescricaoOcorrenciaCnab400(string codigo, string codigoRejeicao)
        {
            switch (codigo)
            {
                case "01":
                    return "Entrada Confirmada na CIP";
                case "02":
                    return "Entrada Confirmada";
                case "03":
                    return "Entrada Rejeitada - " + EntradaDescricaoRejeicaoCnab400(codigoRejeicao);
                case "05":
                    return "Campo Livre Alterado";
                case "06":
                    return "Liquida��o Normal";
                case "08":
                    return "Liquida��o em Cartorio";
                case "09":
                    return "Baixa Autom�tica";
                case "10":
                    return "Baixa p�r ter sido liquidado";
                case "12":
                    return "Confirma Abatimento";
                case "13":
                    return "Abatimento Cancelado";
                case "14":
                    return "Vencimento Alterado";
                case "15":
                    return "Baixa Rejeitada - " + BaixaDescricaoRejeicaoCnab400(codigoRejeicao); 
                case "16":
                    return "Instru��o Rejeitada - " + InstrucoesDescricaoRejeicaoCnab400(codigoRejeicao);
                case "19":
                    return "Confirma Recebimento da Ordem de Protesto";
                case "20":
                    return "Confirma Recebimento da Ordem de Susta��o";
                case "22":
                    return "Seu Numero Alterado";
                case "23":
                    return "titulo Enviado para Cart�rio";
                case "24":
                    return "Confirma recebimento de ordem de n�o protestar";
                case "28":
                    return "D�bito de tarifas/custas � Correspondentes";
                case "40":
                    return "Tarifa de entrada (debitada na liquida��o)";
                case "43":
                    return "Baixado por ter sido protestado";
                case "96":
                    return "Tarifa sobre instru��es � M�s anterior";
                case "97":
                    return "Tarifa sobre baixas � M�s anterior";
                case "98":
                    return "Tarifa sobre entradas � M�s anterior";
                case "99":
                    return "Tarifa sobre instru��o de protesto/susta��o � m�s anterior";
                default:
                    return "";
            }
        }

        private string EntradaDescricaoRejeicaoCnab400(string codigo)
        {
            switch (codigo)
            {
                case "03":
                    return "CEP inv�lido � N�o temos cobrador � Cobrador n�o Localizado";
                case "04":
                    return "Sigla do Estado inv�lida";
                case "05":
                    return "Data de Vencimento inv�lida ou fora do prazo m�nimo";
                case "06":
                    return "C�digo do Banco inv�lido";
                case "08":
                    return "Nome do sacado n�o informado";
                case "10":
                    return "Logradouro n�o informado";
                case "14":
                    return "Registro em duplicidade";
                case "19":
                    return "Data de desconto inv�lida ou maior que a data de vencimento";
                case "20":
                    return "Valor de IOF n�o num�rico";
                case "21":
                    return "Movimento para t�tulo n�o cadastrado no sistema";
                case "22":
                    return "Valor de desconto + abatimento maior que o valor do t�tulo";
                case "25":
                    return "CNPJ ou CPF do sacado inv�lido (aceito com restri��es)";
                case "26":
                    return "Esp�cies de documento inv�lida (difere de 01...10,13 e 99)";
                case "27":
                    return "Data de emiss�o do t�tulo inv�lida";
                case "28":
                    return "Seu n�mero n�o informado";
                case "29":
                    return "CEP � igual a espa�o ou zeros; ou n�o num�rico";
                case "30":
                    return "Valor do t�tulo n�o num�rico ou inv�lido";
                case "36":
                    return "Valor de perman�ncia n�o num�rico";
                case "37":
                    return "Valor de perman�ncia inconsistente, pois, dentro de um m�s, ser� maior que o valor do t�tulo";
                case "38":
                    return "Valor de desconto/abatimento n�o num�rico ou inv�lido";
                case "39":
                    return "Valor de abatimento n�o num�rico";
                case "42":
                    return "T�tulo j� existente em nossos registros. Nosso n�mero n�o aceito";
                case "43":
                    return "T�tulo enviado em duplicidade nesse movimento";
                case "44":
                    return "T�tulo zerado ou em branco; ou n�o num�rico na remessa";
                case "46":
                    return "T�tulo enviado fora da faixa de Nosso N�mero, estipulada para o cliente.";
                case "51":
                    return "Tipo/N�mero de Inscri��o Sacador/Avalista Inv�lido";
                case "52":
                    return "Sacador/Avalista n�o informado";
                case "53":
                    return "Prazo de vencimento do t�tulo excede ao da contrata��o";
                case "54":
                    return "Banco informado n�o � nosso correspondente";
                case "55":
                    return "Banco correspondente informado n�o cobra este CEP ou n�o possui faixas de CEP cadastradas";
                case "56":
                    return "Nosso n�mero no correspondente n�o foi informado";
                case "57":
                    return "Remessa contendo duas instru��es incompat�veis � n�o protestar e dias de protesto ou prazo para protesto inv�lido.";
                case "58":
                    return "Entradas Rejeitadas � Reprovado no Represamento para An�lise";
                case "60":
                    return "CNPJ/CPF do sacado inv�lido � t�tulo recusado";
                case "87":
                    return "Excede Prazo m�ximo entre emiss�o e vencimento";
                case "99":
                    return "T�tulo n�o acatado pelo banco � entrar em contato Gerente da conta";
                case "AA":
                    return "Servi�o de Cobran�a inv�lido";
                case "AB":
                    return "Nossa Carteira inv�lida";
                case "AE":
                    return "T�tulo n�o possui abatimento";
                case "AI":
                    return "Nossa Cobran�a inv�lida";
                case "AJ":
                    return "Modalidade com bancos correspondentes inv�lida";
                case "AL":
                    return "Sacado impedido de entrar nesta cobran�a";
                case "AU":
                    return "Data de ocorr�ncia inv�lida";
                case "AV":
                    return "Valor de tarifa de cobran�a inv�lida";
                case "AX":
                    return "T�tulo em pagamento parcial";
                case "BC":
                    return "An�lise gerencial-sacado inv�lido p/opera��o cr�dito";
                case "BD":
                    return "An�lise gerencial-sacado inadimplente";
                case "BE":
                    return "An�lise gerencial-sacado difere do exigido";
                case "BF":
                    return "An�lise gerencial-vencto excede vencto da opera��o de cr�dito";
                case "BG":
                    return "An�lise gerencial-sacado com baixa liquidez";
                case "BH":
                    return "An�lise gerencial-sacado excede concentra��o";
                case "CC":
                    return "Valor de iof incompat�vel com a esp�cie documento";
                case "CD":
                    return "Efetiva��o de protesto sem agenda v�lida";
                case "CE":
                    return "T�tulo n�o aceito - pessoa f�sica";
                case "CF":
                    return "Excede prazo m�ximo da entrada ao vencimento";
                case "CG":
                    return "T�tulo n�o aceito � por an�lise gerencial";
                case "CH":
                    return "T�tulo em espera � em an�lise pelo banco";
                case "CJ":
                    return "An�lise gerencial-vencto do titulo abaixo przcurto";
                case "CK":
                    return "An�lise gerencial-vencto do titulo abaixo przlongo";
                case "CS":
                    return "T�tulo rejeitado pela checagem de duplicatas";
                case "DA":
                    return "An�lise gerencial � Entrada de T�tulo Descontado com limite cancelado";
                case "DB":
                    return "An�lise gerencial � Entrada de T�tulo Descontado com limite vencido";
                case "DC":
                    return "An�lise gerencial - cedente com limite cancelado";
                case "DD":
                    return "An�lise gerencial � cedente � sacado e teve seu limite cancelado";
                case "DE":
                    return "An�lise gerencial - apontamento no Serasa";
                case "DG":
                    return "Endere�o sacador/avalista n�o informado";
                case "DH":
                    return "Cep do sacador/avalista n�o informado";
                case "DI":
                    return "Cidade do sacador/avalista n�o informado";
                case "DJ":
                    return "Estado do sacador/avalista inv�lido ou n informado";
                case "DM":
                    return "Cliente sem C�digo de Flash cadastrado no cobrador";
                case "DN":
                    return "T�tulo Descontado com Prazo ZERO � Recusado";
                case "DP":
                    return "Data de Refer�ncia menor que a Data de Emiss�o do T�tulo";
                case "DT":
                    return "Nosso N�mero do Correspondente n�o deve ser informado";
                case "EB":
                    return "HSBC n�o aceita endere�o de sacado com mais de 38 caracteres";
                default:
                    return "";
            }
        }
        
        private string BaixaDescricaoRejeicaoCnab400(string codigo)
        {
            switch (codigo)
            {
                case "05":
                    return "Solicita��o de baixa para t�tulo j� baixado ou liquidado";
                case "06":
                    return "Solicita��o de baixa para t�tulo n�o registrado no sistema";
                case "08":
                    return "Solicita��o de baixa para t�tulo em float";
                default:
                    return "";
            }
        }

        private string InstrucoesDescricaoRejeicaoCnab400(string codigo)
        {
            switch (codigo)
            {
                case "04":
                    return "Data de Vencimento n�o num�rica ou inv�lida";
                case "05":
                    return "Data de vencimento inv�lida ou fora do prazo m�nimo";
                case "14":
                    return "Registro em duplicidade";
                case "19":
                    return "Data de desconto inv�lida ou maior que a data de vencimento";
                case "20":
                    return "Campo livre informado";
                case "21":
                    return "T�tulo n�o registrado no sistema";
                case "22":
                    return "T�tulo baixada ou liquidado";
                case "26":
                    return "Esp�cie de documento inv�lida";
                case "27":
                    return "Instru��o n�o aceita, p�r n�o ter sido emitida ordem de protesto ao cart�rio";
                case "28":
                    return "T�tulo tem instru��o de cart�rio ativa";
                case "29":
                    return "T�tulo n�o tem instru��o de cart�rio ativa";
                case "30":
                    return "Existe instru��o de n�o protestar, ativa para o t�tulo";
                case "36":
                    return "Valor de perman�ncia (mora) n�o num�rico";
                case "37":
                    return "T�tulo Descontado Instru��o n�o permitida para a carteira";
                case "38":
                    return "Valor do abatimento n�o num�rico ou maior que a soma do valor do t�tulo + perman�ncia + multa";
                case "39":
                    return "T�tulo em cart�rio";
                case "40":
                    return "Instru��o recusada - cobran�a vinculada / caucionada";
                case "44":
                    return "T�tulo zerado ou em brancos ou n�o num�rico na remessa";
                case "99":
                    return "Ocorr�ncia desconhecida na remessa";
                default:
                    return "";
            }
        }
        private TipoEspecieDocumento AjustaEspecieCnab400(string codigoEspecie)
        {
            switch (codigoEspecie)
            {
                case "01":
                    return TipoEspecieDocumento.DM;
                case "02":
                    return TipoEspecieDocumento.NP;
                case "05":
                    return TipoEspecieDocumento.RC;
                case "12":
                    return TipoEspecieDocumento.DS;
                case "99":
                    return TipoEspecieDocumento.OU;
                default:
                    return TipoEspecieDocumento.OU;
            }
        }

        #endregion
    }
}