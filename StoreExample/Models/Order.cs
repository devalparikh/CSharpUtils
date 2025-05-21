using System.Collections.Concurrent;
using Newtonsoft.Json;
using Riok.Mapperly.Abstractions;
using StoreExample.Events;

namespace StoreExample.Models;

public class Order
{
    public Guid Id { get; init; }
}

public class OrderRequest
{
    public Guid Id { get; init; } = Guid.NewGuid();
}

public class OrderResponse
{
    public Guid Id { get; init; }
}

public class OrderEntity : Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}


[Mapper]
public partial class OrderMapper
{
    public partial OrderEntity ToEntity(Order order);
    public partial Order ToModel(OrderEntity entity);
    public partial Order ToModel(OrderRequest request);
    public partial OrderResponse ToResponse(Order order);

    public async Task<IEnumerable<OrderResponse>> ToResponses(IEnumerable<Order> orders)
    {
        var ordersResponse = new ConcurrentQueue<OrderResponse>(); // Thread-safe and maintains order

        await Parallel.ForEachAsync(orders, async (order, _) =>
        {
            var response = ToResponse(order);
            ordersResponse.Enqueue(response); // Preserves order
        });

        return ordersResponse;
    }
}