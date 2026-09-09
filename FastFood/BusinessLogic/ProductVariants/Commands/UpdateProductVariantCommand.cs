using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.BusinessLogic.ProductVariants.Commands;

public class UpdateProductVariantCommand : BaseCommand<ProductVariantDTO>
{
    public ProductVariantDTO Dto { get; }

    public UpdateProductVariantCommand(ProductVariantDTO dto) => Dto = dto;

    public class Handler : BaseHandler<UpdateProductVariantCommand, ProductVariantDTO>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<ProductVariantDTO> Handle(UpdateProductVariantCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            ProductVariant variant;

            if (dto.Id > 0)
            {
                variant = await _dbContext.ProductVariants
                    .Include(v => v.ProductVariantIngredients)
                    .FirstOrDefaultAsync(v => v.Id == dto.Id, cancellationToken)
                    ?? throw new Exception("Varijanta proizvoda ne postoji.");
            }
            else
            {
                if (dto.Product_FK <= 0)
                {
                    throw new Exception("Proizvod za varijantu ne postoji.");
                }

                variant = new ProductVariant();
                _dbContext.ProductVariants.Add(variant);
            }

            UpdateVariantFields(variant, dto);
            SyncIngredients(variant, dto.ProductVariantIngredients?.ToList() ?? new List<ProductVariantIngredientDTO>());

            await _dbContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ProductVariantDTO>(variant);
        }

        private static void UpdateVariantFields(ProductVariant entity, ProductVariantDTO dto)
        {
            if (dto.Product_FK > 0)
            {
                entity.Product_FK = dto.Product_FK;
            }

            entity.Size = (int)dto.Size;
            entity.Price = dto.Price;
            entity.OriginalFileName = dto.OriginalFileName;
            entity.StoredFileName = dto.StoredFileName;
        }

        private void SyncIngredients(ProductVariant variant, List<ProductVariantIngredientDTO> ingredientDtos)
        {
            var existingByIngredientId = variant.ProductVariantIngredients.ToDictionary(i => i.Ingredient_FK);
            var incomingIngredientIds = new HashSet<int>(ingredientDtos
                .Select(GetIngredientId)
                .Where(id => id > 0));

            var toRemove = variant.ProductVariantIngredients
                .Where(i => !incomingIngredientIds.Contains(i.Ingredient_FK))
                .ToList();

            if (toRemove.Count > 0)
            {
                _dbContext.ProductVariantIngredients.RemoveRange(toRemove);
            }

            foreach (var dto in ingredientDtos)
            {
                var ingredientId = GetIngredientId(dto);
                if (ingredientId <= 0)
                {
                    continue;
                }

                if (existingByIngredientId.TryGetValue(ingredientId, out var existing))
                {
                    existing.Quantity = dto.Quantity;
                    continue;
                }

                variant.ProductVariantIngredients.Add(new ProductVariantIngredient
                {
                    ProductVariant = variant,
                    Ingredient_FK = ingredientId,
                    Quantity = dto.Quantity
                });
            }
        }

        private static int GetIngredientId(ProductVariantIngredientDTO dto)
        {
            return dto.Ingredient_FK > 0 ? dto.Ingredient_FK : dto.Id;
        }
    }
}
