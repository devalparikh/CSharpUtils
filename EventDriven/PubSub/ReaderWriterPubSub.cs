using Synchronization;

namespace EventDriven;

// Use IReaderWriterLock or ReaderWriterLockSlim
public class ReaderWriterPubSub : AbstractPubSub
{
    private readonly ReaderWriterLockSemaphore _readerWriterLockSemaphore = new ReaderWriterLockSemaphore();
    
    public override async Task PublishAsync(Message message)
    {
        _readerWriterLockSemaphore.AcquireWriterLock();
        
        // Broadcast to all subscribers in parallel
        
        // 1. add message to queue for in memory persistence
         _concurrentQueue.Append(message);
         
         // 2. populate tasks to be computed in parallel
         var tasks = new List<Task>();
         lock (_subscribers)
         {
             foreach (var subscriber in _subscribers)
             {
                 var task = Task.Run(() =>
                 {
                     _readerWriterLockSemaphore.AcquireReaderLock();
                     subscriber.Notify(message);
                     _readerWriterLockSemaphore.ReleaseReaderLock();
                 });
                 tasks.Add(task);
             }
         }
         _readerWriterLockSemaphore.ReleaseWriterLock();

         // 3. complete all tasks in parallel
         await Task.WhenAll(tasks);
         
    }

    public override void Subscribe(Subscriber subscriber)
    {
        lock (_subscribers)
        {
            _subscribers.Add(subscriber);
        }
    }
}
