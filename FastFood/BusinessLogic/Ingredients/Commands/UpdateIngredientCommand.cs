using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;

namespace FastFood.BusinessLogic.Ingredients.Commands;

public class UpdateIngredientCommand : BaseCommand<IngredientDTO>
{
    private IngredientDTO Dto { get; set; }
    public UpdateIngredientCommand(IngredientDTO dto)
    {
        Dto = dto;
    }

    public class Handler : BaseHandler<UpdateIngredientCommand, IngredientDTO>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<IngredientDTO> Handle(UpdateIngredientCommand request, CancellationToken cancellationToken)
        {
            var ingredient = await _dbContext.Ingredients.FindAsync(request.Dto.Id);
            if (ingredient == null)
            {
                throw new Exception("Sastojak ne postoji.");
            }

            _mapper.Map(request.Dto, ingredient);

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<IngredientDTO>(ingredient);
        }
    }
}