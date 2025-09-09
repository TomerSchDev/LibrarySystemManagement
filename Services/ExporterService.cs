using System.IO;
using System.Reflection;
using System.Windows;
using Library_System_Management.Helpers;
using Library_System_Management.Models;
using Library_System_Management.Services.ExportServices;

namespace Library_System_Management.Services;


public static class ExporterService
{
    
    private static readonly Dictionary<string, IDataExportService> Services;
    private static readonly string ExportsDirectory = FileRetriever.RetrieveFIlePath(Path.Combine( "Exports"));
    static ExporterService()
    {
        Services = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IDataExportService).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
            .Select(t => (IDataExportService)Activator.CreateInstance(t)!)
            .ToDictionary(ex => ex.Name);
        if (!Directory.Exists(ExportsDirectory))
        {
            Directory.CreateDirectory(ExportsDirectory);
        }
    }

    public static List<string> GetAllExportServices()
    {
        return Services.Keys.ToList();
    }
    public static bool Export(IEnumerable<IExportable> data,string filePath, string exportService)
    {
        var exported = Services.GetValueOrDefault(exportService)?.Export(data, Path.Combine(ExportsDirectory, filePath));
        if (exported == null || (bool)!exported) return false;
         MessageBox.Show("Data exported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
         ReportingService.ReportEvent(SeverityLevel.INFO, "Data exported successfully by service : "+exportService);
         return true;
    }
}