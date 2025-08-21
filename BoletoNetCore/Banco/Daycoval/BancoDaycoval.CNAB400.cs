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
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0392, 002, 0, boleto.DiasLimiteRecebimento.HasValue ? boleto.DiasLimiteRecebimento.Value.ToString("00") : "00", '0'); // Caso não for informado, irá definir o máximo de dias "99".
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
                throw new Exception("Erro durante a geração do registro da NFe do arquivo de REMESSA.", ex);
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
                throw new Exception("Erro durante a geração do registro TRAILER do arquivo de REMESSA.", ex);
            }
        }

        #endregion

        #region Retorno

        public override void LerHeaderRetornoCNAB400(string registro)
        {
            try
            {
                if (registro.Substring(0, 9) != "02RETORNO")
                    throw new Exception("O arquivo não é do tipo \"02RETORNO\"");
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
                //Nº Controle do Participante
                boleto.NumeroControleParticipante = registro.Substring(31, 6);

                //Carteira
                boleto.Carteira = registro.Substring(82, 3);
                boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;

                //Identificação do Título no Banco
                boleto.NossoNumero = registro.Substring(64, 08);
                boleto.NossoNumeroDV = registro.Substring(72, 1); //DV
                boleto.NossoNumeroFormatado = $"{boleto.Banco.Beneficiario.ContaBancaria.Agencia}{boleto.Banco.Beneficiario.ContaBancaria.DigitoAgencia}/{boleto.Carteira}/00{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
                //Identificação de Ocorrência
                boleto.CodigoMovimentoRetorno = registro.Substring(108, 2);
                boleto.CodigoMotivoOcorrencia = registro.Substring(377, 2);
                boleto.DescricaoMovimentoRetorno = DescricaoOcorrenciaCnab400(boleto.CodigoMovimentoRetorno, boleto.CodigoMotivoOcorrencia);

                //Número do Documento
                boleto.NumeroDocumento = registro.Substring(116, 10);
                boleto.EspecieDocumento = AjustaEspecieCnab400(registro.Substring(173, 2));

                //Valores do Título
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(152, 13)) / 100;
                boleto.ValorTarifas = Convert.ToDecimal(registro.Substring(175, 13)) / 100;
                boleto.ValorOutrasDespesas = 0; // Convert.ToDecimal(registro.Substring(188, 13)) / 100;
                boleto.ValorIOF = Convert.ToDecimal(registro.Substring(214, 13)) / 100;
                boleto.ValorAbatimento = Convert.ToDecimal(registro.Substring(227, 13)) / 100;
                boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(240, 13)) / 100;
                boleto.ValorPagoCredito = Convert.ToDecimal(registro.Substring(253, 13)) / 100;
                boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(266, 13)) / 100;
                boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(279, 13)) / 100;

                //Data Ocorrência no Banco
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));

                //Data Vencimento do Título
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(146, 6)).ToString("##-##-##"));

                // Data do Crédito
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));

                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

        public void LerDetalheRetornoCNAB400Segmento2(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException();
        }

        public void LerDetalheRetornoCNAB400Segmento4(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException();
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
                    return "Liquidação Normal";
                case "08":
                    return "Liquidação em Cartorio";
                case "09":
                    return "Baixa Automática";
                case "10":
                    return "Baixa pôr ter sido liquidado";
                case "12":
                    return "Confirma Abatimento";
                case "13":
                    return "Abatimento Cancelado";
                case "14":
                    return "Vencimento Alterado";
                case "15":
                    return "Baixa Rejeitada - " + BaixaDescricaoRejeicaoCnab400(codigoRejeicao); 
                case "16":
                    return "Instrução Rejeitada - " + InstrucoesDescricaoRejeicaoCnab400(codigoRejeicao);
                case "19":
                    return "Confirma Recebimento da Ordem de Protesto";
                case "20":
                    return "Confirma Recebimento da Ordem de Sustação";
                case "22":
                    return "Seu Numero Alterado";
                case "23":
                    return "titulo Enviado para Cartório";
                case "24":
                    return "Confirma recebimento de ordem de não protestar";
                case "28":
                    return "Débito de tarifas/custas – Correspondentes";
                case "40":
                    return "Tarifa de entrada (debitada na liquidação)";
                case "43":
                    return "Baixado por ter sido protestado";
                case "96":
                    return "Tarifa sobre instruções – Mês anterior";
                case "97":
                    return "Tarifa sobre baixas – Mês anterior";
                case "98":
                    return "Tarifa sobre entradas – Mês anterior";
                case "99":
                    return "Tarifa sobre instrução de protesto/sustação – mês anterior";
                default:
                    return "";
            }
        }

        private string EntradaDescricaoRejeicaoCnab400(string codigo)
        {
            switch (codigo)
            {
                case "03":
                    return "CEP inválido – Não temos cobrador – Cobrador não Localizado";
                case "04":
                    return "Sigla do Estado inválida";
                case "05":
                    return "Data de Vencimento inválida ou fora do prazo mínimo";
                case "06":
                    return "Código do Banco inválido";
                case "08":
                    return "Nome do sacado não informado";
                case "10":
                    return "Logradouro não informado";
                case "14":
                    return "Registro em duplicidade";
                case "19":
                    return "Data de desconto inválida ou maior que a data de vencimento";
                case "20":
                    return "Valor de IOF não numérico";
                case "21":
                    return "Movimento para título não cadastrado no sistema";
                case "22":
                    return "Valor de desconto + abatimento maior que o valor do título";
                case "25":
                    return "CNPJ ou CPF do sacado inválido (aceito com restrições)";
                case "26":
                    return "Espécies de documento inválida (difere de 01...10,13 e 99)";
                case "27":
                    return "Data de emissão do título inválida";
                case "28":
                    return "Seu número não informado";
                case "29":
                    return "CEP é igual a espaço ou zeros; ou não numérico";
                case "30":
                    return "Valor do título não numérico ou inválido";
                case "36":
                    return "Valor de permanência não numérico";
                case "37":
                    return "Valor de permanência inconsistente, pois, dentro de um mês, será maior que o valor do título";
                case "38":
                    return "Valor de desconto/abatimento não numérico ou inválido";
                case "39":
                    return "Valor de abatimento não numérico";
                case "42":
                    return "Título já existente em nossos registros. Nosso número não aceito";
                case "43":
                    return "Título enviado em duplicidade nesse movimento";
                case "44":
                    return "Título zerado ou em branco; ou não numérico na remessa";
                case "46":
                    return "Título enviado fora da faixa de Nosso Número, estipulada para o cliente.";
                case "51":
                    return "Tipo/Número de Inscrição Sacador/Avalista Inválido";
                case "52":
                    return "Sacador/Avalista não informado";
                case "53":
                    return "Prazo de vencimento do título excede ao da contratação";
                case "54":
                    return "Banco informado não é nosso correspondente";
                case "55":
                    return "Banco correspondente informado não cobra este CEP ou não possui faixas de CEP cadastradas";
                case "56":
                    return "Nosso número no correspondente não foi informado";
                case "57":
                    return "Remessa contendo duas instruções incompatíveis – não protestar e dias de protesto ou prazo para protesto inválido.";
                case "58":
                    return "Entradas Rejeitadas – Reprovado no Represamento para Análise";
                case "60":
                    return "CNPJ/CPF do sacado inválido – título recusado";
                case "87":
                    return "Excede Prazo máximo entre emissão e vencimento";
                case "99":
                    return "Título não acatado pelo banco – entrar em contato Gerente da conta";
                case "AA":
                    return "Serviço de Cobrança inválido";
                case "AB":
                    return "Nossa Carteira inválida";
                case "AE":
                    return "Título não possui abatimento";
                case "AI":
                    return "Nossa Cobrança inválida";
                case "AJ":
                    return "Modalidade com bancos correspondentes inválida";
                case "AL":
                    return "Sacado impedido de entrar nesta cobrança";
                case "AU":
                    return "Data de ocorrência inválida";
                case "AV":
                    return "Valor de tarifa de cobrança inválida";
                case "AX":
                    return "Título em pagamento parcial";
                case "BC":
                    return "Análise gerencial-sacado inválido p/operação crédito";
                case "BD":
                    return "Análise gerencial-sacado inadimplente";
                case "BE":
                    return "Análise gerencial-sacado difere do exigido";
                case "BF":
                    return "Análise gerencial-vencto excede vencto da operação de crédito";
                case "BG":
                    return "Análise gerencial-sacado com baixa liquidez";
                case "BH":
                    return "Análise gerencial-sacado excede concentração";
                case "CC":
                    return "Valor de iof incompatível com a espécie documento";
                case "CD":
                    return "Efetivação de protesto sem agenda válida";
                case "CE":
                    return "Título não aceito - pessoa física";
                case "CF":
                    return "Excede prazo máximo da entrada ao vencimento";
                case "CG":
                    return "Título não aceito – por análise gerencial";
                case "CH":
                    return "Título em espera – em análise pelo banco";
                case "CJ":
                    return "Análise gerencial-vencto do titulo abaixo przcurto";
                case "CK":
                    return "Análise gerencial-vencto do titulo abaixo przlongo";
                case "CS":
                    return "Título rejeitado pela checagem de duplicatas";
                case "DA":
                    return "Análise gerencial – Entrada de Título Descontado com limite cancelado";
                case "DB":
                    return "Análise gerencial – Entrada de Título Descontado com limite vencido";
                case "DC":
                    return "Análise gerencial - cedente com limite cancelado";
                case "DD":
                    return "Análise gerencial – cedente é sacado e teve seu limite cancelado";
                case "DE":
                    return "Análise gerencial - apontamento no Serasa";
                case "DG":
                    return "Endereço sacador/avalista não informado";
                case "DH":
                    return "Cep do sacador/avalista não informado";
                case "DI":
                    return "Cidade do sacador/avalista não informado";
                case "DJ":
                    return "Estado do sacador/avalista inválido ou n informado";
                case "DM":
                    return "Cliente sem Código de Flash cadastrado no cobrador";
                case "DN":
                    return "Título Descontado com Prazo ZERO – Recusado";
                case "DP":
                    return "Data de Referência menor que a Data de Emissão do Título";
                case "DT":
                    return "Nosso Número do Correspondente não deve ser informado";
                case "EB":
                    return "HSBC não aceita endereço de sacado com mais de 38 caracteres";
                default:
                    return "";
            }
        }
        
        private string BaixaDescricaoRejeicaoCnab400(string codigo)
        {
            switch (codigo)
            {
                case "05":
                    return "Solicitação de baixa para título já baixado ou liquidado";
                case "06":
                    return "Solicitação de baixa para título não registrado no sistema";
                case "08":
                    return "Solicitação de baixa para título em float";
                default:
                    return "";
            }
        }

        private string InstrucoesDescricaoRejeicaoCnab400(string codigo)
        {
            switch (codigo)
            {
                case "04":
                    return "Data de Vencimento não numérica ou inválida";
                case "05":
                    return "Data de vencimento inválida ou fora do prazo mínimo";
                case "14":
                    return "Registro em duplicidade";
                case "19":
                    return "Data de desconto inválida ou maior que a data de vencimento";
                case "20":
                    return "Campo livre informado";
                case "21":
                    return "Título não registrado no sistema";
                case "22":
                    return "Título baixada ou liquidado";
                case "26":
                    return "Espécie de documento inválida";
                case "27":
                    return "Instrução não aceita, pôr não ter sido emitida ordem de protesto ao cartório";
                case "28":
                    return "Título tem instrução de cartório ativa";
                case "29":
                    return "Título não tem instrução de cartório ativa";
                case "30":
                    return "Existe instrução de não protestar, ativa para o título";
                case "36":
                    return "Valor de permanência (mora) não numérico";
                case "37":
                    return "Título Descontado Instrução não permitida para a carteira";
                case "38":
                    return "Valor do abatimento não numérico ou maior que a soma do valor do título + permanência + multa";
                case "39":
                    return "Título em cartório";
                case "40":
                    return "Instrução recusada - cobrança vinculada / caucionada";
                case "44":
                    return "Título zerado ou em brancos ou não numérico na remessa";
                case "99":
                    return "Ocorrência desconhecida na remessa";
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