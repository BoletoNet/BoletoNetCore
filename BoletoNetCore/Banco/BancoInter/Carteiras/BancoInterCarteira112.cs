using System;
using System.Linq;
using BoletoNetCore.Exceptions;
using BoletoNetCore.Extensions;
using static System.String;

namespace BoletoNetCore
{
    [CarteiraCodigo("112")]
    internal class BancoInterCarteira112 : ICarteira<BancoInter>
    {
        internal static Lazy<ICarteira<BancoInter>> Instance { get; } = new Lazy<ICarteira<BancoInter>>(() => new BancoInterCarteira112());

        private BancoInterCarteira112()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {

            // Nosso número: 10 dígitos (DV calculado pela biblioteca) ou 11 dígitos já completos (último dígito é o DV, informado pelo Inter)

            if (IsNullOrWhiteSpace(boleto.NossoNumero) || boleto.NossoNumero == "0000000000")
            {
                // Banco irá gerar Nosso Número
                boleto.NossoNumero = new String('0', 10);
                boleto.NossoNumeroDV = "0";
                boleto.NossoNumeroFormatado = boleto.NossoNumero + "-" + boleto.NossoNumeroDV;
            }
            else
            {
                var agencia = boleto.Banco.Beneficiario.ContaBancaria.Agencia.PadLeft(4, '0');
                var carteira = boleto.Carteira.PadLeft(3, '0');
                var contaBancaria = boleto.Banco.Beneficiario.ContaBancaria;

                string numeroBase;
                if (boleto.NossoNumero.Length == 11)
                {
                    numeroBase = boleto.NossoNumero.Substring(0, 10);
                    boleto.NossoNumeroDV = boleto.NossoNumero.Substring(10, 1);
                }
                else
                {
                    numeroBase = boleto.NossoNumero.PadLeft(10, '0');
                    boleto.NossoNumero = numeroBase;
                    boleto.NossoNumeroDV = (agencia + carteira + numeroBase).CalcularDVBancoInter();
                }

                boleto.NossoNumeroFormatado = $"{contaBancaria.Agencia}{contaBancaria.DigitoAgencia}/{contaBancaria.CarteiraPadrao}/{numeroBase}-{boleto.NossoNumeroDV}";
            }
            

        }

        public virtual string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var contaBancaria = boleto.Banco.Beneficiario.ContaBancaria;
            var codigoEmpresa = boleto.Banco.Beneficiario.CodigoTransmissao;

            if (codigoEmpresa is null || codigoEmpresa.Length != 7 || !codigoEmpresa.All(char.IsDigit))
                throw new InvalidOperationException("O código da empresa (CodigoTransmissao) do beneficiário Inter deve conter 7 dígitos numéricos.");

            // Nosso número de 11 dígitos já inclui o DV; nos demais casos o DV é acrescentado ao número de 10 dígitos.
            var nossoNumeroCampo = boleto.NossoNumero.Length == 11
                ? boleto.NossoNumero
                : boleto.NossoNumero + boleto.NossoNumeroDV;

            return $"{contaBancaria.Agencia}{contaBancaria.CarteiraPadrao}{codigoEmpresa}{nossoNumeroCampo}";
        }
    }
}
