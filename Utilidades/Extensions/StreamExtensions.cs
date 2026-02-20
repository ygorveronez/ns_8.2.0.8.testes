using System;
using System.IO;

namespace Utilidades.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ToByteArray(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static void SaveAs(this Stream stream, string filePath)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            // Abre um FileStream para o caminho do arquivo especificado
            using (Stream fileStream = Utilidades.IO.FileStorageService.Storage.OpenWrite(filePath))
            {
                // Reposiciona o Stream no início, caso não esteja
                if (stream.CanSeek)
                    stream.Position = 0;

                // Copia o conteúdo do Stream para o FileStream
                stream.CopyTo(fileStream);
            }

            if (stream.CanSeek)
                stream.Position = 0;
        }
    }
}
