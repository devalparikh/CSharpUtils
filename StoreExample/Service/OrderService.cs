using StoreExample.ErrorHandling;
using StoreExample.Events;
using StoreExample.Models;
using StoreExample.Repository;
using StoreExample.Repository.DbContext;

namespace StoreExample.Service;

public class OrderService
{
    private readonly OrderDbContext _dbContext;
    private readonly OrderMapper _orderMapper;

    public OrderService(OrderDbContext dbContext, OrderMapper orderMapper)
    {
        _dbContext = dbContext;
        _orderMapper = orderMapper;
    }

    public async Task<ErrorOr<Order>> CreateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        try
        {
            OrderEntity orderEntity = _orderMapper.ToEntity(order);
            orderEntity.AddDomainEvent(new OrderCreatedEvent(orderEntity.Id, "", 1));
            await _dbContext.Orders.AddAsync(orderEntity, cancellationToken);
            await _dbContext.SaveEntitiesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            List<string> errors = [exception.Message];
            if (exception.InnerException != null)
            {
                errors.Add(exception.InnerException.Message);
            }
            Error error = new Error(ErrorType.Failure, errors);
            return error;
        }

        return order;
    }

    public async Task<ErrorOr<IEnumerable<Order>>> GetAllOrdersAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<ErrorOr<Order>> GetOrderAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<ErrorOr<Order>> UpdateOrderAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}