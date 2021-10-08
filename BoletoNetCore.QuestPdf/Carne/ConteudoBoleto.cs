using System;
using BoletoNetCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BoletoNetCore.QuestPdf
{
    internal class ConteudoBoleto : IComponent
    {
        private Boleto boleto;
        private byte[] logo;

        public ConteudoBoleto(Boleto boleto, byte[] logo)
        {
            this.boleto = boleto;
            this.logo = logo;
        }

        public void Compose(IContainer container)
        {
            container.Stack(stack =>
            {
                stack.Item().Element(this.ComposeDadosBancoELinhaDigitavel);
                stack.Item().Row(row =>
                {
                    row.RelativeColumn().Element(this.ComposeInformacoesBoleto);
                    row.ConstantColumn(100).Element(this.ComposeValoresBoleto);
                });

                stack.Item().Row(row =>
                {
                    row.RelativeColumn().Element(this.ComposeDadosPagador);
                    row.ConstantColumn(100).Element(this.ComposeDocumentoPagador);
                });

                stack.Item().BorderTop(BoletoPdfConstants.BorderSize).Row(row =>
                {
                    row.RelativeColumn().Element(this.ComposeCodBarrasBoleto);
                    row.ConstantColumn(100).Element(this.ComposeDadosAutenticacacaoMecanicaBoleto);
                });

            });
        }

        private void ComposeDadosAutenticacacaoMecanicaBoleto(IContainer container)
        {
            container.Stack(stack =>
            {
                stack.Spacing(15);
                stack.Item().AlignCenter().Text("Autenticação Mecânica", BoletoPdfConstants.LabelStyle);
                stack.Item().AlignCenter().Text("Ficha de Compensação", BoletoPdfConstants.LabelStyle);
            });
        }

        private void ComposeCodBarrasBoleto(IContainer container)
        {
            var codbar = this.boleto.CodigoBarra.CodigoDeBarras.GerarCodBarras128(40);
            container.Image(codbar, ImageScaling.FitWidth);
        }

        private void ComposeDocumentoPagador(IContainer container)
        {
            container.Stack(stack =>
            {
                stack.Item().Text("CPF / CNPJ do Sacado", BoletoPdfConstants.LabelStyle);
                stack.Item().Text(this.boleto.Pagador.CPFCNPJ.MascararCpfCnpj(), BoletoPdfConstants.NormalFieldStyle);
                stack.Item().Text("Código de Baixa", BoletoPdfConstants.LabelStyle);
            });
        }

        private void ComposeDadosPagador(IContainer container)
        {
            container.Stack(stack =>
            {
                stack.Item().Row(row =>
                {
                    row.ConstantColumn(30).Text("Pagador", BoletoPdfConstants.LabelStyle);
                    row.RelativeColumn().Stack(stk =>
                    {
                        stk.Item().Text(this.boleto.Pagador.Nome, BoletoPdfConstants.NormalFieldStyle);
                        stk.Item().Text($"{this.boleto.Pagador.Endereco.LogradouroEndereco}, {this.boleto.Pagador.Endereco.LogradouroNumero} - {this.boleto.Pagador.Endereco.Bairro}", BoletoPdfConstants.NormalFieldStyle);
                        stk.Item().Text($"{this.boleto.Pagador.Endereco.CEP.FormatarCep()}, {this.boleto.Pagador.Endereco.Cidade} {this.boleto.Pagador.Endereco.UF}", BoletoPdfConstants.NormalFieldStyle);
                    });
                });

                stack.Item().Text("Sacador/Avalista:", BoletoPdfConstants.LabelStyle);
            });
        }

        private void ComposeInformacoesBoleto(IContainer container)
        {
            container.BorderRight(BoletoPdfConstants.BorderSize).Stack(stack =>
            {
                stack.Item().Text("Local de Pagamento", BoletoPdfConstants.LabelStyle);
                stack.Item().Text(this.boleto.Banco.Beneficiario.ContaBancaria.LocalPagamento, BoletoPdfConstants.NormalFieldStyle);
                stack.Item().BorderHorizontal(BoletoPdfConstants.BorderSize);

                stack.Item().Text("Beneficiário", BoletoPdfConstants.LabelStyle);
                stack.Item().Text($"{this.boleto.Banco.Beneficiario.Nome} - CNPJ: {this.boleto.Banco.Beneficiario.CPFCNPJ.MascararCpfCnpj()}", BoletoPdfConstants.NormalFieldStyle);
                stack.Item().BorderHorizontal(BoletoPdfConstants.BorderSize);

                stack.Item().BorderHorizontal(BoletoPdfConstants.BorderSize).Element(this.ComposeLinhaDataDocumentoBoleto);
                stack.Item().BorderHorizontal(BoletoPdfConstants.BorderSize).Element(this.ComposeLinhUsoBancoBoleto);
                stack.Item().BorderHorizontal(BoletoPdfConstants.BorderSize).Element(this.ComposeLinhaInstrucoesBoleto);
            });
        }

        private void ComposeLinhaInstrucoesBoleto(IContainer container)
        {
            container.Height(52f).Stack(stack =>
            {
                stack.Item().Text("Instruções (Texto de responsabilidade do benenficiário)", BoletoPdfConstants.LabelStyle);
                stack.Item().Text(this.boleto.MensagemInstrucoesCaixaFormatado, BoletoPdfConstants.NormalFieldStyle);
            });
        }

        private void ComposeLinhUsoBancoBoleto(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeColumn().BorderRight(BoletoPdfConstants.BorderSize).Stack(stack =>
                {
                    stack.Item().Text("Uso do Banco", BoletoPdfConstants.LabelStyle);
                    stack.Item().AlignCenter().Text("", BoletoPdfConstants.NormalFieldStyle);
                });

                row.RelativeColumn(0.5f).BorderRight(BoletoPdfConstants.BorderSize).Stack(stack =>
                {
                    stack.Item().PaddingLeft(3).Text("Carteira", BoletoPdfConstants.LabelStyle);
                    stack.Item().AlignCenter().Text(this.boleto.Banco.Beneficiario.ContaBancaria.CarteiraComVariacaoPadrao, BoletoPdfConstants.NormalFieldStyle);
                });

                row.RelativeColumn(0.5f).BorderRight(BoletoPdfConstants.BorderSize).Stack(stack =>
                {
                    stack.Item().PaddingLeft(3).Text("Espécie", BoletoPdfConstants.LabelStyle);
                    stack.Item().AlignCenter().Text(this.boleto.EspecieMoeda, BoletoPdfConstants.NormalFieldStyle);
                });

                row.RelativeColumn().BorderRight(BoletoPdfConstants.BorderSize).Stack(stack =>
                {
                    stack.Item().PaddingLeft(3).Text("Quantidade", BoletoPdfConstants.LabelStyle);
                    stack.Item().AlignCenter().Text("", BoletoPdfConstants.NormalFieldStyle);
                });

                row.RelativeColumn().BorderRight(BoletoPdfConstants.BorderSize).Stack(stack =>
                {
                    stack.Item().PaddingLeft(3).Text("Valor", BoletoPdfConstants.LabelStyle);
                    stack.Item().AlignCenter().Text("", BoletoPdfConstants.NormalFieldStyle);
                });
            });
        }

        private void ComposeLinhaDataDocumentoBoleto(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeColumn().BorderRight(BoletoPdfConstants.BorderSize).Stack(stack =>
                {
                    stack.Item().Text("Data do Documento", BoletoPdfConstants.LabelStyle);
                    stack.Item().AlignCenter().Text(this.boleto.DataEmissao.ToDateStr(), BoletoPdfConstants.NormalFieldStyle);
                });

                row.RelativeColumn().BorderRight(BoletoPdfConstants.BorderSize).Stack(stack =>
                {
                    stack.Item().PaddingLeft(3).Text("Número do Documento", BoletoPdfConstants.LabelStyle);
                    stack.Item().AlignCenter().Text(this.boleto.NumeroDocumento, BoletoPdfConstants.NormalFieldStyle);
                });

                row.RelativeColumn(0.5f).BorderRight(BoletoPdfConstants.BorderSize).Stack(st =>
                {
                    st.Item().PaddingLeft(3).Text("Espécie Doc.", BoletoPdfConstants.LabelStyle);
                    st.Item().AlignCenter().Text($"{this.boleto.EspecieDocumento}", BoletoPdfConstants.NormalFieldStyle);
                });

                row.RelativeColumn(0.5f).BorderRight(BoletoPdfConstants.BorderSize).Stack(st =>
                {
                    st.Item().PaddingLeft(3).Text("Aceite", BoletoPdfConstants.LabelStyle);
                    st.Item().AlignCenter().Text($"{this.boleto.Aceite}", BoletoPdfConstants.NormalFieldStyle);
                });

                row.RelativeColumn().BorderRight(BoletoPdfConstants.BorderSize).Stack(stack =>
                {
                    stack.Item().PaddingLeft(3).Text("Data do Processamento", BoletoPdfConstants.LabelStyle);
                    stack.Item().AlignCenter().Text(this.boleto.DataProcessamento.ToDateStr(), BoletoPdfConstants.NormalFieldStyle);
                });
            });
        }

        private void ComposeValoresBoleto(IContainer container)
        {
            container.Stack(stack =>
            {
                stack.Item().Text("Vencimento", BoletoPdfConstants.LabelStyle);
                stack.Item().AlignRight().Text(this.boleto.DataVencimento.ToDateStr(), BoletoPdfConstants.BoldFieldStyle);
                stack.Item().BorderHorizontal(BoletoPdfConstants.BorderSize);

                stack.Item().Text("Agência / Código Beneficiário", BoletoPdfConstants.LabelStyle);
                stack.Item().AlignRight().Text(this.boleto.Banco.Beneficiario.CodigoFormatado, BoletoPdfConstants.NormalFieldStyle);
                stack.Item().BorderHorizontal(BoletoPdfConstants.BorderSize);

                stack.Item().Text("Nosso Número", BoletoPdfConstants.LabelStyle);
                stack.Item().AlignRight().Text(this.boleto.NossoNumeroFormatado, BoletoPdfConstants.NormalFieldStyle);
                stack.Item().BorderHorizontal(BoletoPdfConstants.BorderSize);

                stack.Item().Text("(=) Valor do Documento", BoletoPdfConstants.LabelStyle);
                stack.Item().AlignRight().Text(this.boleto.ValorTitulo.FormatarMoeda(), BoletoPdfConstants.BoldFieldStyle);
                stack.Item().BorderHorizontal(BoletoPdfConstants.BorderSize);

                stack.Item().Text("(-) Desconto / Abatimento", BoletoPdfConstants.LabelStyle);
                stack.Item().Height(10);
                stack.Item().BorderHorizontal(BoletoPdfConstants.BorderSize);

                stack.Item().Text("(+) Mora / Multa", BoletoPdfConstants.LabelStyle);
                stack.Item().Height(10);
                stack.Item().BorderHorizontal(BoletoPdfConstants.BorderSize);

                stack.Item().Text("(=) Valor Cobrado", BoletoPdfConstants.LabelStyle);
                stack.Item().Height(10);
                stack.Item().BorderHorizontal(BoletoPdfConstants.BorderSize);
            });
        }

        private void ComposeDadosBancoELinhaDigitavel(IContainer container)
        {
            container.BorderBottom(BoletoPdfConstants.BorderSize).Row(row =>
            {
                row.ConstantColumn(75).Height(20).BorderRight(BoletoPdfConstants.BorderSize).AlignLeft().AlignBottom().Image(this.logo, ImageScaling.FitArea);
                row.ConstantColumn(55).BorderRight(BoletoPdfConstants.BorderSize).AlignBottom().AlignCenter().Text($"{this.boleto.Banco.Codigo.ToString("000")}-{this.boleto.Banco.Digito}", BoletoPdfConstants.CodBancoStyle);
                row.RelativeColumn().AlignRight().AlignBottom().Text(this.boleto.CodigoBarra.LinhaDigitavel, BoletoPdfConstants.LinhaDigitavelStyle);
            });
        }
    }
}