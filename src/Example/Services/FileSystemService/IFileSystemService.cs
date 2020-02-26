namespace Example.Services.FileSystemService
{
    public interface IFileSystemService
    {
        void DeleteFile(string path);

        void WriteAllText(string path, string text);
    }
}