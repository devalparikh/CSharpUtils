using System.Collections.Concurrent;

namespace EventDriven;

public abstract class AbstractPubSub
{
    protected readonly ConcurrentQueue<Message> _concurrentQueue = new ConcurrentQueue<Message>();
    protected readonly HashSet<Subscriber> _subscribers = new HashSet<Subscriber>();
    public abstract void Subscribe(Subscriber subscriber);
    public abstract Task PublishAsync(Message message);
}

public class Message
{
    public Publisher Sender;
    public string Content;

    public Message(string content)
    {
        Content = content;
    }
}

public class Publisher
{
    public readonly string Name;
    public Publisher(string name)
    {
        Name = name;
    }
    
    public async Task Publish(AbstractPubSub pubSub, Message message)
    {
        message.Sender = this;
        Console.WriteLine($"Publisher@{Name} sent a message: {message.Content}");
        await pubSub.PublishAsync(message);
    }
    
}

public class Subscriber
{
    private readonly string Name;

    public Subscriber(string name)
    {
        Name = name;
    }
    
    public void Subscribe(PubSub pubSub)
    {
        pubSub.Subscribe(this);
    }

    public void Notify(Message message)
    {
        Console.WriteLine($"Subscriber@{Name} received a message: {message.Content} from Publisher@{message.Sender.Name}");
    }
    
}