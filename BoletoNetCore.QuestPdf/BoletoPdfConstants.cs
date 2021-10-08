using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BoletoNetCore.QuestPdf
{
    internal static class BoletoPdfConstants
    {
        public static float BorderSize => 0.6f;
        public static TextStyle LabelStyle => TextStyle.Default.Size(6);
        public static TextStyle ColumnTitleStyle => TextStyle.Default.Size(7).Bold();
        public static TextStyle NormalFieldStyle => TextStyle.Default.Size(7);
        public static TextStyle BoldFieldStyle => TextStyle.Default.Size(7).Bold();
        public static TextStyle TitleStyle => TextStyle.Default.Size(9).Bold();
        public static TextStyle CodBancoStyle => TextStyle.Default.Size(16).Bold();
        // public static TextStyle LinhaDigitavelStyle => TextStyle.Default.FontType(Fonts.Arial).Size(10);
        public static TextStyle LinhaDigitavelStyle => TextStyle.Default.Size(9);
        public static TextStyle SubTitleStyle => TextStyle.Default.Size(8).Bold();
    }
}