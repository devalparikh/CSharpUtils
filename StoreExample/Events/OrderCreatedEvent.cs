namespace StoreExample.Events;

public class OrderCreatedEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public string CustomerName { get; }
    public decimal TotalAmount { get; }
    public DateTime OccurredOn { get; }

    public OrderCreatedEvent(Guid orderId, string customerName, decimal totalAmount)
    {
        OrderId = orderId;
        CustomerName = customerName;
        TotalAmount = totalAmount;
        OccurredOn = DateTime.UtcNow;
    }
}