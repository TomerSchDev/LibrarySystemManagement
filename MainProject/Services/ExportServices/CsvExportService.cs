using System.IO;
using System.Text;
using Library_System_Management.Models;

namespace Library_System_Management.Services.ExportServices;

public class CsvExportService : IDataExportService
{
    public string Name => "CSV";

    public bool Export(IEnumerable<IExportable> data, string filePath)
    {
        var exportables = data.ToList();
        var properties = IDataExportService.GetObjectPropitiates(exportables);
        var sb = new StringBuilder();

        // Header
        sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));

        // Data rows
        foreach (var item in exportables)
        {
            sb.AppendLine(string.Join(",", properties.Select(p => $"\"{p.GetValue(item)?.ToString()?.Replace("\"", "\"\"")}\"")));
        }

        File.WriteAllText($"{filePath}.csv", sb.ToString(), Encoding.UTF8);
        return true;

    }
}