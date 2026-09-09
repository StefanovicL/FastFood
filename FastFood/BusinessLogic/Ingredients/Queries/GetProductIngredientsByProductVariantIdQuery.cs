using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.BusinessLogic.Ingredients.Queries;

public class GetProductIngredientsByProductVariantIdQuery: BaseQuery<List<IngredientDTO>>
{
    public int ProductVariantId { get; set; }
    public GetProductIngredientsByProductVariantIdQuery(int productVariantId)
    {
        ProductVariantId = productVariantId;
    }

    public class Handler : BaseHandler<GetProductIngredientsByProductVariantIdQuery, List<IngredientDTO>>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<List<IngredientDTO>> Handle(GetProductIngredientsByProductVariantIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.ProductVariantIngredients
                .Where(pi => pi.ProductVariant_FK == request.ProductVariantId)
                .Select(pi => pi.Ingredient)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<IngredientDTO>>(result);
        }
    }
}
