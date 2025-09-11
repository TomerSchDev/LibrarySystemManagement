using System.IO;

namespace LibrarySystemModels.Helpers;

public static class FileRetriever
{
    private static string ProjectRoot {get; }
    static FileRetriever()
    {
        var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
#if DEBUG
        // Try to use the project root if in debug/dev
        var binParent = Directory.GetParent(exePath)?.Parent?.Parent?.FullName;
        if (!string.IsNullOrEmpty(binParent) && Directory.Exists(binParent))
        {
            ProjectRoot = Directory.GetParent(Directory.GetParent(binParent)!.FullName)!.FullName;
        }
        else
        {
            ProjectRoot = exePath;
        }
#else
            ProjectRoot = exePath;
#endif
    }
    public static string RetrieveFIlePath(string filename)
    {
        
        return Path.Combine(ProjectRoot, filename);
    }
}
