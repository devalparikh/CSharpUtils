using System.Collections.Concurrent;
using EventDriven.Common;

namespace EventDriven;

public abstract class AbstractPubSub
{
    protected readonly ConcurrentQueue<Message> _concurrentQueue = new ConcurrentQueue<Message>();
    protected readonly HashSet<Subscriber> _subscribers = new HashSet<Subscriber>();
    public void Subscribe(Subscriber subscriber)
    {
        lock (_subscribers)
        {
            _subscribers.Add(subscriber);
        }
    }
    public abstract Task PublishAsync(Message message);
}

