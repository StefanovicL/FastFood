using Microsoft.AspNetCore.SignalR;

namespace FastFood.Hubs;

// Server-to-client only hub: broadcasts order lifecycle events, clients don't invoke methods on it.
public class OrderHub : Hub
{
}
