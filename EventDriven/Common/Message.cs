namespace EventDriven.Common;

public class Message
{
    public Publisher Sender;
    public string Content;

    public Message(string content)
    {
        Content = content;
    }
}