using System;

namespace BoletoNetCore.Exceptions
{
    public sealed class BoletoNetCoreException : Exception
    {
        private BoletoNetCoreException(string message)
            : base(message)
        {

        }

        private BoletoNetCoreException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public static BoletoNetCoreException BancoNaoImplementado(int codigoBanco)
            => new BoletoNetCoreException($"Banco não implementando: {codigoBanco}");

        public static BoletoNetCoreException ErroAoFormatarBeneficiario(Exception ex)
            => new BoletoNetCoreException("Erro durante a formatação do beneficiário.", ex);

        public static BoletoNetCoreException ErroAoFormatarCodigoDeBarra(Exception ex)
            => new BoletoNetCoreException("Erro durante a formatação do código de barra.", ex);

        public static Exception ErroAoFormatarNossoNumero(Exception ex)
            => new BoletoNetCoreException("Erro durante a formatação do nosso número.", ex);

        public static Exception ErroAoValidarBoleto(Exception ex)
            => new BoletoNetCoreException("Erro durante a validação do boleto.", ex);

        public static Exception ErroAoGerarRegistroHeaderDoArquivoRemessa(Exception ex)
            => new BoletoNetCoreException("Erro durante a geração do registro HEADER do arquivo de REMESSA.", ex);

        public static Exception ErroAoGerarRegistroDetalheDoArquivoRemessa(Exception ex)
            => new BoletoNetCoreException("Erro durante a geração dos registros de DETALHE do arquivo de REMESSA.", ex);

        public static Exception ErroAoGerrarRegistroTrailerDoArquivoRemessa(Exception ex)
            => new BoletoNetCoreException("Erro durante a geração do registro TRAILER do arquivo de REMESSA.", ex);

        public static Exception AgenciaInvalida(string agencia, int digitos)
            => new BoletoNetCoreException($"O número da agência ({agencia}) deve conter {digitos} dígitos.");

        public static Exception ContaInvalida(string conta, int digitos)
            => new BoletoNetCoreException($"O número da conta ({conta}) deve conter {digitos} dígitos.");

        public static Exception CodigoBeneficiarioInvalido(string codigoBeneficiario, int digitos)
            => new BoletoNetCoreException($"O código do beneficiário ({codigoBeneficiario}) deve conter {digitos} dígitos.");

        public static Exception CodigoBeneficiarioInvalido(string codigoBeneficiario, string digitos)
            => new BoletoNetCoreException($"O código do beneficiário ({codigoBeneficiario}) deve conter {digitos} dígitos.");

        public static Exception CarteiraNaoImplementada(string carteiraComVariacao)
            => new BoletoNetCoreException($"Carteira não implementada: {carteiraComVariacao}");

        public static Exception NumeroSequencialInvalido(int numeroSequencial)
            => new BoletoNetCoreException($"Número sequencial é inválido: {numeroSequencial}");
    }
}
