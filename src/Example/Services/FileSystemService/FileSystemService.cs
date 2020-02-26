using System.IO;

namespace Example.Services.FileSystemService
{
    public class FileSystemService : IFileSystemService
    {
        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public void WriteAllText(string path, string text)
        {
            File.WriteAllText(path, text);
        }
    }
}