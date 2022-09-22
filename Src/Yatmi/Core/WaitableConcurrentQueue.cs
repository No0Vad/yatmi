using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Yatmi.Core
{
    internal sealed class WaitableConcurrentQueue<T> : ConcurrentQueue<T>, IDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly TimeGatedSemaphore _throttler;

        public WaitableConcurrentQueue(int maxRequest, TimeSpan minimumHoldTime)
        {
            _semaphore = new SemaphoreSlim(0);
            _throttler = new TimeGatedSemaphore(maxRequest, minimumHoldTime);
        }


        public new void Enqueue(T item)
        {
            _semaphore.Release();
            base.Enqueue(item);
        }


        public async Task<T> DequeueAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _semaphore.WaitAsync(cancellationToken);

                using var _ = await _throttler.WaitAsync();

                TryDequeue(out var workItem);

                return workItem;
            }
            catch (ObjectDisposedException)
            {
                // We don't care about this exception here
                return default;
            }
        }


        public void Dispose()
        {
            _throttler?.Dispose();
            _semaphore?.Dispose();
        }
    }
}