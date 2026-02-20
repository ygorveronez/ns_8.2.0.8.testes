using System;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers.Utils
{

    public interface IDistributedLock
    {
        Task<bool> LockAsync(string key, TimeSpan expiry, CancellationToken cancellationToken);
        Task UnlockAsync(string key, CancellationToken cancellationToken);
    }
}