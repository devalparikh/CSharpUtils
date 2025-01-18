namespace EventDriven.Common;

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