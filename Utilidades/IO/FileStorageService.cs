using System;

namespace Utilidades.IO
{
    public static class FileStorageService
    {
        #region Instances

        private static IFileStorage _currentFileStorageInstance;
        private static readonly Lazy<IFileStorage> _localFileStorageInstance = new Lazy<IFileStorage>(() => new LocalFileStorage());

        private static readonly object _lock = new object();

        #endregion

        #region Public Properties

        // Instância dinâmica baseada na configuração
        public static IFileStorage Storage
        {
            get
            {
                lock (_lock)
                {
                    return _currentFileStorageInstance ??= CreateInstance();
                }
            }
        }

        // Instância fixa do LocalFileStorage
        public static IFileStorage LocalStorage => _localFileStorageInstance.Value;

        #endregion

        #region Private Properties

        private static StorageType _storageType;
        private static string _connectionString;
        private static string _containerName;

        #endregion

        #region Public Properties

        public static StorageType StorageType => _storageType;

        #endregion

        #region Public Methods

        public static void ConfigureWithAzureDefault(string connectionString, string containerName)
        {
            lock (_lock)
            {
                _connectionString = connectionString;
                _containerName = containerName;
                _storageType = StorageType.Azure;

                _currentFileStorageInstance = CreateInstance();
            }
        }

        public static void ConfigureWithLocalDefault()
        {
            lock (_lock)
            {
                _storageType = StorageType.Local;
                
                _currentFileStorageInstance = CreateInstance();
            }
        }

        #endregion

        #region Private Methods

        private static IFileStorage CreateInstance()
        {
            return _storageType switch
            {
                StorageType.Azure => new AzureFileStorage(_connectionString, _containerName),
                _ => LocalStorage
            };
        }

        #endregion
    }
}