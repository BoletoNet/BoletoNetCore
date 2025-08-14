using BoletoNetCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoletoNetCore
{
    [CarteiraCodigo("1")]
    internal class BancoCecredCarteira1 : ICarteira<BancoCecred>
    {
        internal static Lazy<ICarteira<BancoCecred>> Instance { get; } = new Lazy<ICarteira<BancoCecred>>(() => new BancoCecredCarteira1());

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            string parte1 = string.Format("{0}{1}{2}",
                 Utils.FormatCode(boleto.Banco.Beneficiario.Codigo, 6),
                 boleto.NossoNumero,
                 Utils.FormatCode(boleto.Carteira, 2));
            return parte1;
        }

        public void FormataNossoNumero(Boleto boleto)
        {
            string nossoNumero = boleto.NossoNumero;
            boleto.NossoNumeroDV = Mod11(Sequencial(boleto)).ToString();
            string conta = Utils.FormatCode(boleto.Banco.Beneficiario.ContaBancaria.Conta + boleto.Banco.Beneficiario.ContaBancaria.DigitoConta, 8);
            boleto.NossoNumero = Sequencial(boleto);
            boleto.NossoNumeroFormatado = boleto.NossoNumero;
        }
         
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

        private string Sequencial(Boleto boleto)
        {
            string conta = boleto.Banco.Beneficiario.ContaBancaria.Conta;
            string digitoConta = boleto.Banco.Beneficiario.ContaBancaria.DigitoConta;
            string contaComDigito = conta + digitoConta;
            string nossoNumero = boleto.NossoNumero;
            return string.Format("{0}{1}", Utils.FormatCode(contaComDigito, 8), Utils.FormatCode(nossoNumero, 9));
        }
    }
}
