using FastFood.Dtos;

namespace FastFood.Hubs;

public interface IOrderHubNotifier
{
    Task NotifyOrderCreated(OrderDTO order);
    Task NotifyOrderStateChanged(OrderDTO order);
}
