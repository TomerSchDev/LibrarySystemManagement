using System.Windows;

namespace Library_System_Management.Helpers;

using System.IO;
using System.Text.Json;

public static class ConfigHelper
{
    private static JsonDocument _appSettingsJson;
    static ConfigHelper()
    {
        const string jsonPath = "appsettings.local.json";
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine("appsettings file not found... writing new one... user will get error trying to use it");
            File.WriteAllText(jsonPath, "{}");
        }
        
        var json = File.ReadAllText(jsonPath);
        _appSettingsJson = JsonDocument.Parse(json);
    }

    public static JsonElement? LoadConfigProperty(string key)
    {
        using var jsonDocument = _appSettingsJson;
        try
        {
            return jsonDocument.RootElement.GetProperty(key);
        }
        catch (KeyNotFoundException ex)
        {
            throw new KeyNotFoundException("Missing key in appsettings.local.json", ex);
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occured while reading appsettings.local.json", ex.Message);
            return null;
        }
    }
}
