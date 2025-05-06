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

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.Header().Text("Reporte de Consumo Eléctrico").FontSize(20).Bold().AlignCenter();
                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(); // Electrodoméstico
                        columns.RelativeColumn(); // Zona
                        columns.ConstantColumn(80); // Voltaje
                        columns.ConstantColumn(100); // Consumo
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Electrodoméstico").Bold();
                        header.Cell().Element(CellStyle).Text("Zona").Bold();
                        header.Cell().Element(CellStyle).Text("Voltaje (W)").Bold();
                        header.Cell().Element(CellStyle).Text("Consumo (Wh)").Bold();
                    });

                    // Body
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
