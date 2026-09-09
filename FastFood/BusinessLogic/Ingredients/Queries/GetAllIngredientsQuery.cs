using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.BusinessLogic.Ingredients.Queries;

public class GetAllIngredientsQuery : BaseQuery<List<IngredientDTO>>
{
    public GetAllIngredientsQuery()
    {
    }

    public class Handler : BaseHandler<GetAllIngredientsQuery, List<IngredientDTO>>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<List<IngredientDTO>> Handle(GetAllIngredientsQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Ingredients.ToListAsync(cancellationToken);

            return _mapper.Map<List<IngredientDTO>>(result);
        }
    }
}