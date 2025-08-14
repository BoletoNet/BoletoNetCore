using System;
using System.IO;
using System.Text;

namespace BoletoNetCore
{
    public class ArquivoRemessa
    {
        public IBanco Banco { get; set; }
        public TipoArquivo TipoArquivo { get; set; }
        public int NumeroArquivoRemessa { get; set; }
        public int? NumeroArquivoRemessaNoDia { get; set; }

        public string NomeArquivo => Banco?.FormatarNomeArquivoRemessa(NumeroArquivoRemessaNoDia ?? NumeroArquivoRemessa); //

        public ArquivoRemessa(IBanco banco, TipoArquivo tipoArquivo, int numeroArquivoRemessa, int? numeroArquivoRemessaNoDia = null)
        {
            Banco = banco;
            TipoArquivo = tipoArquivo;
            NumeroArquivoRemessa = numeroArquivoRemessa;
            NumeroArquivoRemessaNoDia = numeroArquivoRemessaNoDia;
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

                switch (TipoArquivo)
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
                        throw new Exception("Layout não encontrado");
                }

                StreamWriter arquivoRemessa = new StreamWriter(stream, Encoding.GetEncoding("ISO-8859-1"));
                string strline = string.Empty;

                // Header do Arquivo
                strline = Banco.GerarHeaderRemessa(TipoArquivo, NumeroArquivoRemessa, ref numeroRegistroGeral);
                if (string.IsNullOrWhiteSpace(strline))
                    throw new Exception("Registro HEADER obrigatório.");
                strline = FormataLinhaArquivoCNAB(strline, tamanhoRegistro);
                arquivoRemessa.WriteLine(strline);

                foreach (Boleto boleto in boletos)
                {
                    // Todos os boletos da coleção devem ser do mesmo banco da geração do arquivo remessa
                    // A solução aqui é forçar essa relação, mas talvez seja melhor subir uma exceção detalhando o erro.
                    boleto.Banco = Banco;

                    // Detalhe do arquivo
                    strline = boleto.Banco.GerarDetalheRemessa(TipoArquivo, boleto, ref numeroRegistroGeral);
                    if (string.IsNullOrWhiteSpace(strline))
                        throw new Exception("Registro DETALHE obrigatório.");
                    strline = FormataLinhaArquivoCNAB(strline, tamanhoRegistro);
                    arquivoRemessa.WriteLine(strline);

                    var mensagemRemessa = boleto.Banco.GerarMensagemRemessa(TipoArquivo, boleto, ref numeroRegistroGeral);
                    if (mensagemRemessa != null)
                    {
                        strline = mensagemRemessa;
                        strline = FormataLinhaArquivoCNAB(strline, tamanhoRegistro);
                        arquivoRemessa.WriteLine(strline);
                    }

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
                strline = Banco.GerarTrailerRemessa(TipoArquivo, NumeroArquivoRemessa,
                                                            ref numeroRegistroGeral, valorBoletoGeral,
                                                            numeroRegistroCobrancaSimples, valorCobrancaSimples,
                                                            numeroRegistroCobrancaVinculada, valorCobrancaVinculada,
                                                            numeroRegistroCobrancaCaucionada, valorCobrancaCaucionada,
                                                            numeroRegistroCobrancaDescontada, valorCobrancaDescontada);
                if (!string.IsNullOrWhiteSpace(strline))
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
                else
                {
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
