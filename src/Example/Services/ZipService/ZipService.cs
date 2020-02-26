using System.IO.Compression;
using Example.Model;
using Example.Model.Helpers;

namespace Example.Services.ZipService
{
    public class ZipService : IZipService
    {
        public void Compress(string zipFilePath, params TempFile[] files)
        {
            using (var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (var file in files)
                {
                    zip.CreateEntryFromFile(file.FilePath, file.Name, CompressionLevel.Optimal);
                }
            }
        }
    }
}