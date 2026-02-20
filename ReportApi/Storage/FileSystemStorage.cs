using System.IO;

namespace ReportApi.Storage
{
    public class FileSystemStorage : IStorage
    {
        public void SaveFile(string path, byte[] content)
        {
            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(path, content);
        }

        public void DeleteFile(string path)
        {
            if (Utilidades.IO.FileStorageService.Storage.Exists(path))
            {
                Utilidades.IO.FileStorageService.Storage.Delete(path);
            }
            else
            {
                throw new FileNotFoundException($"The file '{path}' was not found and could not be deleted.");
            }
        }

        public byte[] ReadAllContent(string path)
        {
            if (Utilidades.IO.FileStorageService.Storage.Exists(path))
            {
                return Utilidades.IO.FileStorageService.Storage.ReadAllBytes(path);
            }

            throw new FileNotFoundException($"The file '{path}' was not found.");
        }

        public bool Exists(string path)
        {
            return Utilidades.IO.FileStorageService.Storage.Exists(path);
        }
    }
}
