// namespace StoreExample;
//
// public class example
// {
//     
// }
//
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
//
// // Domain Entity
// public class Order
// {
//     public int Id { get; set; }
//     public string Description { get; set; }
//     public List<OutboxEvent> OutboxEvents { get; set; } = new();
//     
//     public static Order Create(string description)
//     {
//         var order = new Order { Description = description };
//         return order;
//     }
// }
//
// // Outbox Event Entity
// public class OutboxEvent
// {
//     public int Id { get; set; }
//     public string EventType { get; set; }
//     public string Payload { get; set; }
//     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//     public bool Processed { get; set; } = false;
//
//     public OutboxEvent(string eventType, object eventData)
//     {
//         EventType = eventType;
//         Payload = System.Text.Json.JsonSerializer.Serialize(eventData);
//     }
// }
//
// // Database Context
// public class OrderDbContext : DbContext
// {
//     public DbSet<Order> Orders { get; set; }
//     public DbSet<OutboxEvent> OutboxEvents { get; set; }
//
//     public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }
//
//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         modelBuilder.Entity<Order>().HasKey(o => o.Id);
//         modelBuilder.Entity<OutboxEvent>().HasKey(e => e.Id);
//     }
// }
//
// // Outbox Processor Background Service
// public class OutboxProcessor : BackgroundService
// {
//     private readonly IServiceScopeFactory _serviceScopeFactory;
//
//     public OutboxProcessor(IServiceScopeFactory serviceScopeFactory)
//     {
//         _serviceScopeFactory = serviceScopeFactory;
//     }
//
//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         while (!stoppingToken.IsCancellationRequested)
//         {
//             using var scope = _serviceScopeFactory.CreateScope();
//             var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
//
//             var events = await dbContext.OutboxEvents
//                 .Where(e => !e.Processed)
//                 .ToListAsync(stoppingToken);
//
//             foreach (var outboxEvent in events)
//             {
//                 Console.WriteLine($"Processing event: {outboxEvent.EventType}");
//                 
//                 if (outboxEvent.EventType == "OrderCreated")
//                 {
//                     var order = System.Text.Json.JsonSerializer.Deserialize<Order>(outboxEvent.Payload);
//                     EmailOrderConfirmation(order);
//                 }
//                 
//                 outboxEvent.Processed = true;
//             }
//
//             await dbContext.SaveChangesAsync(stoppingToken);
//             await Task.Delay(5000, stoppingToken); // Polling interval
//         }
//     }
//
//     private void EmailOrderConfirmation(Order order)
//     {
//         Console.WriteLine($"Sending email confirmation for Order ID: {order.Id}, Description: {order.Description}");
//     }
// }
//
// // Order Service
// public class OrderService
// {
//     private readonly OrderDbContext _dbContext;
//
//     public OrderService(OrderDbContext dbContext)
//     {
//         _dbContext = dbContext;
//     }
//
//     public async Task CreateOrderAsync(string description)
//     {
//         using var transaction = await _dbContext.Database.BeginTransactionAsync();
//         
//         var order = Order.Create(description);
//         var orderCreatedEvent = new OutboxEvent("OrderCreated", order);
//         
//         _dbContext.Orders.Add(order);
//         _dbContext.OutboxEvents.Add(orderCreatedEvent);
//         
//         await _dbContext.SaveChangesAsync();
//         await transaction.CommitAsync();
//     }
// }
//
// // Program Setup
// var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddDbContext<OrderDbContext>(options =>
//     options.UseInMemoryDatabase("OrderDb"));
// builder.Services.AddScoped<OrderService>();
// builder.Services.AddHostedService<OutboxProcessor>();
//
// var app = builder.Build();
//
// using (var scope = app.Services.CreateScope())
// {
//     var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
//     await dbContext.Database.EnsureCreatedAsync();
// }
//
// app.Run();
