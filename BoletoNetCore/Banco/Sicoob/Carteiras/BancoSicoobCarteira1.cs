using System;
using System.Runtime.InteropServices;
using BoletoNetCore.Extensions;
using static System.String;

namespace BoletoNetCore
{
    [CarteiraCodigo("1/01")]
    internal class BancoSicoobCarteira1: ICarteira<BancoSicoob>
    {
        internal static Lazy<ICarteira<BancoSicoob>> Instance { get; } = new Lazy<ICarteira<BancoSicoob>>(() => new BancoSicoobCarteira1());

        private BancoSicoobCarteira1()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {
            var beneficiario = boleto.Banco.Beneficiario;
            if (beneficiario.ContaBancaria.TipoImpressaoBoleto == TipoImpressaoBoleto.Empresa & boleto.NossoNumero == Empty)
                throw new Exception("Nosso Número não informado.");

            // Nao informou nosso numero
            if (string.IsNullOrEmpty(boleto.NossoNumero))
                throw new Exception("Nosso número não informado");

            // Nosso número não pode ter mais de 7 dígitos
            if (boleto.NossoNumero.Length > 7)
                throw new Exception("Nosso Número (" + boleto.NossoNumero + ") deve conter 7 dígitos.");

            boleto.NossoNumero = boleto.NossoNumero.PadLeft(7, '0');

            // Base para calcular DV: Agencia (4 caracteres) Código do Beneficiário com dígito (10 caracteres) Nosso Número (7 caracteres)
            var baseCalculoDV = $"{beneficiario.ContaBancaria.Agencia}{beneficiario.Codigo.PadLeft(9, '0')}{beneficiario.CodigoDV}{boleto.NossoNumero}";
            boleto.NossoNumeroDV = baseCalculoDV.CalcularDVSicoob();
            boleto.NossoNumeroFormatado = $"{boleto.NossoNumero}-{boleto.NossoNumeroDV}";
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var beneficiario = boleto.Banco.Beneficiario;
            var contaBancaria = beneficiario.ContaBancaria;

            if (contaBancaria.Agencia.Length != 4)
                throw new NotImplementedException($"Não foi possível formatar o campo livre: Número da agência ({contaBancaria.Agencia}) não possui 4 dígitos.");

            return $"{boleto.Carteira}{contaBancaria.Agencia}{boleto.VariacaoCarteira}{beneficiario.Codigo}{beneficiario.CodigoDV}{boleto.NossoNumero}{boleto.NossoNumeroDV}001";
        }
    }
}
