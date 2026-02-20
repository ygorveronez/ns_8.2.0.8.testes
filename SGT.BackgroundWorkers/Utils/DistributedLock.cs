using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dominio.Excecoes.Embarcador;
using Microsoft.Extensions.Logging;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace SGT.BackgroundWorkers.Utils
{

    public class DistributedLock : IDistributedLock
    {
        private readonly ILogger<DistributedLock> _logger;
        private readonly RedLockFactory _redLockFactory;
        private readonly ConcurrentDictionary<string, IRedLock> _locks = new ConcurrentDictionary<string, IRedLock>();
        
        public DistributedLock(ILogger<DistributedLock> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            string stringConexao = string.Empty;
#if DEBUG
            stringConexao = ObterRedisConnectionStringDebug("RedisStringConexao");
#else
            stringConexao = Servicos.Database.ConnectionString.Instance.GetRedisConnectionString("RedisStringConexao");
#endif

            if (!string.IsNullOrEmpty(stringConexao))
            {
                var connectionMultiplexer = ConnectionMultiplexer.Connect(stringConexao);

                var multiplexers = new List<RedLockMultiplexer>
            {
                connectionMultiplexer
            };

                _redLockFactory = RedLockFactory.Create(multiplexers);
            }
            else
            {
                _redLockFactory = null;
            }
        }

        public async Task<bool> LockAsync(string key, TimeSpan expiry, CancellationToken cancellationToken)
        {
            string resource = GetLockKey(key);
            TimeSpan waitTime = expiry; // Total time to wait to acquire the lock
            TimeSpan retryTime = TimeSpan.FromMilliseconds(200); // Time between retries

            try
            {
                if(_redLockFactory == null)
                    return true;

                // Attempt to acquire the lock within the waitTime
                var redLock = await _redLockFactory.CreateLockAsync(resource, expiry, waitTime, retryTime, cancellationToken);

                if (redLock.IsAcquired)
                {
                    // Store the lock for later release
                    _locks[key] = redLock;
                    _logger.LogInformation("üîí Lock acquired for key: {Key}", key);
                    return true;
                }
                else
                {
                    // Dispose of the lock since it was not acquired
                    await redLock.DisposeAsync();
                    _logger.LogWarning("Failed to acquire lock for key: {Key}", key);
                    return false;
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Lock acquisition for key: {Key} was canceled.", key);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to acquire the lock for key: {Key}", key);
                throw;
            }
        }

        public async Task UnlockAsync(string key, CancellationToken cancellationToken)
        {
            if (_locks.TryRemove(key, out var redLock))
            {
                await redLock.DisposeAsync();
                _logger.LogInformation("üîì Lock released for key: {Key}", key);
            }
            else
            {
                _logger.LogWarning("Attempted to release a lock that does not exist for key: {Key}", key);
            }
        }

        private string GetLockKey(string key) => $"Lock_{key}";

        private string ObterRedisConnectionStringDebug(string connectionDefault)
        {
            List<string> arquivo = ObterArquivoURLDebug();

            arquivo.RemoveAt(0);

            string json = string.Join("", arquivo);

            if (string.IsNullOrWhiteSpace(json))
                return connectionDefault;

            dynamic configDebug = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

            return !string.IsNullOrWhiteSpace((string)configDebug.RedisStringConexao) ? (string)configDebug.RedisStringConexao : null;
        }

        private List<string> ObterArquivoURLDebug()
        {
            string caminho = Utilidades.IO.FileStorageService.LocalStorage.Combine(Servicos.FS.GetPath(AppDomain.CurrentDomain.BaseDirectory), "DebugConfig.txt");

            if (Utilidades.IO.FileStorageService.LocalStorage.Exists(caminho))
                return Utilidades.IO.FileStorageService.LocalStorage.ReadLines(caminho).ToList();
            else
                throw new ControllerException("Arquivo DebugConfig.txt n√£o localizado.");
        }
    }
}