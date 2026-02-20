using System;
using System.IO;

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

        public byte[] GetBytes()
        {
            if (InputStream == null)
                return Array.Empty<byte>();

            if (InputStream.CanSeek)
                InputStream.Position = 0;

            using (var ms = new MemoryStream())
            {
                InputStream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
