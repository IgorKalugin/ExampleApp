using System;
using Example.Services.FileSystemService;

namespace Example.Model.Helpers
{
    public class TempFile : IDisposable
    {
        private readonly IFileSystemService fileSystemService;
        
        public TempFile(IFileSystemService fileSystemService, string filePath, string name)
        {
            this.fileSystemService = fileSystemService;
            FilePath = filePath;
            Name = name;
        }
        
        public string FilePath { get; }
        
        public string Name { get; }
        
        public void Dispose()
        {
            fileSystemService.DeleteFile(FilePath);
        }
    }
}