using LibrarySystemModels.Models;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace Library_System_Management.ExportServices;

public class PdfExportService : IDataExportService
{
    public string Name => "PDF";
    public bool Export(IEnumerable<IExportable> data, string filePath)
    {
        // Example using PdfSharpCore
        var document = new PdfDocument();
        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);
        var font = new XFont("Verdana", 10);
        var y = 40;

        var exportables = data as IExportable[] ?? data.ToArray();
        var properties = IDataExportService.GetObjectPropitiates(exportables);

        // Header
        gfx.DrawString(string.Join(" | ", properties.Select(p => p.Name)), font, XBrushes.Black, new XRect(0, y, page.Width, 0));
        y += 20;

        // Rows
        foreach (var item in exportables)
        {
            var line = string.Join(" | ", properties.Select(p => p.GetValue(item)?.ToString() ?? ""));
            gfx.DrawString(line, font, XBrushes.Black, new XRect(0, y, page.Width, 0));
            y += 20;
        }

        document.Save($"{filePath}.pdf");
        return true;
    }
}
