using System.IO;
using Utilidades.Extensions;

namespace Servicos.DTO
{
    public class CustomFile
    {
        public CustomFile(string key, string fileName, string contentType, long length, Stream inputStream)
        {
            Key = key;
            FileName = fileName;
            ContentType = contentType;
            Length = length;
            InputStream = inputStream;
        }

        public string Key { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Length { get; set; }
        public Stream InputStream { get; set; }

        public byte[] GetBytes() => InputStream.ToByteArray();
        public void SaveAs(string filePath) => Utilidades.IO.FileStorageService.Storage.SaveStream(filePath, InputStream);
        public bool IsFileSaved(string filePath) => Utilidades.IO.FileStorageService.Storage.IsFileSaved(filePath);

    }
}
