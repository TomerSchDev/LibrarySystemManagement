using System.IO;
using System.Text;
using LibrarySystemModels.Models;

namespace Library_System_Management.ExportServices;

public class CsvExportService : IDataExportService
{
    private IDataExportService _dataExportServiceImplementation = null!;
    public string Name => "CSV";
   

    public bool Export(IEnumerable<IExportable> data, string filePath)
    {
        var exportable = data.ToList();
        var properties = IDataExportService.GetObjectPropitiates(exportable);
        var sb = new StringBuilder();

        // Header
        sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));

        // Data rows
        foreach (var item in exportable)
        {
            sb.AppendLine(string.Join(",", properties.Select(p => $"\"{p.GetValue(item)?.ToString()?.Replace("\"", "\"\"")}\"")));
        }

        File.WriteAllText($"{filePath}.csv", sb.ToString(), Encoding.UTF8);
        return true;

    }
}