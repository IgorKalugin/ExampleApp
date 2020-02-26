using Example.Model;
using Example.Model.Helpers;

namespace Example.Services.ZipService
{
    public interface IZipService
    {
        void Compress(string originalFilePath, params TempFile[] files);
    }
}