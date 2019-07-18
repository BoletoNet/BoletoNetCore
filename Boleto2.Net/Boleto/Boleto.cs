using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Boleto2Net
{
    [Serializable]
    [Browsable(false)]
    public class Boleto
    {
        public Boleto(IBanco banco)
        {
            Banco = banco;
            Carteira = banco.Cedente.ContaBancaria.CarteiraPadrao;
            CarteiraImpressaoBoleto = banco.Cedente.ContaBancaria.CarteiraPadrao;
            VariacaoCarteira = banco.Cedente.ContaBancaria.VariacaoCarteiraPadrao;
            TipoCarteira = banco.Cedente.ContaBancaria.TipoCarteiraPadrao;
        }

        public Boleto(IBanco banco, Boolean ignorarCarteira)
        {
            Banco = banco;
            //se o arquivo de retorno for criado par multiplas carteiras, ignora a carteira (para compatibilidade)
            if (!ignorarCarteira)
            {
                Carteira = banco.Cedente.ContaBancaria.CarteiraPadrao;
                VariacaoCarteira = banco.Cedente.ContaBancaria.VariacaoCarteiraPadrao;
                TipoCarteira = banco.Cedente.ContaBancaria.TipoCarteiraPadrao;
            }
        }

        public int CodigoMoeda { get; set; } = 9;
        public string EspecieMoeda { get; set; } = "R$";
        public int QuantidadeMoeda { get; set; } = 0;
        public string ValorMoeda { get; set; } = string.Empty;

        public TipoEspecieDocumento EspecieDocumento { get; set; } = TipoEspecieDocumento.NaoDefinido;

        public string NossoNumero { get; set; } = string.Empty;
        public string NossoNumeroDV { get; set; } = string.Empty;
        public string NossoNumeroFormatado { get; set; } = string.Empty;

        public TipoCarteira TipoCarteira { get; set; } = TipoCarteira.CarteiraCobrancaSimples;
        public string Carteira { get; set; } = string.Empty;
        public string VariacaoCarteira { get; set; } = string.Empty;
        public string CarteiraComVariacao => string.IsNullOrEmpty(Carteira) || string.IsNullOrEmpty(VariacaoCarteira) ? $"{Carteira}{VariacaoCarteira}" : $"{Carteira}/{VariacaoCarteira}";
        public string CarteiraImpressaoBoleto { get; set; } = string.Empty;

        public DateTime DataProcessamento { get; set; } = DateTime.Now;
        public DateTime DataEmissao { get; set; } = DateTime.Now;
        public DateTime DataVencimento { get; set; }
        public DateTime DataCredito { get; set; }

        public string NumeroDocumento { get; set; } = string.Empty;
        public string NumeroControleParticipante { get; set; } = string.Empty;
        public string Aceite { get; set; } = "N";
        public string UsoBanco { get; set; } = string.Empty;

        // Valores do Boleto
        public decimal ValorTitulo { get; set; }

        public bool ImprimirValoresAuxiliares { get; set; } = false;
        public decimal ValorPago { get; set; } // ValorPago deve ser preenchido com o valor que o sacado pagou. Se não existir essa informação no arquivo retorno, deixar zerada.
        public decimal ValorPagoCredito { get; set; } // ValorPagoCredito deve ser preenchido com o valor que será creditado na conta corrente. Se não existir essa informação no arquivo retorno, deixar zerada.
        public decimal ValorJurosDia { get; set; }
        public decimal ValorMulta { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorTarifas { get; set; }
        public decimal ValorOutrasDespesas { get; set; }
        public decimal ValorOutrosCreditos { get; set; }
        public decimal ValorIOF { get; set; }
        public decimal ValorAbatimento { get; set; }

        // Juros
        public decimal PercentualJurosDia { get; set; }

        public DateTime DataJuros { get; set; }

        // Multa
        public decimal PercentualMulta { get; set; }

        public DateTime DataMulta { get; set; }

        // Desconto
        public DateTime DataDesconto { get; set; }

        /// <summary>
        /// Banco no qual o boleto/título foi quitado/recolhido
        /// </summary>
        public string BancoCobradorRecebedor { get; set; }
        
        /// <summary>
        /// Agência na qual o boleto/título foi quitado/recolhido
        /// </summary>
        public string AgenciaCobradoraRecebedora { get; set; }

        public string CodigoOcorrencia { get; set; } = "01";
        public string DescricaoOcorrencia { get; set; } = string.Empty;
        public string CodigoOcorrenciaAuxiliar { get; set; } = string.Empty;

        public TipoCodigoProtesto CodigoProtesto { get; set; } = TipoCodigoProtesto.NaoProtestar;
        public int DiasProtesto { get; set; } = 0;
        public TipoCodigoBaixaDevolucao CodigoBaixaDevolucao { get; set; } = TipoCodigoBaixaDevolucao.BaixarDevolver;
        public int DiasBaixaDevolucao { get; set; } = 60;

        public string CodigoInstrucao1 { get; set; } = string.Empty;
        public string ComplementoInstrucao1 { get; set; } = string.Empty;
        public string CodigoInstrucao2 { get; set; } = string.Empty;
        public string ComplementoInstrucao2 { get; set; } = string.Empty;
        public string CodigoInstrucao3 { get; set; } = string.Empty;
        public string ComplementoInstrucao3 { get; set; } = string.Empty;

        public string MensagemInstrucoesCaixa { get; set; } = string.Empty;
        public string MensagemInstrucoesCaixaFormatado { get; set; } = string.Empty;
        public string MensagemArquivoRemessa { get; set; } = string.Empty;
        public string RegistroArquivoRetorno { get; set; } = string.Empty;

        public IBanco Banco { get; set; }
        public Sacado Sacado { get; set; } = new Sacado();
        public Sacado Avalista { get; set; } = new Sacado();
        public CodigoBarra CodigoBarra { get; } = new CodigoBarra();
        public ObservableCollection<GrupoDemonstrativo> Demonstrativos { get; } = new ObservableCollection<GrupoDemonstrativo>();

        public void ValidarDados()
        {
            // Banco Obrigatório
            if (Banco == null)
                throw new Exception("Boleto não possui Banco.");

            // Cedente Obrigatório
            if (Banco.Cedente == null)
                throw new Exception("Boleto não possui cedente.");

            // Conta Bancária Obrigatória
            if (Banco.Cedente.ContaBancaria == null)
                throw new Exception("Boleto não possui conta bancária.");

            // Sacado Obrigatório
            if (Sacado == null)
                throw new Exception("Boleto não possui sacado.");

            // Verifica se data do processamento é valida
            if (DataProcessamento == DateTime.MinValue)
                DataProcessamento = DateTime.Now;

            // Verifica se data de emissão é valida
            if (DataEmissao == DateTime.MinValue)
                DataEmissao = DateTime.Now;

            // Aceite
            if ((Aceite != "A") & (Aceite != "N") & (Aceite != "S"))
                throw new Exception("Aceite do Boleto deve ser definido com A, S ou N");

            Banco.ValidaBoleto(this);
            Banco.FormataNossoNumero(this);
            Boleto2Net.Banco.FormataCodigoBarra(this);
            Boleto2Net.Banco.FormataLinhaDigitavel(this);
            Boleto2Net.Banco.FormataMensagemInstrucao(this);

        }
    }
}