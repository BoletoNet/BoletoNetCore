using System;

namespace BoletoNetCore
{
    [CarteiraCodigo("1/A")]
    internal class BancoSicrediCarteira1 : ICarteira<BancoSicredi>
    {
        internal static Lazy<ICarteira<BancoSicredi>> Instance { get; } = new Lazy<ICarteira<BancoSicredi>>(() => new BancoSicrediCarteira1());

        private BancoSicrediCarteira1()
        {

        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            string CampoLivre = boleto.Carteira + "1" +
                boleto.NossoNumero +
                boleto.Banco.Beneficiario.ContaBancaria.Agencia +
                boleto.Banco.Beneficiario.ContaBancaria.OperacaoConta +
                boleto.Banco.Beneficiario.Codigo + "10";

            CampoLivre += Mod11(CampoLivre); 

            return CampoLivre;
        }

        public void FormataNossoNumero(Boleto boleto)
        {
            var DataDocumento = boleto.DataEmissao.ToString("yy");
            var nossoNumero = boleto.NossoNumero;

            boleto.NossoNumero = string.Format("{0}2{1}", DataDocumento, nossoNumero.PadLeft(5, '0'));

            boleto.NossoNumeroDV = Mod11(Sequencial(boleto)).ToString();
            boleto.NossoNumero = string.Concat(boleto.NossoNumero, Mod11(Sequencial(boleto)));

            boleto.NossoNumeroFormatado = string.Format("{0}/{1}-{2}", boleto.NossoNumero.Substring(0, 2), boleto.NossoNumero.Substring(2, 6), boleto.NossoNumero.Substring(8));            
        }

        public int Mod11(string seq)
        {
            /* Variáveis
             * -------------
             * d - Dígito
             * s - Soma
             * p - Peso
             * b - Base
             * r - Resto
             */

            int d, s = 0, p = 2, b = 9;

            for (int i = seq.Length - 1; i >= 0; i--)
            {
                s = s + (Convert.ToInt32(seq.Substring(i, 1)) * p);
                if (p < b)
                    p = p + 1;
                else
                    p = 2;
            }

            d = 11 - (s % 11);
            if (d > 9)
                d = 0;
            return d;
        }

        public string Sequencial(Boleto boleto)
        {
            string agencia = boleto.Banco.Beneficiario.ContaBancaria.Agencia;     //código da cooperativa de crédito/agência beneficiária (aaaa)
            string posto = boleto.Banco.Beneficiario.ContaBancaria.OperacaoConta; //código do posto beneficiário (pp)

            if (string.IsNullOrEmpty(posto))
            {
                throw new Exception($"Posto beneficiário não preenchido");
            }

            string beneficiario = boleto.Banco.Beneficiario.Codigo;                    //código do beneficiário (ccccc)
            string nossoNumero = boleto.NossoNumero;                         //ano atual (yy), indicador de geração do nosso número (b) e o número seqüencial do beneficiário (nnnnn);

            return string.Concat(agencia, posto, beneficiario, nossoNumero); // = aaaappcccccyybnnnnn
        }
    }
}
