using System.Collections.Concurrent;

namespace EventDriven;

// Use IReaderWriterLock or ReaderWriterLockSlim
public class PubSub : AbstractPubSub
{
    public override async Task PublishAsync(Message message)
    {
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
                     subscriber.Notify(message);
                 });
                 tasks.Add(task);
             }
         }
         
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