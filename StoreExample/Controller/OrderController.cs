using Microsoft.AspNetCore.Mvc;
using StoreExample.ErrorHandling;
using StoreExample.Models;
using StoreExample.Service;

namespace StoreExample.Controller;

public class OrderController : CustomControllerBase
{
    private OrderService _orderService;
    private OrderMapper _orderMapper;

    public OrderController(OrderService orderService, OrderMapper orderMapper)
    {
        _orderService = orderService;
        _orderMapper = orderMapper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(OrderRequest orderRequest)
    {
        Order newOrder = _orderMapper.ToModel(orderRequest);
        ErrorOr<Order> order = await _orderService.CreateOrderAsync(newOrder);
        if (order.IsError) return Failure(order);
        OrderResponse orderResponse = _orderMapper.ToResponse(order.Value!);
        return Created(new Uri($"Order/{orderResponse.Id}", UriKind.Relative), orderResponse);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        if (orders.IsError) Failure(orders);
        IEnumerable<OrderResponse> ordersResponse = await _orderMapper.ToResponses(orders.Value!);
        return Ok(ordersResponse);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var order = await _orderService.GetOrderAsync(id);
        if (order.IsError) return Failure(order);
        OrderResponse orderResponse = _orderMapper.ToResponse(order.Value!);
        return Ok(orderResponse);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(Guid id)
    {
        var order = await _orderService.UpdateOrderAsync(id);
        if (order.IsError) return Failure(order);
        OrderResponse orderResponse = _orderMapper.ToResponse(order.Value!);
        return Ok(orderResponse);
    }
    
}