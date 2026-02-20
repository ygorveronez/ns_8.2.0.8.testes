using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Utilidades.IO
{
    public class AzureFileStorage : IFileStorage
    {
        private readonly BlobContainerClient _blobContainerClient;

        public AzureFileStorage(string connectionString, string containerName)
        {
            _blobContainerClient = new BlobContainerClient(connectionString, containerName);
        }

        public string Combine(params string[] paths)
        {
            return string.Join("/", paths.Select(p => p.Trim('/')));
        }

        public void Copy(string sourcePath, string destinationPath)
        {
            var sourceBlobClient = _blobContainerClient.GetBlobClient(sourcePath);
            var destinationBlobClient = _blobContainerClient.GetBlobClient(destinationPath);

            destinationBlobClient.StartCopyFromUri(sourceBlobClient.Uri);

            while (destinationBlobClient.GetProperties().Value.CopyStatus == Azure.Storage.Blobs.Models.CopyStatus.Pending)
            {
                Thread.Sleep(100);
            }

            if (destinationBlobClient.GetProperties().Value.CopyStatus != Azure.Storage.Blobs.Models.CopyStatus.Success)
            {
                throw new Exception("Copy failed");
            }
        }
        public async Task CopyAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken)
        {
            var sourceBlobClient = _blobContainerClient.GetBlobClient(sourcePath);
            var destinationBlobClient = _blobContainerClient.GetBlobClient(destinationPath);

            await destinationBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri, null, cancellationToken);

            while ((await destinationBlobClient.GetPropertiesAsync(null, cancellationToken)).Value.CopyStatus == Azure.Storage.Blobs.Models.CopyStatus.Pending)
            {
                Thread.Sleep(100);
            }

            if ((await destinationBlobClient.GetPropertiesAsync(null, cancellationToken)).Value.CopyStatus != Azure.Storage.Blobs.Models.CopyStatus.Success)
            {
                throw new Exception("Copy failed");
            }
        }

        public void Delete(string path)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            blobClient.Delete();
        }
        public async Task DeleteAsync(string path, CancellationToken cancellationToken)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            await blobClient.DeleteAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.None, null, cancellationToken);
        }

        public void DeleteIfExists(string path)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            blobClient.DeleteIfExists();
        }
        public async Task DeleteIfExistsAsync(string path, CancellationToken cancellationToken)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            await blobClient.DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.None, null, cancellationToken);
        }

        public bool Exists(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            return blobClient.Exists();
        }
        public async Task<bool> ExistsAsync(string path, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            return await blobClient.ExistsAsync(cancellationToken);
        }

        public System.DateTime GetLastWriteTime(string path)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            return blobClient.GetProperties().Value.LastModified.DateTime;
        }
        public async Task<System.DateTime> GetLastWriteTimeAsync(string path, CancellationToken cancellationToken)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            return (await blobClient.GetPropertiesAsync(null, cancellationToken)).Value.LastModified.DateTime;
        }

        public byte[] ReadAllBytes(string path)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            using (var stream = blobClient.OpenRead())
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
        public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            using (var stream = await blobClient.OpenReadAsync(null, cancellationToken))
            {
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream, 81920, cancellationToken);
                    return memoryStream.ToArray();
                }
            }
        }

        public IEnumerable<string> ReadLines(string path)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            using (var stream = blobClient.OpenRead())
            {
                using (var reader = new StreamReader(stream))
                {
                    var lines = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        lines.Add(reader.ReadLine());
                    }

                    return lines;
                }
            }
        }
        public async Task<IEnumerable<string>> ReadLinesAsync(string path, CancellationToken cancellationToken)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            using (var stream = await blobClient.OpenReadAsync(null, cancellationToken))
            {
                using (var reader = new StreamReader(stream))
                {
                    var lines = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        lines.Add(await reader.ReadLineAsync());
                    }

                    return lines;
                }
            }
        }

        public void SaveStream(string path, Stream fileInputStream)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            blobClient.Upload(fileInputStream);
        }
        public async Task SaveStreamAsync(string path, Stream fileInputStream, CancellationToken cancellationToken)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            await blobClient.UploadAsync(fileInputStream, cancellationToken);
        }

        public void CreateIfNotExists(string path)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            using (var stream = blobClient.OpenWrite(true))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine();
                }
            }
        }
        public async Task CreateIfNotExistsAsync(string path, CancellationToken cancellationToken)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            using (var stream = await blobClient.OpenWriteAsync(true, null, cancellationToken))
            {
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteLineAsync();
                }
            }
        }

        public void WriteLine(string path, string line, System.Text.Encoding encoding = null)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            using (var stream = blobClient.OpenWrite(true))
            {
                using (var writer = encoding == null ? new StreamWriter(stream) : new StreamWriter(stream, encoding))
                {
                    writer.WriteLine(line);
                }
            }
        }
        public void WriteLine(string path, string[] lines, System.Text.Encoding encoding = null)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            using (var stream = blobClient.OpenWrite(true))
            {
                using (var writer = encoding == null ? new StreamWriter(stream) : new StreamWriter(stream, encoding))
                {
                    foreach (string line in lines)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
        }
        public async Task WriteLineAsync(string path, string line, System.Text.Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            using (var stream = await blobClient.OpenWriteAsync(overwrite: true, cancellationToken: cancellationToken))
            {
                using (var writer = encoding == null ? new StreamWriter(stream) : new StreamWriter(stream, encoding))
                {
                    await writer.WriteLineAsync(line);
                }
            }
        }
        public async Task WriteLineAsync(string path, string[] lines, System.Text.Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            using (var stream = await blobClient.OpenWriteAsync(true, null, cancellationToken))
            {
                using (var writer = encoding == null ? new StreamWriter(stream) : new StreamWriter(stream, encoding))
                {
                    foreach (string line in lines)
                    {
                        await writer.WriteLineAsync(line);
                    }
                }
            }
        }

        public void Move(string sourcePath, string destinationPath)
        {
            Copy(sourcePath, destinationPath);
            Delete(sourcePath);
        }
        public async Task MoveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken)
        {
            await CopyAsync(sourcePath, destinationPath, cancellationToken);
            await DeleteAsync(sourcePath, cancellationToken);
        }

        public Stream OpenRead(string path)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            return blobClient.OpenRead();
        }
        public async Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            return await blobClient.OpenReadAsync(null, cancellationToken);
        }

        public string ReadAllText(string path)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            using (var stream = blobClient.OpenRead())
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        public async Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            using (var stream = await blobClient.OpenReadAsync(null, cancellationToken))
            {
                using (var reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        public void WriteAllBytes(string path, byte[] bytes)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            blobClient.Upload(new MemoryStream(bytes));
        }
        public async Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            await blobClient.UploadAsync(new MemoryStream(bytes), cancellationToken);
        }

        public void WriteAllText(string path, string contents, System.Text.Encoding encoding = null)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));

            using (var memoryStream = encoding == null ? new MemoryStream(System.Text.Encoding.UTF8.GetBytes(contents)) : new MemoryStream(encoding.GetBytes(contents)))
                blobClient.Upload(memoryStream);
        }
        public async Task WriteAllTextAsync(string path, string contents, System.Text.Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));

            using (var memoryStream = encoding == null ? new MemoryStream(System.Text.Encoding.UTF8.GetBytes(contents)) : new MemoryStream(encoding.GetBytes(contents)))
                await blobClient.UploadAsync(memoryStream, cancellationToken);
        }

        public Stream OpenWrite(string path)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            return blobClient.OpenWrite(overwrite: true);
        }
        public async Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));
            return await blobClient.OpenWriteAsync(overwrite: true, options: null, cancellationToken: cancellationToken);
        }

        public IEnumerable<string> GetFiles(string path, string searchPattern = null, SearchOption? searchOption = null)
        {
            return GetAzureBlobFiles(path, searchPattern, searchOption).Result;
        }
        public async Task<IEnumerable<string>> GetFilesAsync(string path, string searchPattern = null, SearchOption? searchOption = null, CancellationToken cancellationToken = default)
        {
            return await GetAzureBlobFiles(path, searchPattern, searchOption, cancellationToken);
        }

        public Stream Create(string path)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));

            using (var emptyStream = new MemoryStream())
            {
                blobClient.Upload(emptyStream, overwrite: true);
            }

            return blobClient.OpenWrite(overwrite: true);
        }
        public async Task<Stream> CreateAsync(string path, CancellationToken cancellationToken)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));

            using (var emptyStream = new MemoryStream())
            {
                await blobClient.UploadAsync(emptyStream, overwrite: true, cancellationToken: cancellationToken);
            }

            return await blobClient.OpenWriteAsync(overwrite: true, cancellationToken: cancellationToken);
        }

        public void SaveImage(string path, System.Drawing.Image image, ImageFormat imageFormat = null)
        {
            using (System.IO.MemoryStream memoryStream = new MemoryStream())
            {
                if (imageFormat != null)
                    image.Save(memoryStream, imageFormat);
                else
                    image.Save(memoryStream, image.RawFormat);

                SaveStream(path, memoryStream);
            }
        }
        


        public async Task SaveImageAsync(string path, System.Drawing.Image image, ImageFormat imageFormat = null, CancellationToken cancellationToken = default)
        {
            using (System.IO.MemoryStream memoryStream = new MemoryStream())
            {
                if (imageFormat != null)
                    image.Save(memoryStream, imageFormat);
                else
                    image.Save(memoryStream, image.RawFormat);

                await SaveStreamAsync(path, memoryStream, cancellationToken);
            }
        }

        public StorageFileInfo GetFileInfo(string path)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));

            var properties = blobClient.GetProperties().Value;

            return new StorageFileInfo(properties.ContentLength, path, properties.CreatedOn.LocalDateTime);
        }

        public async Task<StorageFileInfo> GetFileInfoAsync(string path, CancellationToken cancellationToken)
        {
            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));

            var properties = (await blobClient.GetPropertiesAsync()).Value;

            return new StorageFileInfo(properties.ContentLength, path, properties.CreatedOn.LocalDateTime);
        }
        public bool IsFileSaved(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));

            if (!blobClient.Exists())
                return false;

            var properties = blobClient.GetProperties().Value;
            return properties.ContentLength > 0;
        }

        public async Task<bool> IsFileSavedAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    return false;

                var blobClient = _blobContainerClient.GetBlobClient(NormalizarPath(path));

                var existsResponse = await blobClient.ExistsAsync(cancellationToken);
                if (!existsResponse.Value)
                    return false;

                var propertiesResponse = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
                return propertiesResponse.Value.ContentLength > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region Private Methods

        private static string NormalizarPath(string caminho)
        {
            if (string.IsNullOrWhiteSpace(caminho))
                return string.Empty;

            caminho = caminho.Replace('/', '\\');
            while (caminho.Contains("\\\\"))
                caminho = caminho.Replace("\\\\", "\\");

            return caminho;

            
        }



        private bool MatchesPattern(string fileName, string searchPattern)
        {
            string regexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(searchPattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".") + "$";

            return System.Text.RegularExpressions.Regex.IsMatch(System.IO.Path.GetFileName(fileName), regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        private bool IsImmediateChild(string blobName, string prefix)
        {
            // Normaliza o prefixo para garantir que termine com uma barra
            if (!prefix.EndsWith("/"))
                prefix += "/";

            // Verifica se o blobName começa com o prefixo
            if (!blobName.StartsWith(prefix))
                return false;

            // Obtém a parte do caminho restante após o prefixo
            string remainingPath = blobName.Substring(prefix.Length);

            // Verifica se a parte restante contém '/' (indicando subdiretórios)
            return !remainingPath.Contains("/");
        }

        private async Task<IEnumerable<string>> GetAzureBlobFiles(string path, string searchPattern = null, SearchOption? searchOption = null, CancellationToken cancellationToken = default)
        {
            string prefix = path ?? "";
            bool recursive = searchOption == SearchOption.AllDirectories;

            var result = new List<string>();

            await foreach (var blobItem in _blobContainerClient.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken))
            {
                // Filtro pelo padrão de busca
                if (!string.IsNullOrEmpty(searchPattern) && !MatchesPattern(blobItem.Name, searchPattern))
                {
                    continue;
                }

                // Se não for recursivo, filtrar apenas os filhos diretos
                if (!recursive && !IsImmediateChild(blobItem.Name, prefix))
                {
                    continue;
                }

                result.Add(blobItem.Name);
            }

            return result;
        }

        public StorageType GetStorageType()
        {
            return StorageType.Azure;
        }

        #endregion
    }
}