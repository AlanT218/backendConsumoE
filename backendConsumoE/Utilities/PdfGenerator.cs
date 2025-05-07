namespace backendConsumoE.Utilities;

using backendConsumoE.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

public static class PdfGenerator
{
    public static byte[] CreateReporteConsumoPdf(List<ConsumoReporteDto> datos)
    {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

        // calculamos el total
        var totalConsumoWh = datos.Sum(d => d.ConsumoWh);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);

                page.Header()
                    .Text("Reporte de Consumo Eléctrico")
                    .FontSize(20).Bold().AlignCenter();

                // Contenido: primero la tabla, luego el total
                page.Content().Column(column =>
                {
                    // 1) La tabla de datos
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn();
                            cols.RelativeColumn();
                            cols.ConstantColumn(80);
                            cols.ConstantColumn(100);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Electrodoméstico").Bold();
                            header.Cell().Element(CellStyle).Text("Zona").Bold();
                            header.Cell().Element(CellStyle).Text("Potencia (W)").Bold();
                            header.Cell().Element(CellStyle).Text("Consumo (Wh)").Bold();
                        });

                        foreach (var item in datos)
                        {
                            table.Cell().Element(CellStyle).Text(item.NombreElectrodomestico);
                            table.Cell().Element(CellStyle).Text(item.NombreZona);
                            table.Cell().Element(CellStyle).Text($"{item.Consumo}");
                            table.Cell().Element(CellStyle).Text($"{item.ConsumoWh:F2}");
                        }

                        IContainer CellStyle(IContainer container) =>
                            container.Padding(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                    });

                    // 2) El total bajo la tabla
                    column.Item().PaddingTop(10).AlignRight()
                          .Text($"Total Consumo (Wh): {totalConsumoWh:F2}")
                          .FontSize(14).Bold();
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }
}
