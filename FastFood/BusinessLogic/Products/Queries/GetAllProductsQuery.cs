using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.BusinessLogic.Products.Queries;

public class GetAllProductsQuery : BaseQuery<List<ProductDTO>>
{
    public GetAllProductsQuery()
    {
    }

    public class Handler : BaseHandler<GetAllProductsQuery, List<ProductDTO>>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<List<ProductDTO>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Products
                .Where(p => p.ProductVariants.Any())
                .Include(p => p.ProductVariants)
                    .ThenInclude(i => i.ProductVariantIngredients)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ProductDTO>>(result);
        }
    }
}