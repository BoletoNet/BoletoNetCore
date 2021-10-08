using BoletoNetCore;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using System;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace BoletoNetCore.QuestPdf
{
    internal class BoletoCarne : IDocument
    {
        private BoletoNetCore.Boletos listaBoletos;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.MarginHorizontal(20);
                page.MarginVertical(20);
                page.Content().Element(this.ComposeContent);
            });
        }

        private byte[] ObterLogoBanco(IBanco banco)
        {
            var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
            using (var reader = embeddedProvider.GetFileInfo($"logos/{banco.Codigo.ToString("000")}.bmp").CreateReadStream())
            {
                var logo = new byte[reader.Length];
                reader.Read(logo, 0, (int)reader.Length);
                return logo;
            }
        }

        private void ComposeContent(IContainer container)
        {
            container.Stack(stack =>
            {
                byte[] logo = null;
                int codBanco = 0;
                foreach (var bol in this.listaBoletos)
                {
                    if (logo == null || codBanco != bol.Banco.Codigo)
                    {
                        codBanco = bol.Banco.Codigo;
                        logo = this.ObterLogoBanco(bol.Banco);
                    }

                    stack.Item().Row(row =>
                    {
                        row.ConstantColumn(100).Component(new ReciboLateralCarne(bol, logo));
                        row.RelativeColumn().PaddingLeft(5).Component(new ConteudoBoleto(bol, logo));
                    });

                    stack.Item().PaddingBottom(3).Text("Recibo do Pagador - Autenticar no Verso", BoletoPdfConstants.LabelStyle);
                    stack.Item().ExtendHorizontal().BorderHorizontal(BoletoPdfConstants.BorderSize);
                    stack.Item().Height(15).ExtendHorizontal();
                }
            });
        }

        public DocumentMetadata GetMetadata()
        {
            return DocumentMetadata.Default;
        }

        public byte[] BoletoPdf(BoletoNetCore.Boletos listaBoletos)
        {
            this.listaBoletos = listaBoletos;
            return this.GeneratePdf();
        }
    }
}