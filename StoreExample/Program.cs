using Microsoft.EntityFrameworkCore;
using StoreExample.Events;
using StoreExample.Models;
using StoreExample.Repository.DbContext;
using StoreExample.Service;

// var builder = Host.CreateApplicationBuilder(args);

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// Database
builder.Services.AddDbContext<OrderDbContext>(options => options.UseInMemoryDatabase("OrderDb"));

// Services
builder.Services.AddScoped<OrderService>();

// Repositories
// Keyed singleton
// We can pull from options settings instead
// var entityDictionary = new Dictionary<Type, Type>
// {
//     // { typeof(OrderEntity), typeof(MockEntityDatabase<OrderEntity>) },
//     // { typeof(OrderEntity), typeof(MockOrderDatabase) },
//     { typeof(UserEntity), typeof(MockUserDatabase) },
// };
// foreach (var entity in entityDictionary)
// {
//     var serviceType = typeof(IDatabase<>).MakeGenericType(entity.Key);
//     builder.Services.AddSingleton(serviceType, entity.Value);
// }

// Adapters
builder.Services.AddSingleton<OrderMapper>();

// Hosted Services
builder.Services.AddHostedService<OutboxProcessor>();

var app = builder.Build();
app.MapControllers();
app.Run();