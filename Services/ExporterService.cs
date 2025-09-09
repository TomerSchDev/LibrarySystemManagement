using System.Reflection;
using Library_System_Management.Models;
using Library_System_Management.Services.ExportServices;

namespace Library_System_Management.Services;


public static class ExporterService
{
    
    private static readonly Dictionary<string, IDataExportService> Services;

    static ExporterService()
    {
        Services = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IDataExportService).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
            .Select(t => (IDataExportService)Activator.CreateInstance(t)!)
            .ToDictionary(ex => ex.Name);
    }

    public static List<string> GetAllExportServices()
    {
        return Services.Keys.ToList();
    }
    public static void Export(IEnumerable<IExportable> data,string filePath, string exportService)
    {
        Services.GetValueOrDefault(exportService)?.Export(data,filePath);

    }
}