using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;

namespace FastFood.BusinessLogic.Ingredients.Commands;

public class DeleteIngredientCommand : BaseCommand<IngredientDTO>
{
    private int Id { get; set; }
    public DeleteIngredientCommand(int id)
    {
        Id = id;
    }

    public class Handler : BaseHandler<DeleteIngredientCommand, IngredientDTO>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<IngredientDTO> Handle(DeleteIngredientCommand request, CancellationToken cancellationToken)
        {
            var ingredient = await _dbContext.Ingredients.FindAsync(request.Id);
            if (ingredient == null)
            {
                throw new Exception("Sastojak ne postoji.");
            }

            _dbContext.Ingredients.Remove(ingredient);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<IngredientDTO>(ingredient);
        }
    }
}