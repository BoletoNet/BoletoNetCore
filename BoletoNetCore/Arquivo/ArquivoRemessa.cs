using System;
using System.IO;
using System.Text;

namespace BoletoNetCore
{
    public class ArquivoRemessa // : AbstractArquivoRemessa, IArquivoRemessa
    {
        public IBanco Banco { get; set; }
        public TipoArquivo TipoArquivo { get; set; }
        public int NumeroArquivoRemessa { get; set; }

        public string NomeArquivo => this.Banco?.FormatarNomeArquivoRemessa(this.NumeroArquivoRemessa); //

        public ArquivoRemessa(IBanco banco, TipoArquivo tipoArquivo, int numeroArquivoRemessa)
        {
            Banco = banco;
            TipoArquivo = tipoArquivo;
            NumeroArquivoRemessa = numeroArquivoRemessa;
        }

        public void GerarArquivoRemessa(Boletos boletos, Stream stream, bool fecharRemessa = true)
        {
            try
            {
                int numeroRegistroGeral = 0,
                    numeroRegistroCobrancaSimples = 0,
                    numeroRegistroCobrancaVinculada = 0,
                    numeroRegistroCobrancaCaucionada = 0,
                    numeroRegistroCobrancaDescontada = 0;
                decimal valorBoletoGeral = 0,
                    valorCobrancaSimples = 0,
                    valorCobrancaVinculada = 0,
                    valorCobrancaCaucionada = 0,
                    valorCobrancaDescontada = 0;

                int tamanhoRegistro;

                switch (this.TipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        tamanhoRegistro = 240;
                        break;
                    case TipoArquivo.CNAB400:
                        tamanhoRegistro = 400;
                        break;
                    case TipoArquivo.CNAB150:
                        tamanhoRegistro = 150;
                        break;
                    default:
                        throw new Exception("Layout n�o encontrado");
                }

                StreamWriter arquivoRemessa = new StreamWriter(stream, Encoding.GetEncoding("ISO-8859-1"));
                string strline = String.Empty;

                // Header do Arquivo
                strline = Banco.GerarHeaderRemessa(this.TipoArquivo, this.NumeroArquivoRemessa, ref numeroRegistroGeral);
                if (String.IsNullOrWhiteSpace(strline))
                    throw new Exception("Registro HEADER obrigat�rio.");
                strline = FormataLinhaArquivoCNAB(strline, tamanhoRegistro);
                arquivoRemessa.WriteLine(strline);

                foreach (Boleto boleto in boletos)
                {
                    // Todos os boletos da cole��o devem ser do mesmo banco da gera��o do arquivo remessa
                    // A solu��o aqui � for�ar essa rela��o, mas talvez seja melhor subir uma exce��o detalhando o erro.
                    boleto.Banco = this.Banco;

                    // Detalhe do arquivo
                    strline = boleto.Banco.GerarDetalheRemessa(this.TipoArquivo, boleto, ref numeroRegistroGeral);
                    if (String.IsNullOrWhiteSpace(strline))
                        throw new Exception("Registro DETALHE obrigat�rio.");
                    strline = FormataLinhaArquivoCNAB(strline, tamanhoRegistro);
                    arquivoRemessa.WriteLine(strline);
                    
                    // Ajusta Totalizadores
                    valorBoletoGeral += boleto.ValorTitulo;
                    switch (boleto.TipoCarteira)
                    {
                        case TipoCarteira.CarteiraCobrancaSimples:
                            numeroRegistroCobrancaSimples++;
                            valorCobrancaSimples += boleto.ValorTitulo;
                            break;
                        case TipoCarteira.CarteiraCobrancaVinculada:
                            numeroRegistroCobrancaVinculada++;
                            valorCobrancaVinculada += boleto.ValorTitulo;
                            break;
                        case TipoCarteira.CarteiraCobrancaCaucionada:
                            numeroRegistroCobrancaCaucionada++;
                            valorCobrancaCaucionada += boleto.ValorTitulo;
                            break;
                        case TipoCarteira.CarteiraCobrancaDescontada:
                            numeroRegistroCobrancaDescontada++;
                            valorCobrancaDescontada += boleto.ValorTitulo;
                            break;
                        case TipoCarteira.CarteiraCobrancaDebito:
                            numeroRegistroCobrancaSimples++;
                            valorCobrancaSimples += boleto.ValorTitulo;
                            valorCobrancaDescontada += boleto.ValorIOF;
                            break;
                        default:
                            break;
                    }
                }

                // Trailler do Arquivo
                strline = Banco.GerarTrailerRemessa(this.TipoArquivo, this.NumeroArquivoRemessa,
                                                            ref numeroRegistroGeral, valorBoletoGeral,
                                                            numeroRegistroCobrancaSimples, valorCobrancaSimples,
                                                            numeroRegistroCobrancaVinculada, valorCobrancaVinculada,
                                                            numeroRegistroCobrancaCaucionada, valorCobrancaCaucionada,
                                                            numeroRegistroCobrancaDescontada, valorCobrancaDescontada);
                if (!String.IsNullOrWhiteSpace(strline))
                {
                    strline = FormataLinhaArquivoCNAB(strline, tamanhoRegistro);
                    arquivoRemessa.WriteLine(strline);
                }
                

                if (fecharRemessa)
                {
                    arquivoRemessa.Close();
                    arquivoRemessa.Dispose();
                    arquivoRemessa = null;
                }
                else {
                    arquivoRemessa.Flush();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar arquivo remessa.", ex);
            }
        }

        private string FormataLinhaArquivoCNAB(string strLinha, int tamanhoRegistro)
        {
            strLinha = strLinha.ToUpper();
            if (Banco.RemoveAcentosArquivoRemessa)
            {
                strLinha = Utils.SubstituiCaracteresEspeciais(strLinha);
            }
            if (tamanhoRegistro != 0)
            {
                string[] strLinhas = strLinha.Split('\n');
                foreach (string s in strLinhas)
                {
                    if (s.Replace("\r", "").Length != tamanhoRegistro)
                        throw new Exception("Registro com tamanho incorreto:" + s.Replace("\r", "").Length.ToString() + " - " + s);
                }
            }
            return strLinha.ToUpper();
        }

    }
}
