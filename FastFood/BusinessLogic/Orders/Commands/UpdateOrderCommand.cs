using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Enums;
using FastFood.Hubs;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.BusinessLogic.Orders.Commands;

public class UpdateOrderCommand : BaseCommand<OrderDTO>
{
    private OrderDTO Dto { get; set; }
    public UpdateOrderCommand(OrderDTO dto)
    {
        Dto = dto;
    }

    public class Handler : BaseHandler<UpdateOrderCommand, OrderDTO>
    {
        private readonly IOrderHubNotifier _orderHubNotifier;

        public Handler(FastFoodContext dbContext, IMapper mapper, IOrderHubNotifier orderHubNotifier) : base(dbContext, mapper)
        {
            _orderHubNotifier = orderHubNotifier;
        }

        public override async Task<OrderDTO> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _dbContext.Orders
                .Include(o => o.ProductVariantOrders)
                    .ThenInclude(op => op.ProductVariant)
                        .ThenInclude(p => p.Product)
                .Include(o => o.ProductVariantOrders)
                    .ThenInclude(op => op.ProductVariant)
                        .ThenInclude(p => p.ProductVariantIngredients)
                            .ThenInclude(pvi => pvi.Ingredient)
                .FirstOrDefaultAsync(o => o.Id == request.Dto.Id, cancellationToken);

            if (order == null)
            {
                throw new Exception("Porudzbina ne postoji.");
            }

            var previousState = order.State;

            order.State = (int)request.Dto.State;

            if (previousState != order.State)
            {
                var timestamp = DateTime.Now;

                switch (request.Dto.State)
                {
                    case eOrderState.Ready when order.OrderReadyDateTime == null:
                        order.OrderReadyDateTime = timestamp;
                        break;
                    case eOrderState.Completed when order.OrderDeliveredDateTime == null:
                        order.OrderDeliveredDateTime = timestamp;
                        break;
                    case eOrderState.Cancelled when order.OrderCancelledDateTime == null:
                        order.OrderCancelledDateTime = timestamp;
                        break;
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            var orderDto = _mapper.Map<OrderDTO>(order);

            if (previousState != order.State)
            {
                await _orderHubNotifier.NotifyOrderStateChanged(orderDto);
            }

            return orderDto;
        }
    }
}