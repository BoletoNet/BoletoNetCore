using System;
using BoletoNetCore.Util;

namespace BoletoNetCore
{
    partial class BancoSicredi : IBancoCNAB240
    {
        public string GerarDetalheRemessaCNAB240(Boleto boleto, ref int registro)
        {
            var linhas = this.GerarDetalheRemessaCNAB240_SegmentoP(boleto, ref registro);
            linhas += Environment.NewLine + this.GerarDetalheRemessaCNAB240_SegmentoQ(boleto, ref registro);
            linhas += Environment.NewLine + this.GerarDetalheRemessaCNAB240_SegmentoR(boleto, ref registro);
            return linhas;
        }

        private string GerarDetalheRemessaCNAB240_SegmentoR(Boleto boleto, ref int registro)
        {
            registro++;
            var reg = new TRegistroEDI();
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 003, 0, "748", ' '); // 001 a 003 - Código do banco na compensação "748" SCIREDI
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0004, 004, 0, "0001", '0'); // 004 a 007 - Lote de serviço "0001"
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0008, 001, 0, "3", '0');    // 008 a 008 - Tipo de registro = "3" DETALHE
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0009, 005, 0, registro, '0'); // 009 a 013 - Nº sequencial do registro do lote
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "R", ' '); // 014 a 014 - Cód. segmento do registro detalhe
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, "", ' '); // 015 a 015 - Uso exclusivo FEBRABAN/CNAB
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0016, 002, 0, boleto.CodigoMovimentoRetorno, ' '); // 016 a 017 - Código de movimento remessa
            if (boleto.ValorDesconto2 == 0)
            {
                // Sem Desconto 2
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0019, 008, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 015, 0, "0", '0');
            }
            else
            {
                // Com Desconto 2
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0019, 008, 0, boleto.DataDesconto2, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 015, 2, boleto.ValorDesconto2, '0');
            }
            if (boleto.ValorDesconto3 == 0)
            {
                // Sem Desconto 3
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0042, 001, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0043, 008, 0, "0", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0051, 015, 0, "0", '0');
            }
            else
            {
                // Com Desconto 3
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0042, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0043, 008, 0, boleto.DataDesconto3, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0051, 015, 2, boleto.ValorDesconto3, '0');
            }
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0066, 001, 0, "2", '0'); // 66 Código da multa - 2 valor percentual (manual: "No Sicredi a multa só pode ser informada em valor percentual")
            reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0067, 008, 0, boleto.DataMulta, '0'); // 67 - 74 Se cobrar informe a data para iniciar a cobrança ou informe zeros se não cobrar
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0075, 015, 2, boleto.PercentualMulta, '0');  // 75 - 89 Percentual de multa. Informar zeros se não cobrar

            // os campos abaixo nao sao usados segundo o manual do sicredi
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0090, 010, 0, "", ' '); // 90-99 Informações do sacado (manual: sicredi nao usa)
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0100, 040, 0, "", ' '); // 100-139 Menssagem livre (manual: sicredi nao usa)
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0140, 040, 0, "", ' '); // 140-179 Menssagem livre (manual: sicredi nao usa)
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0180, 020, 0, "", ' '); // 180-199 Uso da FEBRABAN "Brancos"
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0200, 008, 0, "0000000", '0'); // 200-207 Código oco. sacado "0000000" (manual: sicredi nao usa)
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0208, 003, 0, "000", '0'); // 208-210 Código do banco na conta de débito "000"
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0211, 003, 0, "00000", '0'); // 211-215 Código da ag. debito
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0216, 003, 0, "", ' '); // 216 Digito da agencia
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0217, 012, 0, "000000000000", '0'); // 217-228 Conta corrente para debito
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0229, 001, 0, "", ' '); // 229 Digito conta de debito
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0230, 001, 0, "", '0'); // 230 Dv agencia e conta
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0231, 001, 0, "0", '0'); // 231 Aviso debito automatico
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0232, 009, 0, "", ' '); // 232-240 Uso FEBRABAN

            reg.CodificarLinha();
            return Utils.SubstituiCaracteresEspeciais(reg.LinhaRegistro);
        }

        private string GerarDetalheRemessaCNAB240_SegmentoQ(Boleto boleto, ref int registro)
        {
            registro++;
            var reg = new TRegistroEDI();
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 003, 0, "748", ' '); // 001 a 003 - Código do banco na compensação "748" SCIREDI
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0004, 004, 0, "0001", '0'); // 004 a 007 - Lote de serviço "0001"
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0008, 001, 0, "3", '0');    // 008 a 008 - Tipo de registro = "3" DETALHE
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0009, 005, 0, registro, '0'); // 009 a 013 - Nº sequencial do registro do lote
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "Q", ' '); // 014 a 014 - Cód. segmento do registro detalhe
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, "", ' '); // 015 a 015 - Uso exclusivo FEBRABAN/CNAB
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0016, 002, 0, boleto.CodigoMovimentoRetorno, ' '); // 016 a 017 - Código de movimento remessa
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0018, 001, 0, boleto.Pagador.TipoCPFCNPJ("0"), '1');  // 018 a 018 - Tipo de inscrição
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0019, 015, 0, boleto.Pagador.CPFCNPJ.OnlyNumber(), '0'); // 019 a 033 - Número de inscrição
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0034, 040, 0, boleto.Pagador.Nome, ' '); // 034 a 073 - Nome
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0074, 040, 0, boleto.Pagador.Endereco.FormataLogradouro(40), ' '); // 074 a 113 - Endereço
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0114, 015, 0, boleto.Pagador.Endereco.Bairro, ' '); // 114 a 128 - Bairro
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0129, 008, 0, boleto.Pagador.Endereco.CEP.OnlyNumber(), '0'); // 129 a 133 - CEP + 134 a 136 - Sufixo do CEP
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0137, 015, 0, boleto.Pagador.Endereco.Cidade, ' '); // 137 a 151 - Cidade
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0152, 002, 0, boleto.Pagador.Endereco.UF, ' '); // 152 a 153 - Unidade da Federação
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0154, 001, 0, boleto.Avalista.TipoCPFCNPJ("0"), '1'); // 154 a 154 - Tipo de inscrição
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0155, 015, 0, boleto.Avalista.CPFCNPJ.OnlyNumber(), '0'); // 155 a 169 - Número de inscrição
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0170, 040, 0, boleto.Avalista.Nome, ' '); // 170 a 209 - Nome do sacador/avalista
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0210, 003, 0, "000", '0'); // 210 a 212 - Cód. bco corresp. na compensação (Manual: "O Sicredi não utiliza esse campo, preencher com zeros")
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0213, 020, 0, "", ' '); // 213 a 232 - Nosso nº no banco correspondente
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0233, 008, 0, "", ' '); // 233 a 240 - Uso exclusivo FEBRABAN/CNAB
            reg.CodificarLinha();
            return Utils.SubstituiCaracteresEspeciais(reg.LinhaRegistro);
        }

        private string GerarDetalheRemessaCNAB240_SegmentoP(Boleto boleto, ref int registro)
        {
            registro++;
            var reg = new TRegistroEDI();
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 003, 0, "748", ' '); // 001 a 003 - Código do banco na compensação "748" SCIREDI
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0004, 004, 0, "0001", '0'); // 004 a 007 - Lote de serviço "0001"
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0008, 001, 0, "3", '0');    // 008 a 008 - Tipo de registro = "3" DETALHE
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0009, 005, 0, registro, '0'); // 009 a 013 - Nº sequencial do registro do lote
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0014, 001, 0, "P", ' '); // 014 a 014 - Cód. segmento do registro detalhe
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0015, 001, 0, "", ' '); // 015 a 015 - Uso exclusivo FEBRABAN/CNAB
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0016, 002, 0, boleto.CodigoMovimentoRetorno, ' '); // 016 a 017 - Código de movimento remessa
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0018, 005, 0, this.Beneficiario.ContaBancaria.Agencia.OnlyNumber(), '0'); // 018 a 022 - Agência mantenedora da conta
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0023, 001, 0, "", ' ');// 023 a 023 - Dígito verificador da agência
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0024, 012, 0, this.Beneficiario.ContaBancaria.Conta.OnlyNumber(), '0'); // 024 a 035 - Número da conta corrente
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0036, 001, 0, this.Beneficiario.ContaBancaria.DigitoConta.OnlyNumber(), '0'); // 036 a 036 - Digito da conta
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0037, 001, 0, "", ' '); // 037 a 037 - Dígito verificador da coop/ag/conta
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0038, 020, 0, boleto.NossoNumero.OnlyNumber() + boleto.NossoNumeroDV, '0'); // 038 a 057 - Identificação do título no banco
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0058, 001, 0, boleto.Carteira.OnlyNumber(), '1'); // 058 a 058 - Código da carteira
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0059, 001, 0, "1", '1'); // 059 a 059 - Forma de cadastro do título no banco
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0060, 001, 0, "1", '1'); // 060 a 060 - Tipo de documento

            // 061 a 062 - Identificação de emissão do bloqueto + 062 a 062 - Identificação da distribuição
            switch (this.Beneficiario.ContaBancaria.TipoImpressaoBoleto)
            {
                case TipoImpressaoBoleto.Empresa:
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0061, 002, 0, "22", '0');
                    break;
                case TipoImpressaoBoleto.Banco:
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0061, 002, 0, "11", '0');
                    break;
                case TipoImpressaoBoleto.BancoReemite:
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0061, 002, 0, "41", '0');
                    break;
                case TipoImpressaoBoleto.BancoNaoReemite:
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0061, 002, 0, "52", '0');
                    break;
                default:
                    throw new Exception("Tipo de Impressão inválido para banco sicredi: " + this.Beneficiario.ContaBancaria.TipoImpressaoBoleto);
            }

            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0063, 015, 0, boleto.NumeroDocumento, ' '); // 063 a 077 - Nº do documento de cobrança
            reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0078, 008, 0, boleto.DataVencimento, '0'); // 078 a 085 - Data de vencimento do título
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0086, 015, 2, boleto.ValorTitulo, '0'); // 086 a 100 - Valor nominal do título
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0101, 005, 0, "00000", '0'); // 101 a 105 - Coop./Ag. encarregada da cobrança
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0106, 001, 0, "", ' '); // 106 a 106 - Dígito verificador da coop./agência
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0107, 002, 0, (int)boleto.EspecieDocumento, '0'); // 107 a 108 - Espécie do título
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0109, 001, 0, boleto.Aceite, 'N'); // 109 a 109 - Identificação de título aceito/não aceito
            reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0110, 008, 0, boleto.DataEmissao, '0'); // 110 a 117 - Data da emissão do título

            // 1 - Valor / 2 - Percentual / 3 - Isento
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0118, 001, 0, boleto.ValorJurosDia != 0 ? "1" : "3", '0'); // 118 a 118 - Código do juro de mora
            reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0119, 008, 0, boleto.DataJuros, '0'); // 119 a 126 - Data do juro de mora
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 015, 2, boleto.ValorJurosDia, '0'); // 127 a 141 - Juros de mora por dia/taxa

            // 0 - Sem Desconto / 1 - Valor / 2 - Percentual / 3 - Valor por Atencipacao / 7 - Cancelamento de Desconto
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0142, 001, 0, boleto.ValorDesconto != 0 ? "1" : "0", '0'); // 142 a 142 - Código do desconto 1

            // Apesar de no manual nao especificar, quando passa pela homogalogação recebe-se a seguinte oritentação:
            // "Valor divergente, ao informar o valor 0, 3 ou 7 no código de desconto 1 no DETALHE segmento P posição 142, é necessário preencher este campo com zeros("00000000")"
            if (boleto.ValorDesconto != 0)
                reg.Adicionar(TTiposDadoEDI.ediDataDDMMAAAA_________, 0143, 008, 0, boleto.DataDesconto, '0'); // 143 a 150 - Data do desconto 1
            else
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0143, 008, 0, "0", '0'); // 143 a 150 - Data do desconto 1

            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0151, 015, 2, boleto.ValorDesconto, '0');  // 151 a 165 - Valor percentual a ser concedido
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0166, 015, 2, boleto.ValorIOF, '0'); // 166 a 180 - Valor do IOF a ser recolhido
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0181, 015, 2, boleto.ValorAbatimento, '0'); // 181 a 195 - Valor do abatimento
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0196, 025, 0, boleto.NumeroDocumento, ' '); // 196 a 220 - Identificação do título na empresa

            /* protesto
            1 - Protestar automaticamente
            3 - Não protestar/Não negativar
            8 - Negativar automaticamente
            9 - Cancelar protesto automático/Cancelar negativação
            */
            switch (boleto.CodigoProtesto)
            {
                case TipoCodigoProtesto.NaoProtestar:
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 001, 0, "3", '0'); // 221 a 221 - Código para protesto
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0222, 002, 0, "0", '0'); // 222 a 223 - Número de dias para protesto
                    break;
                case TipoCodigoProtesto.CancelamentoProtestoAutomatico:
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 001, 0, "9", '0'); // 221 a 221 - Código para protesto
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0222, 002, 0, "0", '0'); // 222 a 223 - Número de dias para protesto
                    break;
                case TipoCodigoProtesto.ProtestarDiasCorridos:
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 001, 0, "1", '0'); // 221 a 221 - Código para protesto
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0222, 002, 0, boleto.DiasProtesto, '0'); // 222 a 223 - Número de dias para protesto
                    break;
                case TipoCodigoProtesto.ProtestarDiasUteis:
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0221, 001, 0, "1", '0'); // 221 a 221 - Código para protesto
                    reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0222, 002, 0, boleto.DiasProtesto, '0'); // 222 a 223 - Número de dias para protesto
                    break;
                default:
                    throw new Exception("Tipo de protesto inválido para Sicredi: " + boleto.CodigoProtesto);
            }

            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0224, 001, 0, "1", '0'); // 224 a 224 - Código para baixa/devolução
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0225, 003, 0, boleto.DiasBaixaDevolucao, '0'); // 225 a 227 - Nº de dias para baixa/devolução (Manual: "O Sicredi não utiliza esse campo, preencher com zeros")
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0228, 002, 0, "09", '0'); // 228 a 229 - Código da moeda = "09"
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0230, 010, 0, "0000000000", '0'); // 230 a 239 - Nº do contrato da operação de crédito (Manual: "O Sicredi não utiliza esse campo, preencher com zeros")
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0240, 001, 0, "", ' '); // 240 a 240 - Uso exclusivo FEBRABAN/CNAB
            reg.CodificarLinha();
            return Utils.SubstituiCaracteresEspeciais(reg.LinhaRegistro);
        }

        public string GerarHeaderLoteRemessaCNAB240(ref int numeroArquivoRemessa, ref int numeroRegistro)
        {
            var reg = new TRegistroEDI();
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 003, 0, "748", ' ')); // 001 a 003 - Código do banco na compensação "748" SCIREDI
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0004, 004, 0, "0001", '0')); // 004 a 007 - Lote de serviço "0001"
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0008, 001, 0, "1", '0'));    // 008 a 008 - Tipo de registro = "1" HEADER LOTE
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 001, 0, "R", ' ')); // 009 a 009 - Tipo de operação = "R" Arquivo de Remessa
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0010, 002, 0, "01", '0')); // 010 a 011 - Tipo de serviço = "01" Cobrança
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0012, 002, 0, "", ' ')); // 012 a 013 - Uso exclusivo FEBRABAN/CNAB
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0014, 003, 0, "040", '0')); // 014 a 016 - Nº da versão do leiaute do lote = "040"
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0017, 001, 0, "", ' ')); // 017 a 017 - Uso exclusivo FEBRABAN/CNAB
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0018, 001, 0, this.Beneficiario.TipoCPFCNPJ("0"), ' ')); // 018 a 018 - Tipo de inscrição da empresa = "1" Pessoa Física "2" Pessoa Jurídica
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0019, 015, 0, this.Beneficiario.CPFCNPJ.OnlyNumber(), '0')); // 019 a 033 - Número de inscrição da empresa
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0034, 020, 0, "", ' ')); // 034 a 053 - Código do convênio no banco (O SICREDI não valida este campo; cfe Manual Agosto 2010 pág. 35)
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0054, 005, 0, this.Beneficiario.ContaBancaria.Agencia.OnlyNumber(), '0')); // 054 a 058 - Agência mantenedora da conta
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0059, 001, 0, "", ' ')); // 059 a 059 - Dígito verificador da agência
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0060, 012, 0, this.Beneficiario.ContaBancaria.Conta.OnlyNumber(), '0')); // 060 a 071 - Número da Canta
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0072, 001, 0, this.Beneficiario.ContaBancaria.DigitoConta.OnlyNumber(), '0')); // 072 a 072 - Zeros
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0073, 001, 0, "", '0')); // 073 a 073 - Dígito verificador da coop/ag/conta
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0074, 030, 0, this.Beneficiario.Nome, ' '));  // 074 a 103 - Nome da empresa
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0104, 040, 0, "", ' ')); // 104 a 143 - Mensagem 1
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0144, 040, 0, "", ' ')); // 144 a 183 - Mensagem 2
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0184, 008, 0, numeroArquivoRemessa, '0')); // 184 a 191 - Número remessa/retorno
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediDataDDMMAAAA_________, 0192, 008, 0, DateTime.Today, '0'));  // 192 a 199 - Data de gravação rem./ret.
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0200, 008, 0, "", '0')); // 200 a 207 - Data do crédito
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0208, 033, 0, "", ' ')); // 208 a 240 - Uso exclusivo FEBRABAN/CNAB
            reg.CodificarLinha();
            return Utils.SubstituiCaracteresEspeciais(reg.LinhaRegistro);
        }

        public string GerarHeaderRemessaCNAB240(ref int numeroArquivoRemessa, ref int numeroRegistro)
        {
            var reg = new TRegistroEDI();
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0001, 003, 0, "748", '0')); // 001 a 003 - Código do banco na compensação "748" SCIREDI
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0004, 004, 0, "0000", '0')); // 004 a 007 - Lote de serviço "0000"
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0008, 001, 0, "0", '1'));    // 008 a 008 - Tipo de registro = "0" HEADER ARQUIVO
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, "", ' ')); // 009 a 017 - Uso exclusivo FEBRABAN/CNAB            
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0018, 001, 0, this.Beneficiario.TipoCPFCNPJ("0"), ' ')); // 018 a 018 - Tipo de inscrição da empresa = "1" Pessoa Física "2" Pessoa Jurídica
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0019, 014, 0, this.Beneficiario.CPFCNPJ.OnlyNumber(), '0'));  // 019 a 032 - Número de inscrição da empresa
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0033, 020, 0, "", ' ')); // 033 a 052 - Código do convênio no banco (O SICREDI não valida este campo; cfe Manual Agosto 2010 pág. 35)
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0053, 005, 0, this.Beneficiario.ContaBancaria.Agencia.OnlyNumber(), '0')); // 053 a 057 - Agência mantenedora da conta
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0058, 001, 0, "", ' ')); // 058 a 058 - Dígito verificador da agência
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0059, 012, 0, this.Beneficiario.ContaBancaria.Conta.OnlyNumber(), '0')); // 059 a 070 - Número da Conta
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0071, 001, 0, this.Beneficiario.ContaBancaria.DigitoConta.OnlyNumber(), '0')); // 071 a 071 - DV Conta
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0072, 001, 0, "", '0')); // 072 a 072 - Dígito verificador da ag / conta
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0073, 030, 0, this.Beneficiario.Nome, ' ')); // 073 a 102 - Nome da empresa
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0103, 030, 0, "SICREDI", ' ')); // 103 a 132 - Nome do banco = "SICREDI"
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0133, 010, 0, "", ' ')); // 133 a 142 - Uso exclusivo FEBRABAN/CNAB
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0143, 001, 0, "1", ' ')); // 143 a 143 - Código Remessa/Retorno = "1" Remessa "2" Retorno
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediDataDDMMAAAA_________, 0144, 008, 0, DateTime.Today, '0'));  // 144 a 151 - Data de geração do arquivo
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediHoraHHMMSS___________, 0144, 008, 0, DateTime.Now, '0')); // 152 a 157 - Hora de geração do arquivo
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0158, 006, 0, numeroArquivoRemessa, '0')); // 158 a 163 - Número sequencial do arquivo
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0164, 003, 0, "081", '0')); // 164 a 166 - Nº da versão do leiaute do arquivo = "081"
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0167, 005, 0, "01600", '0')); // 167 a 171 - Densidade de gravação do arquivo = "01600"
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0172, 020, 0, "", ' ')); // 172 a 191 - Para uso reservado do banco
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0172, 020, 0, "", ' ')); // 192 a 211 - Para uso reservado da empresa
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0172, 029, 0, "", ' ')); // 212 a 240 - Uso exclusivo FEBRABAN/CNAB
            reg.CodificarLinha();
            return Utils.SubstituiCaracteresEspeciais(reg.LinhaRegistro);
        }

        public string GerarTrailerLoteRemessaCNAB240(ref int numeroArquivoRemessa, int numeroRegistroGeral, int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples, int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada, int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada, int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            // O número de registros no lote é igual ao número de registros gerados + 2 (header e trailler do lote)
            var numeroRegistrosNoLote = numeroRegistroGeral + 2;
            var reg = new TRegistroEDI();
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0001, 003, 0, "748", '0'); // 001 a 003 - Código do banco na compensação "748" SCIREDI
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0004, 004, 0, "0001", '0'); // 004 a 007 - Lote de serviço "0000"
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliDireita______, 0008, 001, 0, "5", '0');    // 008 a 008 - Tipo do registro = "5" TRAILLER LOTE
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, "", ' '); // 009 a 017 - Uso exclusivo FEBRABAN/CNAB

            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 006, 0, numeroRegistrosNoLote, '0'); // 018 a 023 - Quantidade de registros no lote
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 006, 2, 0, '0'); // 024 a 029 - Quantidade de títulos em cobrança
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0030, 017, 2, 0, '0'); // 030 a 046 - Valor total dos títulos em carteiras
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0047, 006, 2, 0, '0'); // 047 a 052 - Quantidade de títulos em cobrança
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0053, 017, 2, 0, '0'); // 053 a 069 - Valor total dos títulos em carteiras
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0070, 006, 2, 0, '0'); // 070 a 075 - Quantidade de títulos em cobrança
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0076, 017, 2, 0, '0'); // 076 a 092 - Quantidade de títulos em carteiras
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0093, 006, 2, 0, '0'); // 093 a 098 - Quantidade de títulos em cobrança
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0099, 017, 2, 0, '0'); // 099 a 115 - Valor total dos títulos em carteiras
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0116, 008, 0, "", ' '); // 116 a 123 - Número do aviso de lançamento
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0124, 117, 0, "", ' '); // 124 a 240 - Uso exclusivo FEBRABAN/CNAB

            reg.CodificarLinha();
            return Utils.SubstituiCaracteresEspeciais(reg.LinhaRegistro);
        }

        public string GerarTrailerRemessaCNAB240(int numeroRegistroGeral, decimal valorBoletoGeral, int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples, int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada, int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada, int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            // O número de registros no arquivo é igual ao número de registros gerados + 4 (header e trailler do lote / header e trailler do arquivo)
            var numeroRegistrosNoArquivo = numeroRegistroGeral + 4;
            var qtLotes = 1;
            var reg = new TRegistroEDI();
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 003, 0, "748", '0'); // 001 a 003 - Código do banco na compensação
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0004, 004, 0, "9999", '0'); // 004 a 007 - Lote de serviço = "9999"
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 001, 0, "9", '0'); // 008 a 008 - Tipo do registro = "9" TRAILLER ARQUIVO
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, "", ' '); // 009 a 017 - Uso exclusivo FEBRABAN/CNAB
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0018, 006, 0, qtLotes, '0'); // 018 a 023 - Quantidade de lotes do arquivo
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0024, 006, 0, numeroRegistrosNoArquivo, '0'); // 024 a 029 - Quantidade de registros do arquivo, inclusive este registro que está sendo criado agora
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0030, 006, 0, "0", '0'); // 030 a 035 - Quantidade de contas para conciliação (lotes)
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0036, 205, 0, "", ' '); // 036 a 240 - Uso exclusivo FEBRABAN/CNAB 
            reg.CodificarLinha();
            return reg.LinhaRegistro;
        }

        public override void LerDetalheRetornoCNAB240SegmentoT(ref Boleto boleto, string registro)
        {
            try
            {
                // pega os dados da classe base BancoFebraban.CNAB240
                base.LerDetalheRetornoCNAB240SegmentoT(ref boleto, registro);

                // o manual do sicredi preve o nosso numero da posicao 38 ate 57 (20 caracteres)
                // contudo, o retorno CNAB240 do sicredi retorna assim: "7480001300005T 0600720 0000000427179 212332573           1DOC1234"
                // sendo que o valor na possicao do nosso numero seria "212332883           "
                // há um espacamento em branco no retorno, e a classe base tem feito substring a partir da posicao 8 
                // assim, está pegando apenas o ultimo digito e o restante espacos em branco
                // string tmp = registro.Substring(37, 20);
                // boleto.NossoNumero = tmp.Substring(8, 11);
                // essa implementação sugere dessa maneira, ou seja, aproveitando a classe base mas alterando a posicao do nosso numero

                //Ajustado para separar o DV do Nossonumero mantendo o padrao dos outros bancos.
                //Alterando a propriedade NossoNumeroFormatado para realmente ficar fomatada

                var nossoNumero = registro.Substring(37, 20).Trim();
                boleto.NossoNumero = nossoNumero.Substring(0, nossoNumero.Length - 1);
                boleto.NossoNumeroDV = nossoNumero.Substring(nossoNumero.Length - 1, 1);
                boleto.NossoNumeroFormatado = string.Format("{0}/{1}-{2}", boleto.NossoNumero.Substring(0, 2), boleto.NossoNumero.Substring(2, 6), boleto.NossoNumeroDV);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO SICREDI / CNAB 240 / T.", ex);
            }
        }
    }
}