namespace BoletoNetCore
{
    partial class BancoSicredi : IBancoCNAB240
    {
        public string GerarDetalheRemessaCNAB240(Boleto boleto, ref int registro)
        {
            throw new System.NotImplementedException();
        }

        public string GerarHeaderLoteRemessaCNAB240(ref int numeroArquivoRemessa, ref int numeroRegistro)
        {
            throw new System.NotImplementedException();
        }

        public string GerarHeaderRemessaCNAB240(ref int numeroArquivoRemessa, ref int numeroRegistro)
        {
            var reg = new TRegistroEDI();
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 001, 0, "748", ' ')); // 001 a 003 - Código do banco na compensação "748" SCIREDI
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0004, 004, 0, "0000", '0')); // 004 a 007 - Lote de serviço "0000"
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0008, 001, 0, "0", '1'));    // 008 a 008 - Tipo de registro = "0" HEADER ARQUIVO
            reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0009, 009, 0, "", ' ')); // 009 a 017 - Uso exclusivo FEBRABAN/CNAB
            // 018 a 018 - Tipo de inscrição da empresa = "1" Pessoa Física "2" Pessoa Jurídica
        }

        public string GerarTrailerLoteRemessaCNAB240(ref int numeroArquivoRemessa, int numeroRegistroGeral, int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples, int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada, int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada, int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            throw new System.NotImplementedException();
        }

        public string GerarTrailerRemessaCNAB240(int numeroRegistroGeral, decimal valorBoletoGeral, int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples, int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada, int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada, int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            throw new System.NotImplementedException();
        }
    }
}