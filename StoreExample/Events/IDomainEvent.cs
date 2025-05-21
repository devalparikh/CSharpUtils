namespace StoreExample.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
