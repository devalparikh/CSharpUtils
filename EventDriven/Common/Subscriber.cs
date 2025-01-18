namespace EventDriven.Common;

public class Subscriber
{
    private readonly string Name;

    public Subscriber(string name)
    {
        Name = name;
    }
    
    public void Subscribe(AbstractPubSub pubSub)
    {
        pubSub.Subscribe(this);
    }

    public void Notify(Message message)
    {
        Console.WriteLine($"Subscriber@{Name} received a message: {message.Content} from Publisher@{message.Sender.Name}");
    }
    
}