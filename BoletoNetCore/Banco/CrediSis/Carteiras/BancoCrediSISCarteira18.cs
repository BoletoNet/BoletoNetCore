using System;
using BoletoNetCore.Extensions;
using static System.String;

namespace BoletoNetCore
{
    [CarteiraCodigo("18")]

    public class BancoCrediSISCarteira18 : ICarteira<BancoCrediSIS>
    {
        internal static Lazy<ICarteira<BancoCrediSIS>> Instance { get; } = new Lazy<ICarteira<BancoCrediSIS>>(() => new BancoCrediSISCarteira18());

        private BancoCrediSISCarteira18()
        {
            
        }
        public void FormataCodigoCliente(Boleto boleto)
        {
            //if (boleto.Banco.Beneficiario.Codigo.Length == 7)
            //    boleto.Banco.Beneficiario.codigo = Convert.ToInt32(boleto.Cedente.Codigo.Substring(6));
            if (string.IsNullOrEmpty(boleto.Banco.Beneficiario.Codigo))
                throw new Exception("Código do beneficiario não informado");

            boleto.Banco.Beneficiario.Codigo = boleto.Banco.Beneficiario.Codigo.PadLeft(6, '0').Substring(0, 6);
        }

        public void FormataNossoNumero(Boleto boleto)
        {
            FormataCodigoCliente(boleto);
            #region Carteiras
            #region Carteira 18

            // Nao informou nosso numero
            if (string.IsNullOrEmpty(boleto.NossoNumero))
                throw new Exception("Nosso número não informado");

            // Nosso número não pode ter mais de 6 dígitos
            if (boleto.NossoNumero.Length > 6)
                throw new Exception("Nosso Número (" + boleto.NossoNumero + ") deve conter 6 dígitos.");

            boleto.NossoNumero = boleto.NossoNumero.PadLeft(6, '0');
            //boleto.NossoNumeroFormatado = boleto.NossoNumero;
            var mod11 = Mod11(boleto.Banco.Beneficiario.CPFCNPJ);
            var agencia = Utils.FormatCode(boleto.Banco.Beneficiario.ContaBancaria.Agencia, 4);
            var codBeneficiario = Utils.FormatCode(boleto.Banco.Beneficiario.ContaBancaria.CodigoConvenio, 6);
            var nossoNum = Utils.FormatCode(boleto.NossoNumero, 6);

            //boleto.NossoNumero = string.Format("{0}{1}{2}{3}{4}", "097", mod11, agencia, codBeneficiario, nossoNum);
            boleto.NossoNumeroFormatado = string.Format("{0}{1}{2}{3}{4}", "097", mod11, agencia, codBeneficiario, nossoNum);
            #endregion Carteira 18
            #endregion Carteiras
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            string valorBoleto = boleto.ValorTitulo.ToString("f").Replace(",", "").Replace(".", "");
            valorBoleto = Utils.FormatCode(valorBoleto, 10);

            #region Carteira 18
            //boleto.CodigoBarra.CampoLivre = string.Format("{0}{1}{2}{3}{4}",
            //        Utils.FormatCode(boleto.Banco.Codigo.ToString(), 3), // 1-3
            //        boleto.CodigoMoeda,                           // 4
            //                                                      // 5
            //        FatorVencimento(boleto),                // 6-9
            //        valorBoleto,                            // 10-19
            //        FormataCampoLivre(boleto));             // 20-44
            #endregion Carteira 18

            var mod11 = Mod11(boleto.Banco.Beneficiario.CPFCNPJ);
            var agencia = Utils.FormatCode(boleto.Banco.Beneficiario.ContaBancaria.Agencia, 4);
            var codBeneficiario = Utils.FormatCode(boleto.Banco.Beneficiario.ContaBancaria.CodigoConvenio, 6);
            var nossoNum = Utils.FormatCode(boleto.NossoNumero, 6);

            boleto.CodigoBarra.CampoLivre = string.Format("00000{0}{1}{2}{3}{4}", "097", mod11, agencia, codBeneficiario, nossoNum);

            return boleto.CodigoBarra.CampoLivre;
        }


        //#region Variáveis

        //private string _dacNossoNumero = string.Empty;
        //private int _dacBoleto = 0;
        //private string _codigoConvenioCliente = string.Empty;
        //#endregion

        //public void FormataNossoNumero(Boleto boleto)
        //{
        //    if (boleto.Banco.Beneficiario.ContaBancaria.Agencia.Length > 4)
        //        throw new NotImplementedException("A quantidade de dígitos da Agência " + boleto.Banco.Beneficiario.ContaBancaria.Agencia + ", são de 4 números.");
        //    else if (boleto.Banco.Beneficiario.ContaBancaria.Agencia.Length < 4)
        //        boleto.Banco.Beneficiario.ContaBancaria.Agencia = Utils.FormatCode(boleto.Banco.Beneficiario.ContaBancaria.Agencia, 4);

        //    if (IsNullOrWhiteSpace(boleto.NossoNumero))
        //        throw new Exception("Nosso Número não informado.");

        //    // Nosso número não pode ter mais de 8 dígitos
        //    if (boleto.NossoNumero.Length > 8)
        //        throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve conter 8 dígitos.");

        //    if (string.IsNullOrEmpty(boleto.Carteira))
        //        throw new NotImplementedException("Carteira não informada. Utilize a carteira 18.");

        //    //Verifica as carteiras implementadas
        //    if (!boleto.Carteira.Equals("18"))
        //        throw new NotImplementedException("Carteira não informada. Utilize a carteira 18.");

        //    //Verifica se o nosso número é válido
        //    if (Utils.ToString(boleto.NossoNumero) == string.Empty)
        //        throw new NotImplementedException("Nosso número inválido.");

        //    #region Convênio e Sequência de Nosso Número
        //    #region Carteira 18
        //    //Carteira 18 com nosso número de 6 posições
        //    //if (boleto.Carteira.Equals("18"))
        //    //{
        //    //    if (boleto.Cedente.Convenio.ToString().Length != 6)
        //    //        throw new NotImplementedException(string.Format("Para a carteira {0}, o número do convênio são de 6 posições", boleto.Carteira));

        //    //    if (boleto.NossoNumero.Length > 20)  //A sequencia inicial poderá somente ter no máximo 6 posições
        //    //        throw new NotImplementedException(string.Format("Para a carteira {0}, a quantidade máxima são de 20 de posições para o nosso número", boleto.Carteira));
        //    //}
        //    #endregion Carteira 18
        //    #endregion Convenio e ....

        //    #region Agência e Conta Corrente
        //    //Verificar se a Agencia esta correta
        //    if (boleto.Banco.Beneficiario.ContaBancaria.Agencia.Length > 4)
        //        throw new NotImplementedException("A quantidade de dígitos da Agência " + boleto.Banco.Beneficiario.ContaBancaria.Agencia + ", são de 4 números.");
        //    else if (boleto.Banco.Beneficiario.ContaBancaria.Agencia.Length < 4)
        //        boleto.Banco.Beneficiario.ContaBancaria.Agencia = Utils.FormatCode(boleto.Banco.Beneficiario.ContaBancaria.Agencia, 4);

        //    if (!boleto.Banco.Beneficiario.ContaBancaria.Agencia.Equals("0002") &
        //        !boleto.Banco.Beneficiario.ContaBancaria.Agencia.Equals("0003") &
        //        !boleto.Banco.Beneficiario.ContaBancaria.Agencia.Equals("0004") &
        //        !boleto.Banco.Beneficiario.ContaBancaria.Agencia.Equals("0005") &
        //        !boleto.Banco.Beneficiario.ContaBancaria.Agencia.Equals("0009") &
        //        !boleto.Banco.Beneficiario.ContaBancaria.Agencia.Equals("0011") &
        //        !boleto.Banco.Beneficiario.ContaBancaria.Agencia.Equals("0012") &
        //        !boleto.Banco.Beneficiario.ContaBancaria.Agencia.Equals("0017") &
        //        !boleto.Banco.Beneficiario.ContaBancaria.Agencia.Equals("0018") &
        //        !boleto.Banco.Beneficiario.ContaBancaria.Agencia.Equals("0019") &
        //        !boleto.Banco.Beneficiario.ContaBancaria.Agencia.Equals("0020") &
        //        !boleto.Banco.Beneficiario.ContaBancaria.Agencia.Equals("0021")
        //        )

        //        throw new NotImplementedException("Agência informada não é reconhecida pelo Credisis. Consulte as Agências possíveis.");

        //    //Verificar se a Conta esta correta
        //    if (boleto.Banco.Beneficiario.ContaBancaria.Conta.Length > 8)
        //        throw new NotImplementedException("A quantidade de dígitos da Conta " + boleto.Banco.Beneficiario.ContaBancaria.Conta + ", são de 8 números.");
        //    else if (boleto.Banco.Beneficiario.ContaBancaria.Conta.Length < 8)
        //        boleto.Banco.Beneficiario.ContaBancaria.Conta = Utils.FormatCode(boleto.Banco.Beneficiario.ContaBancaria.Conta, 8);
        //    #endregion Agência e Conta Corrente


        //    //Carteira 18 com nosso número de 11 posições
        //    if (boleto.Carteira.Equals("18"))
        //    {
        //        if (boleto.Banco.Beneficiario.Codigo.ToString().Length == 6)
        //            boleto.NossoNumero = string.Format("{0}{1}{2}{3}{4}", "097", ModNossoNumeroCPF(boleto.Banco.Beneficiario.CPFCNPJ), boleto.Banco.Beneficiario.ContaBancaria.Agencia, boleto.Banco.Beneficiario.ContaBancaria.CodigoConvenio, Utils.FormatCode(boleto.NossoNumero, 6));
        //        else
        //            boleto.NossoNumero = Utils.FormatCode(boleto.NossoNumero, 6);

        //        if (string.IsNullOrEmpty(boleto.Banco.Beneficiario.ContaBancaria.CodigoConvenio))
        //            throw new Exception("Código do convênio não informado");

        //        boleto.NossoNumeroFormatado = string.Format("{0}{1}{2}{3}{4}", "097", ModNossoNumeroCPF(boleto.Banco.Beneficiario.CPFCNPJ), Utils.FormatCode(boleto.Banco.Beneficiario.ContaBancaria.Agencia, 4) , Utils.FormatCode(boleto.Banco.Beneficiario.ContaBancaria.CodigoConvenio, 6) , Utils.FormatCode(boleto.NossoNumero, 6));
        //    }


        //    //if (boleto.BancoCarteira != null)
        //    //    boleto.BancoCarteira.FormataNossoNumero(boleto);
        //    //else

        //    boleto.NossoNumero = string.Format("{0}", boleto.NossoNumero);
        //}

        //public string FormataCodigoBarraCampoLivre(Boleto boleto)
        //{
        //    string FormataCampoLivre = string.Format("00000{0}{1}{2}{3}{4}",
        //                              "097"/*Utils.FormatCode(boleto.CodigoBarra.ToString(), 3)*/,                         // Fixo
        //                              Mod11(boleto.Banco.Beneficiario.CPFCNPJ),                                  // Módulo 11 do CPF/CNPJ (Incluindo dígitos verificadores) do Beneficiário
        //                              boleto.Banco.Beneficiario.ContaBancaria.Agencia,                           // Código da Agência CrediSIS ao qual o Beneficiário possui Conta.
        //                              Utils.FormatCode(boleto.Banco.Beneficiario.ContaBancaria.CodigoConvenio + "", 6), // Código de Convênio do Beneficiário no Sistema CrediSIS
        //                              Utils.FormatCode(boleto.NossoNumero, 6)                         // Sequencial Único do Boleto
        //                              );
        //    return FormataCampoLivre;
        //}

        private string Mod11(string seq)
        {
            int num1 = 0;
            int num2 = 9;
            int num3 = 2;
            for (int startIndex = seq.Length - 1; startIndex >= 0; --startIndex)
            {
                num1 += Convert.ToInt32(seq.Substring(startIndex, 1)) * num2;
                if (num2 == num3)
                    num2 = 9;
                else
                    --num2;
            }
            int num4 = num1 % 11;
            string str;
            switch (num4)
            {
                case 0:
                    str = "0";
                    break;
                case 10:
                    str = "1";
                    break;
                default:
                    str = num4.ToString();
                    break;
            }
            return str;
        }


        //public string ModNossoNumeroCPF(string value)
        //{
        //    string vRetorno;
        //    int d, s = 0, p = 2, b = 8;

        //    for (int i = value.Length - 1; i >= 0; i--)
        //    {
        //        s = s + (Convert.ToInt32(value.Substring(i, 1)) * p);
        //        if (p < b)
        //            p = p + 1;
        //        else
        //            p = 2;
        //    }

        //    d = 11 - (s % 11);
        //    if (d > 9)
        //        d = 0;
        //    vRetorno = d.ToString();

        //    return vRetorno;
        //}

        //public void FormataCodigoBarra(Boleto boleto)
        //{
        //    string valorBoleto = boleto.ValorTitulo.ToString("f").Replace(",", "").Replace(".", "");
        //    valorBoleto = Utils.FormatCode(valorBoleto, 10);

        //    #region Carteira 18
        //    if (boleto.Carteira.Equals("18"))
        //    {
        //        boleto.CodigoBarra.CampoLivre = string.Format("{0}{1}{2}{3}{4}",
        //            Utils.FormatCode(boleto.Banco.Codigo.ToString(), 3), // 1-3
        //            boleto.CodigoMoeda,                           // 4
        //                                                    // 5
        //            FatorVencimento(boleto),                // 6-9
        //            valorBoleto,                            // 10-19
        //            FormataCampoLivre(boleto));             // 20-44
        //    }
        //    #endregion Carteira 18

        //    _dacBoleto = Mod11(boleto.CodigoBarra.CodigoDeBarras, 9);

        //    boleto.CodigoBarra.CodigoDeBarras = StringExtensions.Left(boleto.CodigoBarra.CodigoDeBarras, 4) + _dacBoleto + StringExtensions.Right(boleto.CodigoBarra.CodigoDeBarras, 39);
        //}

        /// <summary>
        /// Formata o nosso número para ser mostrado no boleto.
        /// </summary>
        /// <remarks>
        /// Última a atualização por Transis em 26/09/2011
        /// </remarks>
        /// <param name="boleto"></param>


        //public void FormataLinhaDigitavel(Boleto boleto)
        //{
        //    string cmplivre = string.Empty;
        //    string campo1 = string.Empty;
        //    string campo2 = string.Empty;
        //    string campo3 = string.Empty;
        //    string campo4 = string.Empty;
        //    string campo5 = string.Empty;
        //    long icampo5 = 0;
        //    int digitoMod = 0;

        //    /*
        //    Campos 1 (AAABC.CCCCX):
        //    A = Código do Banco na Câmara de Compensação “001”
        //    B = Código da moeda "9" (*)
        //    C = Posição 20 a 24 do código de barras
        //    X = DV que amarra o campo 1 (Módulo 10, contido no Anexo 7)
        //     */

        //    cmplivre = StringExtensions.Mid(boleto.CodigoBarra.CodigoDeBarras, 20, 25);

        //    campo1 = StringExtensions.Left(boleto.CodigoBarra.CodigoDeBarras, 4) + StringExtensions.Mid(cmplivre, 1, 5);
        //    digitoMod = Mod10(campo1);
        //    campo1 = campo1 + digitoMod.ToString();
        //    campo1 = StringExtensions.Mid(campo1, 1, 5) + "." + StringExtensions.Mid(campo1, 6, 5);
        //    /*
        //    Campo 2 (DDDDD.DDDDDY)
        //    D = Posição 25 a 34 do código de barras
        //    Y = DV que amarra o campo 2 (Módulo 10, contido no Anexo 7)
        //     */
        //    campo2 = StringExtensions.Mid(cmplivre, 6, 10);
        //    digitoMod = Mod10(campo2);
        //    campo2 = campo2 + digitoMod.ToString();
        //    campo2 = StringExtensions.Mid(campo2, 1, 5) + "." + StringExtensions.Mid(campo2, 6, 6);


        //    /*
        //    Campo 3 (EEEEE.EEEEEZ)
        //    E = Posição 35 a 44 do código de barras
        //    Z = DV que amarra o campo 3 (Módulo 10, contido no Anexo 7)
        //     */
        //    campo3 = StringExtensions.Mid(cmplivre, 16, 10);
        //    digitoMod = Mod10(campo3);
        //    campo3 = campo3 + digitoMod;
        //    campo3 = StringExtensions.Mid(campo3, 1, 5) + "." + StringExtensions.Mid(campo3, 6, 6);

        //    /*
        //    Campo 4 (K)
        //    K = DV do Código de Barras (Módulo 11, contido no Anexo 10)
        //     */
        //    campo4 = StringExtensions.Mid(boleto.CodigoBarra.CodigoDeBarras, 5, 1);

        //    /*
        //    Campo 5 (UUUUVVVVVVVVVV)
        //    U = Fator de Vencimento ( Anexo 10)
        //    V = Valor do Título (*)
        //     */
        //    icampo5 = Convert.ToInt64(StringExtensions.Mid(boleto.CodigoBarra.CodigoDeBarras, 6, 14));

        //    if (icampo5 == 0)
        //        campo5 = "000";
        //    else
        //        campo5 = icampo5.ToString();

        //    boleto.CodigoBarra.LinhaDigitavel = campo1 + " " + campo2 + " " + campo3 + " " + campo4 + " " + campo5;
        //}

        ///<summary>
        /// Campo Livre 25 posiçoes
        ///    20 a 23 -  4 - Agência Cedente (Sem o digito verificador,completar com zeros a esquerda quandonecessário)
        ///    24 a 25 -  2 - Carteira
        ///    26 a 36 - 11 - Número do Nosso Número(Sem o digito verificador)
        ///    37 a 43 -  7 - Conta do Cedente (Sem o digito verificador,completar com zeros a esquerda quando necessário)
        ///    44 a 44	- 1 - Zero            
        ///</summary>
        //public string FormataCodigoBarraCampoLivre(Boleto boleto)
        //{
        //    string FormataCampoLivre = string.Format("00000{0}{1}{2}{3}{4}",
        //                                Utils.FormatCode(boleto.Banco.Codigo.ToString(), 3),                         // Fixo
        //                                Mod11(boleto.Banco.Beneficiario.CPFCNPJ),                                  // Módulo 11 do CPF/CNPJ (Incluindo dígitos verificadores) do Beneficiário
        //                                boleto.Banco.Beneficiario.ContaBancaria.Agencia,                           // Código da Agência CrediSIS ao qual o Beneficiário possui Conta.
        //                                Utils.FormatCode(boleto.Banco.Beneficiario.ContaBancaria.CodigoConvenio + "", 6),                // Código de Convênio do Beneficiário no Sistema CrediSIS
        //                                Utils.FormatCode(boleto.NossoNumero, 6)                         // Sequencial Único do Boleto
        //    );
        //    return FormataCampoLivre;
        //}

       

        //public override void FormataCodigoBarra(Boleto boleto)
        //{
        //    string valorBoleto = boleto.ValorBoleto.ToString("f").Replace(",", "").Replace(".", "");
        //    valorBoleto = Utils.FormatCode(valorBoleto, 10);

        //    #region Carteira 18
        //    if (boleto.Carteira.Equals("18"))
        //    {
        //        boleto.CodigoBarra.Codigo = string.Format("{0}{1}{2}{3}{4}",
        //            Utils.FormatCode(Codigo.ToString(), 3), // 1-3
        //            boleto.Moeda,                           // 4
        //                                                    // 5
        //            FatorVencimento(boleto),                // 6-9
        //            valorBoleto,                            // 10-19
        //            FormataCampoLivre(boleto));             // 20-44
        //    }
        //    #endregion Carteira 18

        //    _dacBoleto = Mod11(boleto.CodigoBarra.Codigo, 9);

        //    boleto.CodigoBarra.c = Strings.Left(boleto.CodigoBarra.Codigo, 4) + _dacBoleto + Strings.Right(boleto.CodigoBarra.Codigo, 39);
        //}

        //public void FormataCodigoCliente(Beneficiario cedente)
        //{
        //    //if (cedente.Codigo.Length > 6)
        //    //    cedente.DigitoCedente = Convert.ToInt32(cedente.Codigo.Substring(6));

        //    cedente.Codigo = cedente.Codigo.Substring(0, 6).PadLeft(6, '0');
        //}

      

        public static long FatorVencimento(Boleto boleto)
        {
            var dateBase = new DateTime(1997, 10, 7, 0, 0, 0);

            //Verifica se a data esta dentro do range utilizavel
            var dataAtual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            long rangeUtilizavel = Utils.DateDiff(DateInterval.Day, dataAtual, boleto.DataVencimento);

            if (rangeUtilizavel > 5500 || rangeUtilizavel < -3000)
                throw new Exception("Data do vencimento fora do range de utilização proposto pela CENEGESC. Comunicado FEBRABAN de n° 082/2012 de 14/06/2012");

            while (boleto.DataVencimento > dateBase.AddDays(9999))
                dateBase = boleto.DataVencimento.AddDays(-(((Utils.DateDiff(DateInterval.Day, dateBase, boleto.DataVencimento) - 9999) - 1) + 1000));

            return Utils.DateDiff(DateInterval.Day, dateBase, boleto.DataVencimento);
        }

        //protected static int Mod11(string seq, int b)
        //{
        //    /* Variáveis
        //     * -------------
        //     * d - Dígito
        //     * s - Soma
        //     * p - Peso
        //     * b - Base
        //     * r - Resto
        //     */

        //    int d, s = 0, p = 2;


        //    for (int i = seq.Length; i > 0; i--)
        //    {
        //        s = s + (Convert.ToInt32(seq.Mid(i, 1)) * p);
        //        if (p == b)
        //            p = 2;
        //        else
        //            p = p + 1;
        //    }

        //    d = 11 - (s % 11);


        //    if ((d > 9) || (d == 0) || (d == 1))
        //        d = 1;

        //    return d;
        //}

        //internal static int Mod10(string seq)
        //{
        //    /* Variáveis
        //     * -------------
        //     * d - Dígito
        //     * s - Soma
        //     * p - Peso
        //     * b - Base
        //     * r - Resto
        //     */

        //    int d, s = 0, p = 2, r;

        //    for (int i = seq.Length; i > 0; i--)
        //    {
        //        r = (Convert.ToInt32(seq.Mid(i, 1)) * p);

        //        if (r > 9)
        //            r = (r / 10) + (r % 10);

        //        s += r;

        //        if (p == 2)
        //            p = 1;
        //        else
        //            p = p + 1;
        //    }
        //    d = ((10 - (s % 10)) % 10);
        //    return d;
        //}

    }
}
