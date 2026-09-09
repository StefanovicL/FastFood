using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.BusinessLogic.Orders.Queries;

public class GetAllOrdersQuery : BaseQuery<List<OrderDTO>>
{
    public class Handler : BaseHandler<GetAllOrdersQuery, List<OrderDTO>>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<List<OrderDTO>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Orders
                .Include(o => o.ProductVariantOrders)
                    .ThenInclude(op => op.ProductVariant)
                        .ThenInclude(p => p.Product)
                .ToListAsync();  

            return _mapper.Map<List<OrderDTO>>(result);
        }
    }
}