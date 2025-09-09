using System.IO;

namespace Library_System_Management.Helpers;

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
            ProjectRoot = Directory.GetParent(binParent)!.FullName;
        }
        else
        {
            ProjectRoot = exePath;
        }
#else
            // In prod, always use the EXE directory
            ProjectRoot = exeDir;
#endif
    }
    public static string RetrieveFIlePath(string filename)
    {
        
        return Path.Combine(ProjectRoot, filename);
    }
}
