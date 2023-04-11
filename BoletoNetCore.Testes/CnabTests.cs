using System;
using System.Linq;
using BoletoNetCore.Extensions;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Testes Cnab")]
    public class CnabTests
    {
        [TestCase("02", "06171915A6")]
        [TestCase("02", "0617000000")]
        [TestCase("02", "0617")]
        [TestCase("02", "0617A6")]
        [TestCase("02", "08180617A6")]
        public void Deve_possuir_motivo_ocorrencia_data_vencimento_anterior_a_data_emissao(string codMovimentoRetorno, string codMotivo)
        {
            var motivos = Cnab.MotivoOcorrenciaCnab240(codMotivo, codMovimentoRetorno).ToList();
            Assert.IsTrue(motivos.Any(q => q == "Data de Vencimento Anterior a Data de Emissão"));
        }

        [TestCase("02", "06171915A6")]
        [TestCase("02", "0617000000")]
        [TestCase("02", "0617")]
        [TestCase("02", "0617A6")]
        [TestCase("02", "08180617A6")]
        public void Deve_possuir_motivo_ocorrencia_tipo_ou_numero_de_inscricao_do_beneficiario_invalido(string codMovimentoRetorno, string codMotivo)
        {
            var motivos = Cnab.MotivoOcorrenciaCnab240(codMotivo, codMovimentoRetorno).ToList();
            Assert.IsTrue(motivos.Any(q => q == "Tipo / Número de Inscrição do Beneficiário Inválidos"));
        }

        [TestCase("02", "06171915A6")] 
        public void Deve_possuir_motivo_ocorrencia_caracteristica_da_cobranca_incompativeis(string codMovimentoRetorno, string codMotivo)
        {
            var motivos = Cnab.MotivoOcorrenciaCnab240(codMotivo, codMovimentoRetorno).ToList();
            Assert.IsTrue(motivos.Any(q => q == "Características da Cobrança Incompatíveis"));
        }

        [TestCase("02", "0617A6")]
        [TestCase("02", "08180617A6")]
        public void Deve_possuir_motivo_ocorrencia_codigo_do_convenente_invalido_ou_encerrado(string codMovimentoRetorno, string codMotivo)
        {
            var motivos = Cnab.MotivoOcorrenciaCnab240(codMotivo, codMovimentoRetorno).ToList(); 
            Assert.IsTrue(motivos.Any(q => q == "Código do Convenente Inválido ou Encerrado"));
        }

        [TestCase("02", "06171915A6")]
        public void Deve_possuir_motivo_ocorrencia_titulo_cargo_de_banco_correspondente_com_vencimento_inferior_xx_dias(string codMovimentoRetorno, string codMotivo)
        {
            var motivos = Cnab.MotivoOcorrenciaCnab240(codMotivo, codMovimentoRetorno).ToList();
            Assert.IsTrue(motivos.Any(q => q == "Título a Cargo de Bancos Correspondentes com Vencimento Inferior a XX Dias"));
        }

        [TestCase("02", "0000000000")]
        [TestCase("06", "")]
        public void Quando_nao_possuir_motivo_ocorrencia_count_deve_ser_zero(string codMovimentoRetorno, string codMotivo)
        {
            var motivos = Cnab.MotivoOcorrenciaCnab240(codMotivo, codMovimentoRetorno).ToList();
            Assert.IsTrue(motivos.Count == 0);
        }

        [TestCase("02", "0000000000")]
        [TestCase("06", "")]
        public void Quando_nao_possuir_motivo_ocorrencia_motivos_join_deve_retornar_string_vazia(string codMovimentoRetorno, string codMotivo)
        {
            var motivos = Cnab.MotivoOcorrenciaCnab240(codMotivo, codMovimentoRetorno).ToList();
            Assert.AreEqual(string.Join(",", motivos), string.Empty);
        }

        [TestCase("02", "000000001")]
        [TestCase("04", "000000002")]
        [TestCase("04", "002000002")]
        [TestCase("06", "1")]
        public void Quando_motivo_ocorrencia_for_incorreto_deve_retornar_count_zero(string codMovimentoRetorno, string codMotivo)
        {
            var motivos = Cnab.MotivoOcorrenciaCnab240(codMotivo, codMovimentoRetorno).ToList();
            Assert.IsTrue(motivos.Count == 0);
        }

        [TestCase("02", "000000001")]
        [TestCase("04", "000000002")]
        [TestCase("04", "002000002")]
        [TestCase("06", "1")]
        public void Quando_motivo_ocorrencia_for_incorreto_join_deve_retornar_string_vazia(string codMovimentoRetorno, string codMotivo)
        {
            var motivos = Cnab.MotivoOcorrenciaCnab240(codMotivo, codMovimentoRetorno).ToList();
            Assert.AreEqual(string.Join(",", motivos), string.Empty);
        }

        /// <summary>
        /// Em inconsistências parciais, a função tenta ler dentro do possível até o padrão de inconsistência encontrado
        /// </summary>
        /// <param name="codMovimentoRetorno"></param>
        /// <param name="codMotivo"></param>
        [TestCase("06", "000200002")]
        public void MotivoOcorrenciaDescricaoParcialmenteIncorretoNaoQuebra(string codMovimentoRetorno, string codMotivo)
        {
            var motivos = Cnab.MotivoOcorrenciaCnab240(codMotivo, codMovimentoRetorno).ToList();
            Assert.IsTrue(motivos.Count == 1);
        }
    }
}
