using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Enums;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.BusinessLogic.Orders.Queries;

public class GetActiveOrdersQuery : BaseQuery<List<OrderDTO>>
{
    public class Handler : BaseHandler<GetActiveOrdersQuery, List<OrderDTO>>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<List<OrderDTO>> Handle(GetActiveOrdersQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Orders
                .Include(o => o.ProductVariantOrders)
                    .ThenInclude(op => op.ProductVariant)
                        .ThenInclude(p => p.Product)
                .Include(o => o.ProductVariantOrders)
                    .ThenInclude(op => op.ProductVariant)
                        .ThenInclude(p => p.ProductVariantIngredients)
                            .ThenInclude(pvi => pvi.Ingredient)
                .Where(o => o.State == (int)eOrderState.Created)
                .ToListAsync();

            return _mapper.Map<List<OrderDTO>>(result);
        }
    }
}
