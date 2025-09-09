using System.Windows;

namespace Library_System_Management.Helpers;

using System.IO;
using System.Text.Json;

public static class ConfigHelper
{
    private static readonly JsonDocument  _doc;
    static ConfigHelper()
    {
        var jsonPath = FileRetriever.RetrieveFIlePath( "appsettings.local.json");
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine("appsettings file not found... writing new one... user will get error trying to use it");
            File.WriteAllText(jsonPath, "{}");
        }
        
        var json = File.ReadAllText(jsonPath);
        _doc = JsonDocument.Parse(json);
    }

    private static JsonElement Root => _doc.RootElement;
    public static string? GetString(string path)
    {
        var keys = path.Split(':');
        var current = keys.Aggregate(Root, (current1, k) => current1.GetProperty(k));
        return current.ValueKind switch
        {
            JsonValueKind.String => current.GetString(),
            JsonValueKind.Number => current.GetRawText() // returns number as string, e.g. "587"
            ,
            JsonValueKind.True or JsonValueKind.False => current.GetBoolean().ToString(),
            _ => current.ToString()
        };
    }

    // Or return a section for more advanced lookup
    public static JsonElement GetSection(string section)
    {
        return Root.GetProperty(section);
    }
}
