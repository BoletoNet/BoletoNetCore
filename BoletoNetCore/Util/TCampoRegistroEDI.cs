using System;

namespace BoletoNetCore
{
    public class TCampoRegistroEDI
    {
        #region Vari�veis Privadas
        private string _DescricaoCampo;
        private TTiposDadoEDI _TipoCampo;
        private int _TamanhoCampo;
        private int _QtdDecimais;
        private object _ValorNatural;
        private string _ValorFormatado;
        private int _OrdemNoRegistroEDI;
        private string _SeparadorDatas;
        private string _SeparadorHora;
        private int _PosicaoInicial;
        private int _PosicaoFinal;
        private char _Preenchimento = ' ';
        #endregion

        #region Propriedades
        /// <summary>
        /// Descri��o do campo no registro EDI (meramente descritivo)
        /// </summary>
        public string DescricaoCampo
        {
            get { return _DescricaoCampo; }
            set { _DescricaoCampo = value; }
        }

        /// <summary>
        /// Tipo de dado de ORIGEM das informa��es do campo EDI.
        /// </summary>
        public TTiposDadoEDI TipoCampo
        {
            get { return _TipoCampo; }
            set { _TipoCampo = value; }
        }

        /// <summary>
        /// Tamanho em caracteres do campo no arquivo EDI (DESTINO)
        /// </summary>
        public int TamanhoCampo
        {
            get { return _TamanhoCampo; }
            set { _TamanhoCampo = value; }
        }

        /// <summary>
        /// Quantidade de casas decimais do campo, caso ele seja do tipo num�rico sem decimais. Caso
        /// n�o se aplique ao tipo de dado, o valor da propriedade ser� ignorado nas fun��es de formata��o.
        /// </summary>
        public int QtdDecimais
        {
            get { return _QtdDecimais; }
            set { _QtdDecimais = value; }
        }

        /// <summary>
        /// Valor de ORIGEM do campo, sem formata��o, no tipo de dado adequado ao campo. O valor deve ser atribuido
        /// com o tipo de dado adequado ao seu proposto, por exemplo, Double para representar valor, DateTime para
        /// representar datas e/ou horas, etc.
        /// </summary>
        public object ValorNatural
        {
            get { return _ValorNatural; }
            set { _ValorNatural = value; }
        }

        /// <summary>
        /// Valor formatado do campo, pronto para ser utilizado no arquivo EDI. A formata��o ser� de acordo
        /// com a especificada na propriedade TipoCampo, com num�ricos alinhados � direita e zeros � esquerda
        /// e campos alfanum�ricos alinhados � esquerda e com brancos � direita.
        /// Tamb�m pode receber o valor vindo do arquivo EDI, para ser decodificado e o resultado da decodifica��o na propriedade
        /// ValorNatural
        /// </summary>
        public string ValorFormatado
        {
            get { return _ValorFormatado; }
            set { _ValorFormatado = value; }
        }

        /// <summary>
        /// N�mero de ordem do campo no registro EDI
        /// </summary>
        public int OrdemNoRegistroEDI
        {
            get { return _OrdemNoRegistroEDI; }
            set { _OrdemNoRegistroEDI = value; }
        }

        /// <summary>
        /// Caractere separador dos elementos de campos com o tipo DATA. Colocar null caso esta propriedade
        /// n�o se aplique ao tipo de dado.
        /// </summary>
        public string SeparadorDatas
        {
            get { return _SeparadorDatas; }
            set { _SeparadorDatas = value; }
        }

        /// <summary>
        /// Caractere separador dos elementos de campos com o tipo HORA. Colocar null caso esta propriedade
        /// n�o se aplique ao tipo de dado.
        /// </summary>
        public string SeparadorHora
        {
            get { return _SeparadorHora; }
            set { _SeparadorHora = value; }
        }

        /// <summary>
        /// Posi��o do caracter inicial do campo no arquivo EDI
        /// </summary>
        public int PosicaoInicial
        {
            get { return _PosicaoInicial; }
            set { _PosicaoInicial = value; }
        }

        public int PosicaoFinal
        {
            get { return _PosicaoFinal; }
            set { _PosicaoFinal = value; }
        }
        /// <summary>
        /// Caractere de Preenchimento do campo da posi��o inicial at� a posi��o final
        /// </summary>
        public char Preenchimento
        {
            get { return _Preenchimento; }
            set { _Preenchimento = value; }
        }
        #endregion

        #region Construtores
        /// <summary>
        /// Cria um objeto TCampoRegistroEDI
        /// </summary>
        public TCampoRegistroEDI()
        { 
        }

        /// <summary>
        /// Cria um objeto do tipo TCampoRegistroEDI inicializando as propriedades b�sicas.
        /// </summary>
        /// <param name="pTipoCampo">Tipo de dado de origem dos dados</param>
        /// <param name="pPosicaoInicial">Posi��o Inicial do Campo no Arquivo</param>
        /// <param name="pTamanho">Tamanho em caracteres do campo (destino)</param>
        /// <param name="pDecimais">Quantidade de decimais do campo (destino)</param>
        /// <param name="pValor">Valor do campo (Origem), no tipo de dado adequado ao prop�sito do campo</param>
        /// <param name="pPreenchimento">Caractere de Preenchimento do campo caso o valor n�o ocupe todo o tamanho</param>
        /// <param name="pSeparadorHora">Separador de hora padr�o; null para sem separador</param>
        /// <param name="pSeparadorData">Separador de data padr�o; null para sem separador</param>
        public TCampoRegistroEDI(TTiposDadoEDI pTipoCampo, int pPosicaoInicial, int pTamanho, int pDecimais, object pValor, char pPreenchimento, string pSeparadorHora, string pSeparadorData)
        {
            this._TipoCampo = pTipoCampo;
            this._TamanhoCampo = pTamanho;
            this._QtdDecimais = pDecimais;
            this._ValorNatural = pValor;
            this._SeparadorHora = pSeparadorHora;
            this._SeparadorDatas = pSeparadorData;
            this._OrdemNoRegistroEDI = 0;
            this._DescricaoCampo = "";
            this._PosicaoInicial = pPosicaoInicial - 1; //Compensa a indexa��o com base em zero
            this._PosicaoFinal = pPosicaoInicial + this._TamanhoCampo;
            this._Preenchimento = pPreenchimento;
        }
        /// <summary>
        /// Cria um objeto do tipo TCampoRegistroEDI inicializando as propriedades b�sicas.
        /// </summary>
        /// <param name="pTipoCampo">Tipo de dado de origem dos dados</param>
        /// <param name="pPosicaoInicial">Posi��o Inicial do Campo no Arquivo</param>
        /// <param name="pTamanho">Tamanho em caracteres do campo (destino)</param>
        /// <param name="pDecimais">Quantidade de decimais do campo (destino)</param>
        /// <param name="pValor">Valor do campo (Origem), no tipo de dado adequado ao prop�sito do campo</param>
        /// <param name="pPreenchimento">Caractere de Preenchimento do campo caso o valor n�o ocupe todo o tamanho</param>
        public TCampoRegistroEDI(TTiposDadoEDI pTipoCampo, int pPosicaoInicial, int pTamanho, int pDecimais, object pValor, char pPreenchimento)
        {
            this._TipoCampo = pTipoCampo;
            this._TamanhoCampo = pTamanho;
            this._QtdDecimais = pDecimais;
            this._ValorNatural = pValor;
            this._SeparadorHora = null;
            this._SeparadorDatas = null;
            this._OrdemNoRegistroEDI = 0;
            this._DescricaoCampo = "";
            this._PosicaoInicial = pPosicaoInicial - 1; //Compensa a indexa��o com base em zero
            this._PosicaoFinal = pPosicaoInicial + this._TamanhoCampo;
            this._Preenchimento = pPreenchimento;
        }
        #endregion

        #region M�todos P�blicos
        /// <summary>
        /// Aplica formata��o ao valor do campo em ValorNatural, colocando o resultado na propriedade ValorFormatado
        /// </summary>
        public void CodificarNaturalParaEDI()
        {
            switch (this._TipoCampo)
            {
                case TTiposDadoEDI.ediAlphaAliEsquerda_____:
                {
                    this._ValorFormatado = FormatarAlphaEsquerda(this._ValorNatural, this._TamanhoCampo, this._Preenchimento);
                    break;
                }
                case TTiposDadoEDI.ediAlphaAliDireita______:
                {
                    this._ValorFormatado = FormatarAlphaDireita(this._ValorNatural, this._TamanhoCampo, this._Preenchimento);
                    break;
                }
                case TTiposDadoEDI.ediInteiro______________:
                {
                    this._ValorFormatado = FormatarInteiro(this._ValorNatural, this._TamanhoCampo, this._Preenchimento);
                    break;
                }
                case TTiposDadoEDI.ediNumericoSemSeparador_:
                {
                    this._ValorFormatado = FormatarNumericoSemSeparador(this._ValorNatural, this._TamanhoCampo, this._QtdDecimais, this._Preenchimento);
                    break;
                }
                case TTiposDadoEDI.ediNumericoComPonto_____:
                {
                    this._ValorFormatado = FormatarNumericoComPonto(this._ValorNatural, this._TamanhoCampo, this._QtdDecimais, this._Preenchimento);
                    break;
                }
                case TTiposDadoEDI.ediNumericoComVirgula___:
                {
                    this._ValorFormatado = FormatarNumericoComVirgula(this._ValorNatural, this._TamanhoCampo, this._QtdDecimais, this._Preenchimento);
                    break;
                }
                case TTiposDadoEDI.ediDataAAAAMMDD_________:
                {
                    if ( (DateTime)this._ValorNatural != DateTime.MinValue)
                    {
                        string sep = (this._SeparadorDatas == null ? "" : this._SeparadorDatas.ToString());
                        string Formatacao = "{0:yyyy" + sep + "MM" + sep + "dd}";
                        this._ValorFormatado = String.Format(Formatacao, this._ValorNatural);
                    }
                    else
                    {
                        this._ValorNatural = "";
                        goto case TTiposDadoEDI.ediAlphaAliEsquerda_____;
                    }
                    break;
                }
                case TTiposDadoEDI.ediDataDDMM_____________:
                {
                    if ((DateTime)this._ValorNatural != DateTime.MinValue)
                    {
                        string sep = (this._SeparadorDatas == null ? "" : this._SeparadorDatas.ToString());
                        string Formatacao = "{0:dd" + sep + "MM}";
                        this._ValorFormatado = String.Format(Formatacao, this._ValorNatural);
                    }
                    else
                    {
                        this._ValorNatural = "";
                        goto case TTiposDadoEDI.ediAlphaAliEsquerda_____;
                    }
                    break;
                }
                case TTiposDadoEDI.ediDataDDMMAAAA_________:
                {
                    if ((DateTime)this._ValorNatural != DateTime.MinValue)
                    {
                        string sep = (this._SeparadorDatas == null ? "" : this._SeparadorDatas.ToString());
                        string Formatacao = "{0:dd" + sep + "MM" + sep + "yyyy}";
                        this._ValorFormatado = String.Format(Formatacao, this._ValorNatural);
                    }
                    else
                    {
                        this._ValorNatural = "";
                        goto case TTiposDadoEDI.ediAlphaAliEsquerda_____;
                    }
                    break;
                }
                case TTiposDadoEDI.ediDataDDMMAA___________:
                {
                    if ((DateTime)this._ValorNatural != DateTime.MinValue)
                    {
                        string sep = (this._SeparadorDatas == null ? "" : this._SeparadorDatas.ToString());
                        string Formatacao = "{0:dd" + sep + "MM" + sep + "yy}";
                        this._ValorFormatado = String.Format(Formatacao, this._ValorNatural);
                    }
                    else
                    {
                        this._ValorNatural = "";
                        goto case TTiposDadoEDI.ediAlphaAliEsquerda_____;
                    }
                    break;
                }
                case TTiposDadoEDI.ediDataMMAAAA___________:
                {
                    if ((DateTime)this._ValorNatural != DateTime.MinValue)
                    {
                        string sep = (this._SeparadorDatas == null ? "" : this._SeparadorDatas.ToString());
                        string Formatacao = "{0:MM" + sep + "yyyy}";
                        this._ValorFormatado = String.Format(Formatacao, this._ValorNatural);
                    }
                    else
                    {
                        this._ValorNatural = "";
                        goto case TTiposDadoEDI.ediAlphaAliEsquerda_____;
                    }
                    break;
                }
                case TTiposDadoEDI.ediDataMMDD_____________:
                {
                    if ((DateTime)this._ValorNatural != DateTime.MinValue)
                    {
                        string sep = (this._SeparadorDatas == null ? "" : this._SeparadorDatas.ToString());
                        string Formatacao = "{0:MM" + sep + "dd}";
                        this._ValorFormatado = String.Format(Formatacao, this._ValorNatural);
                    }
                    else
                    {
                        this._ValorNatural = "";
                        goto case TTiposDadoEDI.ediAlphaAliEsquerda_____;
                    }
                    break;
                }
                case TTiposDadoEDI.ediHoraHHMM_____________:
                {
                    string sep = (this._SeparadorHora == null ? "" : this._SeparadorHora.ToString());
                    string Formatacao = "{0:HH" + sep + "mm}";
                    this._ValorFormatado = String.Format(Formatacao, this._ValorNatural);
                    break;
                }
                case TTiposDadoEDI.ediHoraHHMMSS___________:
                {
                    string sep = (this._SeparadorHora == null ? "" : this._SeparadorHora.ToString());
                    string Formatacao = "{0:HH" + sep + "mm" + sep + "ss}";
                    this._ValorFormatado = String.Format(Formatacao, this._ValorNatural);
                    break;
                }
                case TTiposDadoEDI.ediDataDDMMAAAAWithZeros:
                {
                    string sep = (this._SeparadorDatas == null ? "" : this._SeparadorDatas.ToString());
                    if ( (this._ValorNatural != null) || (!this.ValorNatural.ToString().Trim().Equals("")))
                    {
                        string Formatacao = "{0:dd" + sep + "MM" + sep + "yyyy}";
                        this._ValorFormatado = String.Format(Formatacao, this._ValorNatural);
                    }
                    else
                    {
                        this._ValorFormatado = "00" + sep + "00" + sep + "0000";
                    }
                    break;
                }
                case TTiposDadoEDI.ediDataAAAAMMDDWithZeros:
                {
                    string sep = (this._SeparadorDatas == null ? "" : this._SeparadorDatas.ToString());
                    if (this._ValorNatural != null)
                    {
                        string Formatacao = "{0:yyyy" + sep + "MM" + sep + "dd}";
                        this._ValorFormatado = String.Format(Formatacao, this._ValorNatural);
                    }
                    else
                    {
                        this._ValorFormatado = "00" + sep + "00" + sep + "0000";
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Transforma o valor vindo do campo do registro EDI da propriedade ValorFormatado para o valor natural (com o tipo
        /// de dado adequado) na propriedade ValorNatural
        /// </summary>
        public void DecodificarEDIParaNatural()
        {
            if (this._ValorFormatado.Trim().Equals(""))
            {
                this._ValorNatural = null;
            }
            else
            {
                switch (this._TipoCampo)
                {
                    case TTiposDadoEDI.ediAlphaAliEsquerda_____:
                    {
                        this._ValorNatural = this._ValorFormatado.ToString().Trim();
                        break;
                    }
                    case TTiposDadoEDI.ediAlphaAliDireita______:
                    {
                        this._ValorNatural = this._ValorFormatado.ToString().Trim();
                        break;
                    }
                    case TTiposDadoEDI.ediInteiro______________:
                    {
                        this._ValorNatural = long.Parse(this._ValorFormatado.ToString().Trim());
                        break;
                    }
                    case TTiposDadoEDI.ediNumericoSemSeparador_:
                    {
                        string s = this._ValorFormatado.Substring(0, this._ValorFormatado.Length - this._QtdDecimais) + "," + this._ValorFormatado.Substring(this._ValorFormatado.Length - this._QtdDecimais, this._QtdDecimais);
                        this._ValorNatural = Double.Parse(s.Trim());
                        break;
                    }
                    case TTiposDadoEDI.ediNumericoComPonto_____:
                    {
                        this._ValorNatural = Double.Parse(this._ValorFormatado.ToString().Replace(".", ",").Trim());
                        break;
                    }
                    case TTiposDadoEDI.ediNumericoComVirgula___:
                    {
                        this._ValorNatural = Double.Parse(this._ValorFormatado.ToString().Trim().Replace(".", ","));
                        break;
                    }
                    case TTiposDadoEDI.ediDataAAAAMMDD_________:
                    {
                        if (!this._ValorFormatado.Trim().Equals(""))
                        {
                            ReadOnlySpan<char> texto = this._ValorFormatado.AsSpan().Trim();
                            int ano, mes, dia;
                            
                            if (this._SeparadorDatas != null)
                            {
                                SplitDataHora(texto, this._SeparadorDatas[0], out var p1, out var p2, out var p3);
                                ano = int.Parse(p1.ToString());
                                mes = int.Parse(p2.ToString());
                                dia = int.Parse(p3.ToString());
                            }
                            else
                            {
                                ano = int.Parse(texto.Slice(0, 4).ToString());
                                mes = int.Parse(texto.Slice(4, 2).ToString());
                                dia = int.Parse(texto.Slice(6, 2).ToString());
                            }
                            
                            if (dia == 0 && mes == 0 && ano == 0)
                            {
                                this._ValorNatural = null;
                            }
                            else
                            {
                                this._ValorNatural = new DateTime(ano, mes, dia);
                            }
                        }
                        else
                        {
                            this._ValorNatural = null;
                        }
                        break;
                    }
                    case TTiposDadoEDI.ediDataDDMM_____________:
                    {
                        if (!this._ValorFormatado.Trim().Equals(""))
                        {
                            ReadOnlySpan<char> texto = this._ValorFormatado.AsSpan().Trim();
                            int dia, mes;
                            
                            if (this._SeparadorDatas != null)
                            {
                                SplitDataHora(texto, this._SeparadorDatas[0], out var p1, out var p2, out _);
                                dia = int.Parse(p1.ToString());
                                mes = int.Parse(p2.ToString());
                            }
                            else
                            {
                                dia = int.Parse(texto.Slice(0, 2).ToString());
                                mes = int.Parse(texto.Slice(2, 2).ToString());
                            }
                            
                            this._ValorNatural = new DateTime(1900, mes, dia);
                        }
                        else
                        {
                            this._ValorNatural = null;
                        }
                        break;
                    }
                    case TTiposDadoEDI.ediDataDDMMAAAA_________:
                    {
                        ReadOnlySpan<char> texto = this._ValorFormatado.AsSpan().Trim();
                        
                        if (texto.IsEmpty)
                        {
                            this._ValorNatural = new DateTime(1900, 1, 1); //data start
                        }
                        else
                        {
                            int dia, mes, ano;
                            
                            if (this._SeparadorDatas != null)
                            {
                                SplitDataHora(texto, this._SeparadorDatas[0], out var p1, out var p2, out var p3);
                                dia = int.Parse(p1.ToString());
                                mes = int.Parse(p2.ToString());
                                ano = int.Parse(p3.ToString());
                            }
                            else
                            {
                                dia = int.Parse(texto.Slice(0, 2).ToString());
                                mes = int.Parse(texto.Slice(2, 2).ToString());
                                ano = int.Parse(texto.Slice(4, 4).ToString());
                            }
                            
                            if (dia == 0 && mes == 0 && ano == 0)
                            {
                                this._ValorNatural = new DateTime(1900, 1, 1); //data start
                            }
                            else
                            {
                                this._ValorNatural = new DateTime(ano, mes, dia);
                            }
                        }
                        break;
                    }
                    case TTiposDadoEDI.ediDataDDMMAA___________:
                    {
                        ReadOnlySpan<char> texto = this._ValorFormatado.AsSpan().Trim();
                        int dia, mes, ano;
                        
                        if (this._SeparadorDatas != null)
                        {
                            SplitDataHora(texto, this._SeparadorDatas[0], out var p1, out var p2, out var p3);
                            dia = int.Parse(p1.ToString());
                            mes = int.Parse(p2.ToString());
                            ano = int.Parse(p3.ToString());
                        }
                        else
                        {
                            dia = int.Parse(texto.Slice(0, 2).ToString());
                            mes = int.Parse(texto.Slice(2, 2).ToString());
                            ano = int.Parse(texto.Slice(4, 2).ToString());
                        }
                        
                        // Ajusta ano de 2 dígitos para 4 dígitos
                        if (ano < 100)
                            ano += ano < 50 ? 2000 : 1900;
                        
                        this._ValorNatural = new DateTime(ano, mes, dia);
                        break;
                    }
                    case TTiposDadoEDI.ediDataMMAAAA___________:
                    {
                        ReadOnlySpan<char> texto = this._ValorFormatado.AsSpan().Trim();
                        int mes, ano;
                        
                        if (this._SeparadorDatas != null)
                        {
                            SplitDataHora(texto, this._SeparadorDatas[0], out var p1, out var p2, out _);
                            mes = int.Parse(p1.ToString());
                            ano = int.Parse(p2.ToString());
                        }
                        else
                        {
                            mes = int.Parse(texto.Slice(0, 2).ToString());
                            ano = int.Parse(texto.Slice(2, 4).ToString());
                        }
                        
                        this._ValorNatural = new DateTime(ano, mes, 1);
                        break;
                    }
                    case TTiposDadoEDI.ediDataMMDD_____________:
                    {
                        ReadOnlySpan<char> texto = this._ValorFormatado.AsSpan().Trim();
                        int mes, dia;
                        
                        if (this._SeparadorDatas != null)
                        {
                            SplitDataHora(texto, this._SeparadorDatas[0], out var p1, out var p2, out _);
                            mes = int.Parse(p1.ToString());
                            dia = int.Parse(p2.ToString());
                        }
                        else
                        {
                            mes = int.Parse(texto.Slice(0, 2).ToString());
                            dia = int.Parse(texto.Slice(2, 2).ToString());
                        }
                        
                        this._ValorNatural = new DateTime(1900, mes, dia);
                        break;
                    }
                    case TTiposDadoEDI.ediHoraHHMM_____________:
                    {
                        ReadOnlySpan<char> texto = this._ValorFormatado.AsSpan().Trim();
                        int hora, minuto;
                        
                        if (this._SeparadorHora != null)
                        {
                            SplitDataHora(texto, this._SeparadorHora[0], out var p1, out var p2, out _);
                            hora = int.Parse(p1.ToString());
                            minuto = int.Parse(p2.ToString());
                        }
                        else
                        {
                            hora = int.Parse(texto.Slice(0, 2).ToString());
                            minuto = int.Parse(texto.Slice(2, 2).ToString());
                        }
                        
                        this._ValorNatural = new DateTime(1, 1, 1, hora, minuto, 0);
                        break;
                    }
                    case TTiposDadoEDI.ediHoraHHMMSS___________:
                    {
                        ReadOnlySpan<char> texto = this._ValorFormatado.AsSpan().Trim();
                        int hora, minuto, segundo;
                        
                        if (this._SeparadorHora != null)
                        {
                            SplitDataHora(texto, this._SeparadorHora[0], out var p1, out var p2, out var p3);
                            hora = int.Parse(p1.ToString());
                            minuto = int.Parse(p2.ToString());
                            segundo = int.Parse(p3.ToString());
                        }
                        else
                        {
                            hora = int.Parse(texto.Slice(0, 2).ToString());
                            minuto = int.Parse(texto.Slice(2, 2).ToString());
                            segundo = int.Parse(texto.Slice(4, 2).ToString());
                        }
                        
                        this._ValorNatural = new DateTime(1, 1, 1, hora, minuto, segundo);
                        break;
                    }
                    case TTiposDadoEDI.ediDataDDMMAAAAWithZeros:
                    {
                        goto case TTiposDadoEDI.ediDataDDMMAAAA_________;
                    }
                    case TTiposDadoEDI.ediDataAAAAMMDDWithZeros:
                    {
                        goto case TTiposDadoEDI.ediDataAAAAMMDD_________;
                    }
                }
            }
        }

        #endregion

        #region M�todos Privados e Protegidos

        /// <summary>        /// Divide uma string de data/hora em 3 partes usando um separador (compatível com .NET Standard 2.0)
        /// </summary>
        private static void SplitDataHora(ReadOnlySpan<char> texto, char separador, out ReadOnlySpan<char> parte1, out ReadOnlySpan<char> parte2, out ReadOnlySpan<char> parte3)
        {
            int firstSep = texto.IndexOf(separador);
            if (firstSep < 0)
            {
                // Sem separador, retorna vazio
                parte1 = ReadOnlySpan<char>.Empty;
                parte2 = ReadOnlySpan<char>.Empty;
                parte3 = ReadOnlySpan<char>.Empty;
                return;
            }
            
            parte1 = texto.Slice(0, firstSep);
            
            int secondSep = texto.Slice(firstSep + 1).IndexOf(separador);
            if (secondSep < 0)
            {
                // Apenas 2 partes
                parte2 = texto.Slice(firstSep + 1);
                parte3 = ReadOnlySpan<char>.Empty;
                return;
            }
            
            secondSep += firstSep + 1; // Ajusta para posição absoluta
            parte2 = texto.Slice(firstSep + 1, secondSep - firstSep - 1);
            parte3 = texto.Slice(secondSep + 1);
        }

        /// <summary>        /// Formata valor alpha alinhado � esquerda com padding � direita
        /// </summary>
        private static string FormatarAlphaEsquerda(object valor, int tamanho, char preenchimento)
        {
            if (valor == null)
                return new string(preenchimento, tamanho);
            
            string textoOriginal = valor.ToString().Trim();
            
            if (textoOriginal.Length >= tamanho)
                return textoOriginal.Substring(0, tamanho);
            
            // Usa PadRight otimizado
            return textoOriginal.PadRight(tamanho, preenchimento);
        }

        /// <summary>
        /// Formata valor alpha alinhado � direita com padding � esquerda
        /// </summary>
        private static string FormatarAlphaDireita(object valor, int tamanho, char preenchimento)
        {
            if (valor == null)
                return new string(preenchimento, tamanho);
            
            string textoOriginal = valor.ToString().Trim();
            
            if (textoOriginal.Length >= tamanho)
                return textoOriginal.Substring(0, tamanho);
            
            // Usa PadLeft otimizado
            return textoOriginal.PadLeft(tamanho, preenchimento);
        }

        /// <summary>
        /// Formata valor inteiro com padding � esquerda
        /// </summary>
        private static string FormatarInteiro(object valor, int tamanho, char preenchimento)
        {
            if (valor == null)
                return new string(preenchimento, tamanho);
            
            string textoOriginal = valor.ToString().Trim();
            
            if (textoOriginal.Length >= tamanho)
                return textoOriginal.Substring(0, tamanho);
            
            return textoOriginal.PadLeft(tamanho, preenchimento);
        }

        /// <summary>
        /// Formata valor num�rico sem separador decimal
        /// </summary>
        private static string FormatarNumericoSemSeparador(object valor, int tamanho, int decimais, char preenchimento)
        {
            if (valor == null)
                return new string(' ', tamanho); // Regra espec�fica: NULL = espa�os

            // Formata e remove separadores
            string formatacao = $"{{0:f{decimais}}}";
            string resultado = string.Format(formatacao, valor)
                .Replace(",", "")
                .Replace(".", "")
                .Trim();
            
            if (resultado.Length >= tamanho)
                return resultado.Substring(0, tamanho);
            
            return resultado.PadLeft(tamanho, preenchimento);
        }

        /// <summary>
        /// Formata valor num�rico com ponto decimal
        /// </summary>
        private static string FormatarNumericoComPonto(object valor, int tamanho, int decimais, char preenchimento)
        {
            string formatacao = $"{{0:f{decimais}}}";
            string resultado = string.Format(formatacao, valor)
                .Replace(",", ".")
                .Trim();
            
            if (resultado.Length >= tamanho)
                return resultado.Substring(0, tamanho);
            
            return resultado.PadLeft(tamanho, preenchimento);
        }

        /// <summary>
        /// Formata valor num�rico com v�rgula decimal
        /// </summary>
        private static string FormatarNumericoComVirgula(object valor, int tamanho, int decimais, char preenchimento)
        {
            string formatacao = $"{{0:f{decimais}}}";
            string resultado = string.Format(formatacao, valor)
                .Replace(".", ",")
                .Trim();
            
            if (resultado.Length >= tamanho)
                return resultado.Substring(0, tamanho);
            
            return resultado.PadLeft(tamanho, preenchimento);
        }


        #endregion


    }
}