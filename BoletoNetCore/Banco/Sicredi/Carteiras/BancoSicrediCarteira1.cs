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

            if (boleto.Banco.Beneficiario.ContaBancaria.OperacaoConta.Length != 2)
                throw new NotImplementedException($"Não foi possível formatar o campo livre: Codigo de Operação da conta ({boleto.Banco.Beneficiario.ContaBancaria.OperacaoConta}) não possui 2 dígitos.");

            if (boleto.Banco.Beneficiario.Codigo.Length != 5)
                throw new NotImplementedException($"Não foi possível formatar o campo livre: Conta ({boleto.Banco.Beneficiario.Codigo}) não possui 5 dígitos.");

            if (boleto.Banco.Beneficiario.ContaBancaria.Agencia.Length != 4)
                throw new NotImplementedException($"Não foi possível formatar o campo livre: Numero de Agencia ({boleto.Banco.Beneficiario.ContaBancaria.Agencia}) não possui 4 dígitos.");

            string CampoLivre = boleto.Carteira + "1" +
                boleto.NossoNumero + boleto.NossoNumeroDV +
                boleto.Banco.Beneficiario.ContaBancaria.Agencia +
                boleto.Banco.Beneficiario.ContaBancaria.OperacaoConta +
                boleto.Banco.Beneficiario.Codigo + "10";

            CampoLivre += Mod11(CampoLivre);

            return CampoLivre;
        }

        public void FormataNossoNumero(Boleto boleto)
        {
            if (string.IsNullOrWhiteSpace(boleto.NossoNumero))
                throw new Exception("Nosso Número não informado.");

            //Formato Nosso Número
            //AA/BXXXXX-D, onde:
            //AA = Ano(pode ser diferente do ano corrente)
            //B = Byte de geração(0 a 9). O Byte 1 só poderá ser informado pela Cooperativa
            //XXXXX = Número livre de 00000 a 99999
            //D = Dígito verificador pelo módulo 11
            
            // Nosso número não pode ter mais de 8 dígitos
            if (boleto.NossoNumero.Length == 7 || boleto.NossoNumero.Length > 8)
                throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve conter até 5 dígitos ou exatamente 6 ou 8 dígitos.");
            else if (boleto.NossoNumero.Length <= 5)
            {
                boleto.NossoNumero = string.Format("{0}{1}{2}", boleto.DataEmissao.ToString("yy"), boleto.ByteNossoNumero, boleto.NossoNumero.PadLeft(5, '0'));
            }
            else if (boleto.NossoNumero.Length == 6)
            {
                if (boleto.NossoNumero.Substring(0, 1) == "1")
                {
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) de 6 dígitos não pode começar com 1 (Reservado para Cooperativa).");
                }
                boleto.NossoNumero = string.Format("{0}{1}", boleto.DataEmissao.ToString("yy"), boleto.NossoNumero);
            }
            else
            {
                if (boleto.NossoNumero.Substring(2, 1) == "1")
                {
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) de 8 dígitos não pode ter o Byte (3a. posição) como 1 (Reservado para Cooperativa).");
                }
            }

            boleto.NossoNumeroDV = Mod11(Sequencial(boleto)).ToString();

            boleto.NossoNumeroFormatado = string.Format("{0}/{1}-{2}", boleto.NossoNumero.Substring(0, 2), boleto.NossoNumero.Substring(2, 6), boleto.NossoNumeroDV);
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
