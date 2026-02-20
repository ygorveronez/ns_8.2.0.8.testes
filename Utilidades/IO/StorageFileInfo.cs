
namespace Utilidades.IO
{
    public class StorageFileInfo
    {
        public StorageFileInfo(long size, string path, System.DateTime createdOn)
        {
            Size = size;
            Path = path;
            CreatedOn = createdOn;
        }

        /// <summary>
        /// The size of the file in bytes.
        /// </summary>
        public long Size { get; }
        public string Path { get; }
        public System.DateTime CreatedOn { get; }
    }
}
