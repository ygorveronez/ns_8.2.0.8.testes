using iTextSharp.text.pdf.parser;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Utilidades.IO
{
    public class LocalFileStorage : IFileStorage
    {
        public string Combine(params string[] paths)
        {
            return System.IO.Path.Combine(paths);
        }

        public void WriteAllBytes(string path, byte[] bytes)
        {
            CreateDirectory(path);
            System.IO.File.WriteAllBytes(path, bytes);
        }
        public async Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken)
        {
            CreateDirectory(path);

            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            {
                await fileStream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
            }
        }

        public void WriteAllText(string path, string contents, System.Text.Encoding encoding = null)
        {
            CreateDirectory(path);

            if (encoding != null)
                System.IO.File.WriteAllText(path, contents, encoding);
            else
                System.IO.File.WriteAllText(path, contents);
        }
        public async Task WriteAllTextAsync(string path, string contents, System.Text.Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            CreateDirectory(path);

            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            {
                if (encoding != null)
                {
                    using (var writer = new StreamWriter(fileStream))
                        await writer.WriteAsync(contents);
                }
                else
                {
                    using (var writer = new StreamWriter(fileStream, encoding))
                        await writer.WriteAsync(contents);
                }
            }
        }

        public string ReadAllText(string path)
        {
            return System.IO.File.ReadAllText(path);
        }
        public async Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            using (var reader = new StreamReader(fileStream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public bool Exists(string path)
        {
            return System.IO.File.Exists(path);
        }
        public async Task<bool> ExistsAsync(string path, CancellationToken cancellationToken)
        {
            return await Task.Run(() => System.IO.File.Exists(path), cancellationToken);
        }

        public void DeleteIfExists(string path)
        {
            if (Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
        public async Task DeleteIfExistsAsync(string path, CancellationToken cancellationToken)
        {
            if (await ExistsAsync(path, cancellationToken))
            {
                await Task.Run(() => System.IO.File.Delete(path), cancellationToken);
            }
        }

        public void Delete(string path)
        {
            System.IO.File.Delete(path);
        }
        public async Task DeleteAsync(string path, CancellationToken cancellationToken)
        {
            await Task.Run(() => System.IO.File.Delete(path), cancellationToken);
        }

        public void Copy(string sourcePath, string destinationPath)
        {
            CreateDirectory(destinationPath);
            System.IO.File.Copy(sourcePath, destinationPath, true);
        }
        public async Task CopyAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken)
        {
            using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            using (var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            {
                await sourceStream.CopyToAsync(destinationStream, 81920, cancellationToken);
            }
        }

        public void Move(string sourcePath, string destinationPath)
        {
            CreateDirectory(destinationPath);
            System.IO.File.Move(sourcePath, destinationPath);
        }
        public async Task MoveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                CreateDirectory(destinationPath);
                System.IO.File.Move(sourcePath, destinationPath);
            }, cancellationToken);
        }

        public Stream OpenRead(string path)
        {
            return System.IO.File.OpenRead(path);
        }
        public async Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken)
        {
            return await Task.Run(() => System.IO.File.OpenRead(path), cancellationToken);
        }

        public System.DateTime GetLastWriteTime(string path)
        {
            return System.IO.File.GetLastWriteTime(path);
        }
        public async Task<System.DateTime> GetLastWriteTimeAsync(string path, CancellationToken cancellationToken)
        {
            return await Task.Run(() => System.IO.File.GetLastWriteTime(path), cancellationToken);
        }

        public byte[] ReadAllBytes(string path)
        {
            return System.IO.File.ReadAllBytes(path);
        }
        public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                var fileBytes = new byte[fileStream.Length];
                await fileStream.ReadAsync(fileBytes, 0, (int)fileStream.Length, cancellationToken);
                return fileBytes;
            }
        }

        public IEnumerable<string> ReadLines(string path)
        {
            return System.IO.File.ReadLines(path);
        }
        public async Task<IEnumerable<string>> ReadLinesAsync(string path, CancellationToken cancellationToken)
        {
            var lines = new List<string>();

            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lines.Add(line);
                }
            }

            return lines;
        }

        public void SaveStream(string path, Stream fileInputStream)
        {
            CreateDirectory(path);

            if (fileInputStream.CanRead && fileInputStream.Position != 0)
                fileInputStream.Position = 0;

            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, false))
            {
                fileInputStream.CopyTo(fileStream, 81920);
            }
        }
        public async Task SaveStreamAsync(string path, Stream fileInputStream, CancellationToken cancellationToken)
        {
            CreateDirectory(path);
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            {
                await fileInputStream.CopyToAsync(fileStream, 81920, cancellationToken);
            }
        }

        public void CreateIfNotExists(string path)
        {
            CreateDirectory(path);

            if (!System.IO.File.Exists(path) && !string.IsNullOrEmpty(System.IO.Path.GetExtension(path)))
            {
                System.IO.File.Create(path);
            }
        }
        public async Task CreateIfNotExistsAsync(string path, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                CreateDirectory(path);

                if (!System.IO.File.Exists(path) && !string.IsNullOrEmpty(System.IO.Path.GetExtension(path)))
                {
                    System.IO.File.Create(path);
                }

            }, cancellationToken);
        }

        public void WriteLine(string path, string line, System.Text.Encoding encoding = null)
        {
            CreateDirectory(path);

            if (encoding == null)
                System.IO.File.AppendAllText(path, line);
            else
                System.IO.File.AppendAllText(path, line, encoding);
        }
        public void WriteLine(string path, string[] lines, System.Text.Encoding encoding = null)
        {
            CreateDirectory(path);

            using (StreamWriter writer = encoding == null ? new StreamWriter(path, true) : new StreamWriter(path, true, encoding))
            {
                foreach (string line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }
        public async Task WriteLineAsync(string path, string line, System.Text.Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            CreateDirectory(path);

            using (StreamWriter writer = encoding == null ? new StreamWriter(path, true) : new StreamWriter(path, true, encoding))
            {
                await writer.WriteLineAsync(line);
            }
        }
        public async Task WriteLineAsync(string path, string[] lines, System.Text.Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            CreateDirectory(path);

            using (StreamWriter writer = encoding == null ? new StreamWriter(path, true) : new StreamWriter(path, true, encoding))
            {
                foreach (string line in lines)
                {
                    await writer.WriteLineAsync(line);
                }
            }
        }

        public Stream OpenWrite(string path)
        {
            CreateDirectory(path);
            return System.IO.File.OpenWrite(path);
        }
        public async Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                CreateDirectory(path);
                return System.IO.File.OpenWrite(path);
            }, cancellationToken);
        }

        public IEnumerable<string> GetFiles(string path, string searchPattern = null, SearchOption? searchOption = null)
        {
            if (!string.IsNullOrWhiteSpace(searchPattern) && searchOption.HasValue)
                return System.IO.Directory.GetFiles(path, searchPattern, searchOption.Value);

            if (!string.IsNullOrWhiteSpace(searchPattern) && !searchOption.HasValue)
                return System.IO.Directory.GetFiles(path, searchPattern);

            return System.IO.Directory.GetFiles(path);
        }
        public async Task<IEnumerable<string>> GetFilesAsync(string path, string searchPattern = null, SearchOption? searchOption = null, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                if (!string.IsNullOrWhiteSpace(searchPattern) && searchOption.HasValue)
                    return System.IO.Directory.GetFiles(path, searchPattern, searchOption.Value);

                if (!string.IsNullOrWhiteSpace(searchPattern) && !searchOption.HasValue)
                    return System.IO.Directory.GetFiles(path, searchPattern);

                return System.IO.Directory.GetFiles(path);

            }, cancellationToken);
        }

        private static void CreateDirectory(string path)
        {
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
        }

        public Stream Create(string path)
        {
            CreateDirectory(path);
            return System.IO.File.Create(path);
        }
        public async Task<Stream> CreateAsync(string path, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                CreateDirectory(path);
                return System.IO.File.Create(path);
            }, cancellationToken);
        }

        public void SaveImage(string path, System.Drawing.Image image, System.Drawing.Imaging.ImageFormat imageFormat = null)
        {
            CreateDirectory(path);

            if (imageFormat == null)
            {
                image.Save(path);
            }
            else
            {
                image.Save(path, imageFormat);
            }
        }
        public async Task SaveImageAsync(string path, System.Drawing.Image image, System.Drawing.Imaging.ImageFormat imageFormat = null, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                CreateDirectory(path);

                if (imageFormat == null)
                {
                    image.Save(path);
                }
                else
                {
                    image.Save(path, imageFormat);
                }
            }, cancellationToken);
        }
        

        public StorageFileInfo GetFileInfo(string path)
        {
            FileInfo fileInfo = new FileInfo(path);

            return new StorageFileInfo(fileInfo.Length, fileInfo.FullName, fileInfo.CreationTime);
        }

        public async Task<StorageFileInfo> GetFileInfoAsync(string path, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                FileInfo fileInfo = new FileInfo(path);

                return new StorageFileInfo(fileInfo.Length, fileInfo.FullName, fileInfo.CreationTime);

            }, cancellationToken);
        }

        public bool IsFileSaved(string path)
        {
            return System.IO.File.Exists(path) && new FileInfo(path).Length > 0;
        }

        public async Task<bool> IsFileSavedAsync(string path, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                return System.IO.File.Exists(path) && new FileInfo(path).Length > 0;
            }, cancellationToken);
        }

        public StorageType GetStorageType()
        {
            return StorageType.Local;
        }
    }
}