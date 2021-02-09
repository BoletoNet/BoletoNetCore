using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BoletoNetCore
{
    [ProgId(nameof(BoletoNetCoreProxy))]
    [ComVisible(true)]
    public class BoletoNetCoreProxy
    {
        // Esta classe é para permitir que essa DLL seja utilizada
        // via COM, em linguagens que como Visual Fox Pro.

        readonly public string Versao = "1.76";

        protected Boletos boletos = new Boletos();
        public int quantidadeBoletos { get { return boletos.Count; } }
        public Boleto boleto { get; set; }
        protected bool setupOk { get; set; } = false;

        public bool SetupCobranca(string cnpj, string razaoSocial,
                                    string enderecoLogradouro, string enderecoNumero, string enderecoComplemento, string enderecoBairro, string enderecoCidade, string enderecoEstado, string enderecoCep, string observacoes,
                                    int numeroBanco, string agencia, string digitoAgencia, string operacaoConta, string conta, string digitoConta,
                                    string codigoBeneficiario, string digitoCodigoBeneficiario, string codigoTransmissao,
                                    string carteira, string variacaoCarteira,
                                    int tipoCarteira, int tipoFormaCadastramento, int tipoImpressaoBoleto, int tipoDocumento,
                                    ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (string.IsNullOrWhiteSpace(cnpj))
                {
                    mensagemErro += "Cnpj não informado." + Environment.NewLine;
                }
                if (string.IsNullOrWhiteSpace(razaoSocial))
                {
                    mensagemErro += "Razão Social não informada." + Environment.NewLine;
                }
                if (numeroBanco == 0)
                {
                    mensagemErro += "Banco não informado." + Environment.NewLine;
                }
                if (string.IsNullOrWhiteSpace(agencia))
                {
                    mensagemErro += "Agência não informada." + Environment.NewLine;
                }
                if (string.IsNullOrWhiteSpace(conta))
                {
                    mensagemErro += "Conta não informada." + Environment.NewLine;
                }
                if (string.IsNullOrWhiteSpace(carteira))
                {
                    mensagemErro += "Carteira não informada." + Environment.NewLine;
                }
                if (!string.IsNullOrWhiteSpace(mensagemErro))
                {
                    return false;
                }
                if (tipoCarteira < 1 || tipoCarteira > 5)
                {
                    mensagemErro += "Tipo da Carteira inválida: 1-Simples, 2-Vinculada, 3-Caucionada, 4-Descontada, 5-Vendor" + Environment.NewLine;
                }
                if (tipoFormaCadastramento < 1 || tipoFormaCadastramento > 2)
                {
                    mensagemErro += "Tipo da Forma de Cadastramento inválida: 1-Com Registro, 2-Sem Registro" + Environment.NewLine;
                }
                if (tipoImpressaoBoleto < 1 || tipoImpressaoBoleto > 2)
                {
                    mensagemErro += "Tipo da Impressão do Boleto inválida: 1-Banco, 2-Empresa" + Environment.NewLine;
                }
                if (tipoDocumento < 1 || tipoDocumento > 2)
                {
                    mensagemErro += "Tipo do Documento do Boleto inválido: 1-Tradicional, 2-Escritural" + Environment.NewLine;
                }

                // Banco, Beneficiario, Conta Corrente
                boletos.Banco = Banco.Instancia(numeroBanco);
                boletos.Banco.Beneficiario = new Beneficiario
                {
                    CPFCNPJ = cnpj,
                    Nome = razaoSocial,
                    Observacoes = observacoes,
                    ContaBancaria = new ContaBancaria
                    {
                        Agencia = agencia,
                        DigitoAgencia = digitoAgencia,
                        OperacaoConta = operacaoConta,
                        Conta = conta,
                        DigitoConta = digitoConta,
                        CarteiraPadrao = carteira,
                        VariacaoCarteiraPadrao = variacaoCarteira,
                        TipoCarteiraPadrao = (TipoCarteira)tipoCarteira,
                        TipoFormaCadastramento = (TipoFormaCadastramento)tipoFormaCadastramento,
                        TipoImpressaoBoleto = (TipoImpressaoBoleto)tipoImpressaoBoleto,
                        TipoDocumento = (TipoDocumento)tipoDocumento
                    },
                    Codigo = codigoBeneficiario,
                    CodigoDV = digitoCodigoBeneficiario,
                    CodigoTransmissao = codigoTransmissao,
                    Endereco = new Endereco
                    {
                        LogradouroEndereco = enderecoLogradouro,
                        LogradouroNumero = enderecoNumero,
                        LogradouroComplemento = enderecoComplemento,
                        Bairro = enderecoBairro,
                        Cidade = enderecoCidade,
                        UF = enderecoEstado,
                        CEP = enderecoCep?.Replace(".", "")
                    }
                };
                boletos.Banco.FormataBeneficiario();
                setupOk = true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
            return true;
        }
        public bool AcessarBoletoDocumento(string documento, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (string.IsNullOrWhiteSpace(documento))
                {
                    mensagemErro = "Documento não informado.";
                    return false;
                }
                foreach (Boleto boleto in boletos)
                {
                    if (documento == boleto.NumeroDocumento)
                    {
                        this.boleto = boleto;
                        return true;
                    }
                }
                mensagemErro = "Boleto não encontrado: " + documento;
                return false;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool AcessarBoletoIndice(int index, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                boleto = boletos[index];
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool NovoBoleto(ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                boleto = new Boleto(boletos.Banco);
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool DefinirPagador(string cnpj, string razaoSocial, string endereco, string numero, string complemento, string bairro, string cidade, string uf, string cep, string observacoes, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                mensagemErro = "";
                if (boleto == null)
                {
                    mensagemErro += "Nenhum boleto aberto." + Environment.NewLine;
                    return false;
                }
                if (string.IsNullOrWhiteSpace(cnpj))
                {
                    mensagemErro += "Cnpj não informado." + Environment.NewLine;
                }
                if (string.IsNullOrWhiteSpace(razaoSocial))
                {
                    mensagemErro += "Razão Social não informada." + Environment.NewLine;
                }
                if (!string.IsNullOrWhiteSpace(mensagemErro))
                {
                    return false;
                }
                var pagador = new Pagador
                {
                    CPFCNPJ = cnpj,
                    Nome = razaoSocial,
                    Observacoes = observacoes,
                    Endereco = new Endereco
                    {
                        LogradouroEndereco = endereco,
                        LogradouroNumero = numero,
                        LogradouroComplemento = complemento,
                        Bairro = bairro,
                        Cidade = cidade,
                        UF = uf,
                        CEP = cep?.Replace(".", "")
                    }
                };
                boleto.Pagador = pagador;
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool DefinirBoleto(string siglaEspecieDocumento, string numeroDocumento, string nossoNumero, DateTime dataEmissao, DateTime dataProcessamento, DateTime dataVencimento, Decimal valorBoleto, string nrControle, string aceite, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                mensagemErro = "";
                if (boleto == null)
                {
                    mensagemErro += "Nenhum boleto aberto." + Environment.NewLine;
                    return false;
                }
                boleto.NumeroDocumento = numeroDocumento;
                boleto.NumeroControleParticipante = nrControle;
                boleto.NossoNumero = nossoNumero;
                boleto.DataEmissao = dataEmissao;
                boleto.DataProcessamento = dataProcessamento;
                boleto.DataVencimento = dataVencimento;
                boleto.ValorTitulo = valorBoleto;
                boleto.Aceite = aceite;
                boleto.EspecieDocumento = Utils.ToEnum<TipoEspecieDocumento>(siglaEspecieDocumento, true, TipoEspecieDocumento.OU);
                return true;

            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool DefinirDesconto(DateTime dataDesconto, decimal valorDesconto, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                mensagemErro = "";
                if (boleto == null)
                {
                    mensagemErro += "Nenhum boleto aberto." + Environment.NewLine;
                    return false;
                }
                boleto.DataDesconto = dataDesconto;
                boleto.ValorDesconto = valorDesconto;
                return true;

            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool DefinirMulta(DateTime dataMulta, decimal valorMulta, decimal percMulta, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                mensagemErro = "";
                if (boleto == null)
                {
                    mensagemErro += "Nenhum boleto aberto." + Environment.NewLine;
                    return false;
                }
                boleto.DataMulta = dataMulta;
                boleto.PercentualMulta = percMulta;
                boleto.ValorMulta = valorMulta;
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool DefinirJuros(DateTime dataJuros, decimal valorJuros, decimal percJuros, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                mensagemErro = "";
                if (boleto == null)
                {
                    mensagemErro += "Nenhum boleto aberto." + Environment.NewLine;
                    return false;
                }
                boleto.DataJuros = dataJuros;
                boleto.PercentualJurosDia = percJuros;
                boleto.ValorJurosDia = valorJuros;
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool DefinirInstrucoes(string instrucoesCaixa, string mensagemRemessa, string instrucao1, string instrucao1Aux, string instrucao2, string instrucao2Aux, string instrucao3, string instrucao3Aux, bool imprimirValoresAuxiliares, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                boleto.MensagemInstrucoesCaixa = instrucoesCaixa;
                boleto.MensagemArquivoRemessa = mensagemRemessa;
                boleto.CodigoInstrucao1 = instrucao1;
                boleto.ComplementoInstrucao1 = instrucao1Aux;
                boleto.CodigoInstrucao2 = instrucao2;
                boleto.ComplementoInstrucao2 = instrucao2Aux;
                boleto.CodigoInstrucao3 = instrucao3;
                boleto.ComplementoInstrucao3 = instrucao3Aux;
                boleto.ImprimirValoresAuxiliares = imprimirValoresAuxiliares;
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool DefinirProtesto(int codigoProtesto, int diasProtesto, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                boleto.CodigoProtesto = (TipoCodigoProtesto)codigoProtesto;
                boleto.DiasProtesto = diasProtesto;
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool DefinirBaixaDevolucao(int codigoBaixaDevolucao, int diasBaixaDevolucao, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                boleto.CodigoBaixaDevolucao = (TipoCodigoBaixaDevolucao)codigoBaixaDevolucao;
                boleto.DiasBaixaDevolucao = diasBaixaDevolucao;
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool FecharBoleto(ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                boleto.ValidarDados();
                boletos.Add(boleto);
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool LimparBoletos(ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                boletos.Clear();
                return true;

            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public virtual bool GerarBoletos(string nomeArquivo, ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(nomeArquivo))
                {
                    mensagemErro = "Nome do arquivo não informado." + Environment.NewLine;
                    return false;
                }
                if (quantidadeBoletos == 0)
                {
                    mensagemErro = "Nenhum boleto encontrado." + Environment.NewLine;
                    return false;
                }
                var extensaoArquivo = nomeArquivo.Substring(nomeArquivo.Length - 3).ToUpper();
                if (extensaoArquivo != "HTM" && extensaoArquivo != "PDF")
                {
                    mensagemErro = "Tipo do arquivo inválido: HTM ou PDF" + Environment.NewLine;
                    return false;
                }
                var html = new StringBuilder();
                foreach (Boleto boletoTmp in boletos)
                {
                    BoletoBancario imprimeBoleto = new BoletoBancario
                    {
                        Boleto = boletoTmp,
                        OcultarInstrucoes = false,
                        MostrarComprovanteEntrega = true,
                        MostrarEnderecoBeneficiario = true
                    };
                    {
                        html.Append("<div style=\"page-break-after: always;\">");
                        html.Append(imprimeBoleto.MontaHtmlEmbedded());
                        html.Append("</div>");
                    }
                }
                switch (extensaoArquivo.ToUpper())
                {
                    case "HTM":
                        GerarArquivoTexto(html.ToString(), nomeArquivo);
                        break;
                    default:
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool GerarRemessa(int formatoArquivo, string nomeArquivo, int sequenciaArquivo, ref string mensagemErro)
        {
            try
            {
                if (!setupOk)
                {
                    mensagemErro = "Realize o setup da cobrança antes de executar este método.";
                    return false;
                }
                if (formatoArquivo != 0 & formatoArquivo != 1)
                {
                    // Formato do Arquivo - CNAB240 = 0 / CNAB400 = 1
                    mensagemErro = "Tipo do arquivo inválido: 0-CNAB240, 1-CNAB400";
                    return false;
                }
                var arquivoRemessa = new ArquivoRemessa(boletos.Banco, (TipoArquivo)formatoArquivo, sequenciaArquivo);
                using (var fileStream = new FileStream(nomeArquivo, FileMode.Create))
                {
                    arquivoRemessa.GerarArquivoRemessa(boletos, fileStream);
                }
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }
        public bool LerRetorno(int formatoArquivo, string nomeArquivo, ref string mensagemErro)
        {
            try
            {
                if (boletos.Banco == null)
                {
                    mensagemErro = "Banco não definido.";
                    return false;
                }
                if (formatoArquivo != 0 & formatoArquivo != 1)
                {
                    // Formato do Arquivo - CNAB240 = 0 / CNAB400 = 1
                    mensagemErro = "Tipo do arquivo inválido: 0-CNAB240, 1-CNAB400";
                    return false;
                }
                boletos.Clear();

                var arquivoRetorno = new ArquivoRetorno(boletos.Banco, (TipoArquivo)formatoArquivo);
                using (var fileStream = new FileStream(nomeArquivo, FileMode.Open))
                {
                    boletos = arquivoRetorno.LerArquivoRetorno(fileStream);
                }

                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }

        public bool LerRetorno(int formatoArquivo, Stream nomeArquivo, ref string mensagemErro)
        {
            try
            {
                if (boletos.Banco == null)
                {
                    mensagemErro = "Banco não definido.";
                    return false;
                }
                if (formatoArquivo != 0 & formatoArquivo != 1)
                {
                    // Formato do Arquivo - CNAB240 = 0 / CNAB400 = 1
                    mensagemErro = "Tipo do arquivo inválido: 0-CNAB240, 1-CNAB400";
                    return false;
                }
                boletos.Clear();

                var arquivoRetorno = new ArquivoRetorno(boletos.Banco, (TipoArquivo)formatoArquivo);
                
                boletos = arquivoRetorno.LerArquivoRetorno(nomeArquivo);
                return true;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    mensagemErro += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                return false;
            }
        }

        protected void GerarArquivoTexto(string texto, string nomeArquivo)
        {
            using (FileStream fs = new FileStream(nomeArquivo, FileMode.Create))
            {
                using (var sw = new StreamWriter(fs, Encoding.Default))
                {
                    sw.Write(texto);
                    sw.Close();
                    fs.Close();
                }
            }
        }
        
    }

}
