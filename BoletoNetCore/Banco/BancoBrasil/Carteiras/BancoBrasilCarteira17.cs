using System;
using static System.String;

namespace BoletoNetCore
{
    [CarteiraCodigo("17/019", "17/027", "17/035")]
    internal class BancoBrasilCarteira17 : ICarteira<BancoBrasil>
    {
        internal static Lazy<ICarteira<BancoBrasil>> Instance { get; } = new Lazy<ICarteira<BancoBrasil>>(() => new BancoBrasilCarteira17());

        private BancoBrasilCarteira17()
        {

        }

        public void FormataNossoNumero(Boleto boleto)
        {
            // Carteira 17 - Variação 019/027: Cliente emite o boleto
            // O nosso número não pode ser em branco.
            if (IsNullOrWhiteSpace(boleto.NossoNumero))
                boleto.NossoNumero = "";//boleto registrado não aceita nosso número somente em geração do boleto, mas em remessa CNAB é possível
                //throw new Exception("Nosso Número não informado.");

            if (boleto.Banco.Beneficiario.Codigo.Length != 7)
                throw new NotImplementedException("Não foi possível formatar o nosso número: Código do Beneficiário não tem 7 dígitos.");
            
            // Se o convênio for de 7 dígitos,
            // o nosso número deve estar formatado corretamente (com 17 dígitos e iniciando com o código do convênio),
            if (boleto.NossoNumero?.Length == 17)
            {
                if (!boleto.NossoNumero.StartsWith(boleto.Banco.Beneficiario.Codigo))
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve iniciar com \"{boleto.Banco.Beneficiario.Codigo}\" e conter 17 dígitos.");
            }
            else
            {
                // ou deve ser informado com até 10 posições (será formatado para 17 dígitos pelo Boleto.Net).
                if (boleto.NossoNumero?.Length > 10)
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve iniciar com \"{boleto.Banco.Beneficiario.Codigo}\" e conter 17 dígitos.");
                boleto.NossoNumero = $"{boleto.Banco.Beneficiario.Codigo}{boleto.NossoNumero.PadLeft(10, '0')}";
            }
            // Para convênios com 7 dígitos, não existe dígito de verificação do nosso número
            boleto.NossoNumeroDV = "";
            boleto.NossoNumeroFormatado = boleto.NossoNumero;
        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            return $"000000{boleto.NossoNumero}{boleto.Carteira}";
        }
    }
}
