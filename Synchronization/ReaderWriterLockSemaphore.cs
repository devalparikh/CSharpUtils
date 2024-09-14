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
            // For the last reader, allow waiting writer to acquire a lock
            _writerSemaphore.Release();
        }
        _readerSemaphore.Release();
    }

    public async void AcquireWriterLock()
    {
        await _writerSemaphore.WaitAsync();
    }

    public void ReleaseWriterLock()
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

// example:
// 1. writer1 acquires write semaphore
// 1. writer1 is writing
// 2. reader1 acquires read semaphore
// 2. reader1 waits on writer1
// 3. reader2 waits on reader1
// 4. writer1 releases write semaphore
// 5. reader1 acquires write semaphore (writers are now blocked)
// 6. reader1 releases read semaphore
// 7. reader2 acquires read semaphore
// 8. reader3, reader4, ... all readers are now acquiring/releasing read semaphore to update the counter
//    all readers can read concurrently 
// 9. last reader releases write semaphore
// 10. now a writer can write again