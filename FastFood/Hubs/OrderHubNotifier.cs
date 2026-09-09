using FastFood.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace FastFood.Hubs;

public class OrderHubNotifier : IOrderHubNotifier
{
    private readonly IHubContext<OrderHub> _hubContext;

    public OrderHubNotifier(IHubContext<OrderHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyOrderCreated(OrderDTO order)
    {
        return _hubContext.Clients.All.SendAsync("OrderCreated", order);
    }

    public Task NotifyOrderStateChanged(OrderDTO order)
    {
        return _hubContext.Clients.All.SendAsync("OrderStateChanged", order);
    }
}
