using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;

namespace FastFood.BusinessLogic.Ingredients.Commands;

public class CreateIngredientCommand : BaseCommand<IngredientDTO>
{
    private IngredientDTO Dto { get; set; }
    public CreateIngredientCommand(IngredientDTO dto)
    {
        Dto = dto;
    }

    public class Handler : BaseHandler<CreateIngredientCommand, IngredientDTO>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<IngredientDTO> Handle(CreateIngredientCommand request, CancellationToken cancellationToken)
        {
            var ingredient = _mapper.Map<Ingredient>(request.Dto);

            _dbContext.Ingredients.Add(ingredient);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<IngredientDTO>(ingredient);
        }
    }
}