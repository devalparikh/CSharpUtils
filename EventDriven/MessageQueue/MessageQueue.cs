using EventDriven.Common;

namespace EventDriven;

public class MessageQueue
{
    private List<Message> _messages = new List<Message>();
    private int _offset = 0;
    private object _lock = new object();

    public void Push(Message message)
    {
        _messages.Add(message);
    }

    public Message Poll()
    {
        Message message;
        message = _messages[_offset];
        
        lock (_lock)
        {
            _offset++;
        }

        return message;

    }
    
}
