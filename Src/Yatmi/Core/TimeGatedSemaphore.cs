using System;
using System.Threading;
using System.Threading.Tasks;

namespace Yatmi.Core;

// Base code from https://stackoverflow.com/a/57691890 but with some modifications

internal sealed class TimeGatedSemaphore : IDisposable
{
    private readonly SemaphoreSlim _semaphore;
    private readonly CancellationTokenSource _tokenSource;

    public TimeSpan MinimumHoldTime { get; }


    public TimeGatedSemaphore(int maxRequest, TimeSpan minimumHoldTime)
    {
        _tokenSource = new CancellationTokenSource();
        _semaphore = new SemaphoreSlim(maxRequest);
        MinimumHoldTime = minimumHoldTime;
    }


    public async Task<IDisposable> WaitAsync()
    {
        await _semaphore.WaitAsync();

        return new InternalReleaser(_semaphore, Task.Delay(MinimumHoldTime, _tokenSource.Token), _tokenSource.Token);
    }


    public void Dispose()
    {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
        _semaphore?.Dispose();
    }




    private class InternalReleaser : IDisposable
    {
        private readonly SemaphoreSlim _semaphoreToRelease;
        private readonly Task _notBeforeTask;
        private readonly CancellationToken _token;

        public InternalReleaser(SemaphoreSlim semaphoreSlim, Task dependantTask, CancellationToken token)
        {
            _semaphoreToRelease = semaphoreSlim;
            _notBeforeTask = dependantTask;
            _token = token;
        }

        public void Dispose()
        {
            _notBeforeTask.ContinueWith(_ => _semaphoreToRelease.Release(), _token);
        }
    }
}