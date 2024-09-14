namespace Synchronization;

// Custom implementation of ReaderWriterLockSlim
// This implementation is based on the lock primitive and while loops to wait

// Multiple readers can acquire the lock concurrently
// Only 1 writer can acquire at a time
// A writer can not acquire if a reader currently holds a lock
public class ReaderWriterLock : IReaderWriterLock
{
    private int readerCount;
    private int waitingWriters;
    private bool writerActive; // not count since it will only be one
    
    private readonly object syncLock = new object();

    public ReaderWriterLock()
    {
    }

    public void AcquireReaderLock()
    {

        lock (syncLock)
        {
            while (writerActive || waitingWriters > 0)
            {
                // 1. release the lock
                // 2. put current thread in Wait state
                // 3. when thread is woken up by Pulse, reacquires the lock
                Monitor.Wait(syncLock);
            }

            readerCount++;
        }
        
    }

    public void ReleaseReaderLock()
    {
        lock (syncLock)
        {
            readerCount--;
            if (readerCount == 0)
            {
                // Notify all threads waiting on lock - wake up and try to acquire lock
                Monitor.PulseAll(syncLock);
            }
        }
        
    }

    public void AcquireWriterLock()
    {
        lock (syncLock)
        {
            while (readerCount != 0 || writerActive)
            {
                waitingWriters++;
                Monitor.Wait(syncLock);
            }

            waitingWriters--;
            writerActive = true;
        }
    }

    public void ReleaseWriterLock()
    {
        lock (syncLock)
        {
            writerActive = false;
            Monitor.PulseAll(syncLock);
        }
        
    }
}