using System;

namespace BoletoNetCore
{
    partial class BancoUniprimeNortePR : IBancoCNAB400
    {
        public string GerarDetalheRemessaCNAB400(Boleto boleto, ref int registro)
        {
            var detalhe = GerarDetalheRemessaCNAB400Registro1(boleto, ref registro);
            return detalhe;
        }

        private string GerarDetalheRemessaCNAB400Registro1(Boleto boleto, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 001, 0, "1", ' '));   // 001-001 Identificação do Registro
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 019, 0, "", ' '));    // 002-020 Brancos

                // 021-037 Identificação da Empresa Beneficiária no Banco (Zero, Carteira, Agência e Conta Corrente)
                // no manual é indicado como um unico campo, estou separando para facilitar
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0021, 001, 0, "0", ' '));               // Zero Fixo 
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0022, 003, 0, boleto.Carteira, '0'));   // Carteira
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0025, 005, 0, this.Beneficiario.ContaBancaria.Agencia, '0'));   // Agencia
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0030, 007, 0, this.Beneficiario.ContaBancaria.Conta, '0'));   // Conta
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0037, 001, 0, this.Beneficiario.ContaBancaria.DigitoConta, '0'));   // Digito da conta


                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0038, 025, 0, boleto.NumeroDocumento, ' '));                    // 038-062 No de Controle do participante 
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0063, 003, 0, "084", ' '));                                     // 063-065 Código do Banco a ser debitado na Câmara de Compensação
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0066, 001, 0, boleto.PercentualMulta > 0 ? "2" : "0", ' '));    // 066-066 Campo de Multa (Se = 2 considerar percentual de multa. Se = 0, sem multa)
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0067, 004, 2, boleto.PercentualMulta, '0'));                    // 067-070 Percentual de Multa
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0071, 011, 0, boleto.NossoNumero, '0'));                        // 071-081 Identificação do Título no Banco
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0082, 001, 0, boleto.NossoNumeroDV, '0'));                      // 082-082 Digito de Auto Conferência do Número Bancário
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0083, 010, 0, "", ' '));                                        // 083-092 Brancos

                // 093-093 Condição para Emissão da Papeleta de Cobrança
                switch (boleto.Banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto)
                {
                    case TipoImpressaoBoleto.Banco:
                        reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0093, 001, 0, "1", '0'));
                        break;
                    case TipoImpressaoBoleto.Empresa:
                        reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0093, 001, 0, "2", '0'));
                        break;
                }

                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0094, 015, 0, "", ' '));   // 094-108 Brancos
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0109, 002, 0, String.IsNullOrWhiteSpace(boleto.CodigoMotivoOcorrencia) ? "01" : boleto.CodigoMotivoOcorrencia, '0')); // 109-110 Codigo de Ocorrência na Remessa (01 - Remessa, tem outros)
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0111, 010, 0, boleto.NumeroDocumento, ' '));   // 111-120 Número do Documento
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0121, 006, 0, boleto.DataVencimento.ToString("ddMMyy"), ' '));   // 121-126 Data do Vencimento do Titulo
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 013, 2, boleto.ValorTitulo, '0'); // 127 - 139 Valor do Titulo
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 008, 0, "", ' '));   // 140-147 Brancos
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0148, 002, 0, AjustaEspecieCnab400(boleto.EspecieDocumento), '0'); // 148-149 Especie do Titulo
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0150, 001, 0, boleto.Aceite, ' '); // 150 - 150 Aceite (No manual esta como "Sempre N")
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0151, 006, 0, boleto.DataEmissao.ToString("ddMMyy"), ' '));   // 151-156 Data de Emissao do Titulo

                // 157-158 indicacao de protesto / negativacao
                // 01 – Protestar dias corridos
                // 02 – Protestar dias úteis
                // 03 – Sem Protesto / Negativação
                // 07 – Negativar dias Corridos
                // 99 – Cancelar Protesto / Negativação
                switch (boleto.CodigoProtesto)
                {
                    case TipoCodigoProtesto.ProtestarDiasCorridos:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0157, 002, 0, "01", '0');
                        break;
                    case TipoCodigoProtesto.ProtestarDiasUteis:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0157, 002, 0, "02", '0');
                        break;
                    case TipoCodigoProtesto.NegativacaoSemProtesto:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0157, 002, 0, "07", '0');
                        break;
                    case TipoCodigoProtesto.CancelamentoProtestoAutomatico:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0157, 002, 0, "99", '0');
                        break;
                    // case TipoCodigoProtesto.NaoProtestar:
                    default:
                        reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0157, 002, 0, "03", '0');
                        break;
                }

                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0159, 002, 0, boleto.DiasProtesto, '0'); // 159-160 Quantidade de dias para protesto / Negativação
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0161, 013, 2, boleto.ValorJurosDia, '0'); // 161 - 173 Valor a ser cobrado por Dia de Atraso
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0174, 006, 0, boleto.ValorDesconto > 0 ? boleto.DataDesconto.ToString("ddMMyy") : "000000", ' ');   // 174-179 Data Limite P/Concessão de Desconto
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0180, 013, 2, boleto.ValorDesconto, '0'); // 180 - 192 Valor do Desconto
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0193, 013, 0, "", ' ');   // 193-205 Brancos
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0206, 013, 2, boleto.ValorAbatimento, '0'); // 206 - 218 Valor do Abatimento a ser concedido ou cancelado
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0219, 002, 0, boleto.Pagador.TipoCPFCNPJ("00"), '0'); // 219 - 220 Identificação do Tipo de Inscrição do Pagador
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 014, 0, boleto.Pagador.CPFCNPJ, '0'); // 221 - 234 No Inscrição do Pagador
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0235, 040, 0, boleto.Pagador.Nome, ' ');   // 235-274 Nome do Pagador
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0275, 040, 0, boleto.Pagador.Endereco.FormataLogradouro(40), ' ');   // 274-314 Endereço Completo
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0315, 012, 0, "", ' ');   // 315-326 Brancos
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0327, 008, 0, boleto.Pagador.Endereco.CEP, '0');   // 327-334 CEP
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0335, 020, 0, boleto.Pagador.Endereco.Bairro, ' ');   // 335-354 Bairro do Pagador
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0355, 038, 0, boleto.Pagador.Endereco.Cidade, ' ');   // 355-392 Cidade do Pagador
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0393, 002, 0, boleto.Pagador.Endereco.UF, ' ');   // 393-394 UF do Pagador
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0'); // 395-400 No Sequencial do Registro

                reg.CodificarLinha();
                return Utils.SubstituiCaracteresEspeciais(reg.LinhaRegistro);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE - Registro 01 do arquivo CNAB400.", ex);
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
                case "03":
                    return TipoEspecieDocumento.NS;
                case "05":
                    return TipoEspecieDocumento.RC;
                case "10":
                    return TipoEspecieDocumento.LC;
                case "11":
                    return TipoEspecieDocumento.ND;
                case "12":
                    return TipoEspecieDocumento.DS;
                default:
                    return TipoEspecieDocumento.OU;
            }
        }

        private string AjustaEspecieCnab400(TipoEspecieDocumento especieDocumento)
        {
            switch (especieDocumento)
            {
                case TipoEspecieDocumento.DM:
                    return "01";
                case TipoEspecieDocumento.NP:
                    return "02";
                case TipoEspecieDocumento.NS:
                    return "03";

                // todo: 04 Cobrança seriada

                case TipoEspecieDocumento.RC:
                    return "05";
                case TipoEspecieDocumento.LC:
                    return "10";
                case TipoEspecieDocumento.ND:
                    return "11";
                case TipoEspecieDocumento.DS:
                    return "12";
                default:
                    return "99";
            }
        }

        public string GerarHeaderRemessaCNAB400(ref int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                var reg = new TRegistroEDI();
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 001, 0, "0", ' '));                       //001-001 Identificação do Registro
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 001, 0, "1", ' '));                       //002-002 Identificação do Arquivo Remessa
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0003, 007, 0, "REMESSA", ' '));                 //003-009 Literal REMESSA
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0010, 002, 0, "01", ' '));                      //010-011 Código do Serviço
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0012, 015, 0, "COBRANCA", ' '));                //012-026 Literal Cobrança
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 020, 0, this.Beneficiario.Codigo, '0'));  //027-046 Código da Empresa
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 030, 0, this.Beneficiario.Nome, ' '));    //047-076 Código da Empresa
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0077, 003, 0, "084", ' '));                     //077-079 Número do banco Uniprime na Câmara de Compensação
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0080, 015, 0, "UNIPRIME", ' '));                        //080-094 Nome do Banco por Extenso
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0095, 006, 0, DateTime.Today.ToString("ddMMyy"), ' ')); //095-100 Data da Gravação do Arquivo
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0101, 008, 0, "", ' '));                                //101-108 Brancos
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0109, 002, 0, "MX", ' '));                              //109-110 Identificação do Sistema
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0111, 007, 0, numeroArquivoRemessa.ToString(), '0'));   //111-117 No Sequencial de Remessa
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0118, 277, 0, "", ' '));                                //118-394 Brancos
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0395, 006, 0, numeroRegistroGeral, '0'));               //395-400 No Sequencial do Registro "000001"
                reg.CodificarLinha();
                return Utils.SubstituiCaracteresEspeciais(reg.LinhaRegistro);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB400.", ex);
            }
        }

        public string GerarTrailerRemessaCNAB400(int numeroRegistroGeral, decimal valorBoletoGeral, int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples, int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada, int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada, int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 001, 0, "9", ' '));                    //001-001 Identificação do registro
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 393, 0, "", ' '));                     //002-394 Brancos
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0'));    //395-400 No Sequencial do Registro
                reg.CodificarLinha();
                return Utils.SubstituiCaracteresEspeciais(reg.LinhaRegistro);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do registro TRAILER do arquivo de REMESSA.", ex);
            }
        }

        public void LerDetalheRetornoCNAB400Segmento1(ref Boleto boleto, string registro)
        {
            try
            {
                //N� Controle do Participante
                boleto.NumeroControleParticipante = registro.Substring(37, 25);

                //Carteira (no arquivo retorno, vem com 1 caracter. Ajustamos para 2 caracteres, como no manual do Bradesco.
                boleto.Carteira = registro.Substring(107, 1).PadLeft(2, '0');
                boleto.TipoCarteira = TipoCarteira.CarteiraCobrancaSimples;

                //Identifica��o do T�tulo no Banco
                boleto.NossoNumero = registro.Substring(70, 11); //Sem o DV
                boleto.NossoNumeroDV = registro.Substring(81, 1); //DV
                boleto.NossoNumeroFormatado = $"{boleto.Carteira}/{boleto.NossoNumero}-{boleto.NossoNumeroDV}";

                //Identifica��o de Ocorr�ncia
                boleto.CodigoMovimentoRetorno = registro.Substring(108, 2);
                boleto.DescricaoMovimentoRetorno = DescricaoOcorrenciaCnab400(boleto.CodigoMovimentoRetorno);
                boleto.CodigoMotivoOcorrencia = registro.Substring(318, 10);

                //N�mero do Documento
                boleto.NumeroDocumento = registro.Substring(116, 10);
                boleto.EspecieDocumento = AjustaEspecieCnab400(registro.Substring(173, 2));

                //Valores do T�tulo
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(152, 13)) / 100;
                boleto.ValorTarifas = Convert.ToDecimal(registro.Substring(175, 13)) / 100;
                boleto.ValorOutrasDespesas = Convert.ToDecimal(registro.Substring(188, 13)) / 100;
                boleto.ValorIOF = Convert.ToDecimal(registro.Substring(214, 13)) / 100;
                boleto.ValorAbatimento = Convert.ToDecimal(registro.Substring(227, 13)) / 100;
                boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(240, 13)) / 100;
                boleto.ValorPago = Convert.ToDecimal(registro.Substring(253, 13)) / 100;
                boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(266, 13)) / 100;
                boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(279, 13)) / 100;

                //Data Ocorr�ncia no Banco
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));

                //Data Vencimento do T�tulo
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(146, 6)).ToString("##-##-##"));

                // Data do Cr�dito
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(295, 6)).ToString("##-##-##"));

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
            throw new System.NotImplementedException();
        }

        public void LerTrailerRetornoCNAB400(string registro)
        {
        }
    }
}