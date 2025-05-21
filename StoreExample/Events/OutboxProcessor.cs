using Microsoft.EntityFrameworkCore;
using StoreExample.Models;
using StoreExample.Repository.DbContext;

namespace StoreExample.Events;

public class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly OrderMapper _orderMapper;

    public OutboxProcessor(IServiceScopeFactory serviceScopeFactory, OrderMapper orderMapper)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _orderMapper = orderMapper;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"{nameof(OutboxProcessor)} execution");

            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

            var eventsToProcess = await dbContext.OutboxMessages
                .Where(e => !e.Processed)
                .ToListAsync(stoppingToken);

            foreach (var outboxMessage in eventsToProcess)
            {
                Console.WriteLine($"Processing event: {outboxMessage.Type}");

                if (outboxMessage.Type == typeof(OrderCreatedEvent).AssemblyQualifiedName)
                {
                    var orderCreatedEvent = System.Text.Json.JsonSerializer.Deserialize<OrderCreatedEvent>(outboxMessage.Content);
                    // var order = _orderMapper.ToModel(orderEntity!);
                    EmailOrderConfirmation(orderCreatedEvent);
                    FulfillShipping(orderCreatedEvent);
                    PublishToMessageQueue(orderCreatedEvent);
                    // other domain handlers
                }

                outboxMessage.Processed = true;
            }

            await dbContext.SaveChangesAsync(stoppingToken);
            await Task.Delay(5000, stoppingToken);
        }
    }
    
    private static void EmailOrderConfirmation(OrderCreatedEvent order)
    {
        Console.WriteLine($"Sending email confirmation for Order ID: {order.OrderId}, Description: {order}");
    }
    
    private static void FulfillShipping(OrderCreatedEvent order)
    {
        Console.WriteLine($"Fulfilling shipping for Order ID: {order.OrderId}, Description: {order}");
    }
    
    private static void PublishToMessageQueue(OrderCreatedEvent order)
    {
        Console.WriteLine($"Publish to message queue for Order ID: {order.OrderId}, Description: {order}");
    }
}