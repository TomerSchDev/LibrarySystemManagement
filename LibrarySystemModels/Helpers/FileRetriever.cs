using System.IO;

namespace LibrarySystemModels.Helpers;

public static class FileRetriever
{
    private static string ProjectRoot {get; }
    static FileRetriever()
    {
        ProjectRoot = AppContext.BaseDirectory;

    }
    public static string RetrieveFIlePath(string filename)
    {
        
        return Path.Combine(ProjectRoot, filename);
    }
}
