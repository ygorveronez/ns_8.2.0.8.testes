using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Utilidades.IO
{

    public interface IFileStorage
    {
        string Combine(params string[] paths);
        void WriteAllBytes(string path, byte[] bytes);
        Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken);
        void WriteAllText(string path, string contents, System.Text.Encoding encoding = null);
        Task WriteAllTextAsync(string path, string contents, System.Text.Encoding encoding = null, CancellationToken cancellationToken = default);
        string ReadAllText(string path);
        Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken);
        bool Exists(string path);
        Task<bool> ExistsAsync(string path, CancellationToken cancellationToken);
        void DeleteIfExists(string path);
        Task DeleteIfExistsAsync(string path, CancellationToken cancellationToken);
        void Delete(string path);
        Task DeleteAsync(string path, CancellationToken cancellationToken);
        void Copy(string sourcePath, string destinationPath);
        Task CopyAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken);
        void Move(string sourcePath, string destinationPath);
        Task MoveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken);
        Stream OpenRead(string path);
        Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken);
        Stream OpenWrite(string path);
        Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken);
        System.DateTime GetLastWriteTime(string path);
        Task<System.DateTime> GetLastWriteTimeAsync(string path, CancellationToken cancellationToken);
        byte[] ReadAllBytes(string path);
        Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken);
        IEnumerable<string> ReadLines(string path);
        Task<IEnumerable<string>> ReadLinesAsync(string path, CancellationToken cancellationToken);
        void SaveStream(string path, Stream fileInputStream);
        Task SaveStreamAsync(string path, Stream fileInputStream, CancellationToken cancellationToken);
        void CreateIfNotExists(string path);
        Task CreateIfNotExistsAsync(string path, CancellationToken cancellationToken);
        void WriteLine(string path, string line, System.Text.Encoding encoding = null);
        void WriteLine(string path, string[] lines, System.Text.Encoding encoding = null);
        Task WriteLineAsync(string path, string line, System.Text.Encoding encoding = null, CancellationToken cancellationToken = default);
        Task WriteLineAsync(string path, string[] lines, System.Text.Encoding encoding = null, CancellationToken cancellationToken = default);
        IEnumerable<string> GetFiles(string path, string searchPattern = null, SearchOption? searchOption = null);
        Task<IEnumerable<string>> GetFilesAsync(string path, string searchPattern = null, SearchOption? searchOption = null, CancellationToken cancellationToken = default);
        Stream Create(string path);
        Task<Stream> CreateAsync(string path, CancellationToken cancellationToken);
        void SaveImage(string path, System.Drawing.Image image, System.Drawing.Imaging.ImageFormat imageFormat = null);

        Task SaveImageAsync(string path, System.Drawing.Image image, System.Drawing.Imaging.ImageFormat imageFormat = null, CancellationToken cancellationToken = default);
        StorageFileInfo GetFileInfo(string path);
        Task<StorageFileInfo> GetFileInfoAsync(string path, CancellationToken cancellationToken);
        public bool IsFileSaved(string path);
        public Task<bool> IsFileSavedAsync(string path, CancellationToken cancellationToken = default);
        public StorageType GetStorageType();

    }
}