namespace Synchronization;

// Custom implementation of ReaderWriterLockSlim
// This implementation is based on the semaphore primitive

// Contrast to ReaderWriterLock, semaphores will automatically maintain state of which waiting thread to resume on 
// WaitAsync() - waits and acquires

// Multiple readers can acquire the lock concurrently
// Only 1 writer can acquire at a time
// A writer can not acquire if a reader currently holds a lock
public class ReaderWriterLockSemaphore : IReaderWriterLock
{
    private SemaphoreSlim _readerSemaphore = new SemaphoreSlim(1,1);
    private SemaphoreSlim _writerSemaphore = new SemaphoreSlim(1, 1);

    private int _readerCount;
    
    public async void AcquireReaderLock()
    {
        await _readerSemaphore.WaitAsync();

        _readerCount++;
        bool isFirstReader = _readerCount == 1; 
        if (isFirstReader)
        {
            // For the first reader, wait on the writer to be released 
            await _writerSemaphore.WaitAsync();
        }

        _readerSemaphore.Release();
    }

    public async void ReleaseReaderLock()
    {
        await _readerSemaphore.WaitAsync();
        _readerCount--;
        bool isLastReader = _readerCount == 0;
        if (isLastReader)
        {
            _writerSemaphore.Release();
        }
        _readerSemaphore.Release();
    }

    public async void AcquireWriterLock()
    {
        await _writerSemaphore.WaitAsync();
    }

    public async void ReleaseWriterLock()
    {
        _writerSemaphore.Release();
    }
}

// Usage
// AcquireReaderLock()
// read logic ...
// ReleaseReaderLock()

// AcquireWriterLock()
// write logic ...
// ReleaseWriterLock()