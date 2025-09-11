using System.Reflection;
using LibrarySystemModels.Models;

namespace Library_System_Management.ExportServices
{
    public interface IDataExportService
    {
        string Name { get; }
        bool Export(IEnumerable<IExportable> data, string filePath);

        static PropertyInfo[] GetObjectPropitiates(IEnumerable<IExportable> data) => data.ToList().Cast<object>().FirstOrDefault()!.GetType().GetProperties();
    }

}

