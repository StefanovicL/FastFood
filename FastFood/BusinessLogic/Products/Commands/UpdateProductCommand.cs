using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.BusinessLogic.Products.Commands;

public class UpdateProductCommand : BaseCommand<ProductDTO>
{
    public ProductDTO Dto { get; }

    public UpdateProductCommand(ProductDTO dto) => Dto = dto;

    public class Handler : BaseHandler<UpdateProductCommand, ProductDTO>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper) { }

        public override async Task<ProductDTO> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await LoadProductGraph(request.Dto.Id, cancellationToken) ?? throw new Exception("Proizvod ne postoji.");

            UpdateProductFields(product, request.Dto);

            SyncVariants(product, request.Dto.ProductVariants.ToList());

            await _dbContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ProductDTO>(product);
        }

        private Task<Product?> LoadProductGraph(int productId, CancellationToken ct)
        {
            return _dbContext.Products
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.ProductVariantIngredients)
                .FirstOrDefaultAsync(p => p.Id == productId, ct);
        }

        private static void UpdateProductFields(Product product, ProductDTO dto)
        {
            product.Name = dto.Name;
            product.Description = dto.Description;
        }

        private void SyncVariants(Product product, List<ProductVariantDTO> variantDtos)
        {
            variantDtos ??= new List<ProductVariantDTO>();

            var existingById = product.ProductVariants.ToDictionary(v => v.Id);
            var incomingIds = new HashSet<int>(variantDtos.Where(v => v.Id > 0).Select(v => v.Id));

            RemoveMissingVariants(product, incomingIds);

            foreach (var dto in variantDtos)
            {
                if (dto.Id <= 0)
                {
                    // New variant
                    var created = _mapper.Map<ProductVariant>(dto);
                    product.ProductVariants.Add(created);

                    // Ingredients for a newly created variant
                    ApplyIngredientsToNewVariant(created, dto.ProductVariantIngredients.ToList());
                    continue;
                }

                if (!existingById.TryGetValue(dto.Id, out var existing))
                    continue; // DTO refers to non-existing variant

                UpdateVariantFields(existing, dto);
                SyncIngredients(existing, dto.ProductVariantIngredients.ToList());
            }
        }

        private void RemoveMissingVariants(Product product, HashSet<int> incomingIds)
        {
            var toRemove = product.ProductVariants
                .Where(v => v.Id > 0 && !incomingIds.Contains(v.Id))
                .ToList();

            if (toRemove.Count == 0) 
                return;

            _dbContext.ProductVariants.RemoveRange(toRemove);
        }

        private static void UpdateVariantFields(ProductVariant entity, ProductVariantDTO dto)
        {
            entity.Size = (int)dto.Size;
            entity.Price = dto.Price;
        }

        private void ApplyIngredientsToNewVariant(ProductVariant variant, List<ProductVariantIngredientDTO> ingredientDtos)
        {
            ingredientDtos ??= new List<ProductVariantIngredientDTO>();

            foreach (var ingDto in ingredientDtos)
            {
                variant.ProductVariantIngredients.Add(_mapper.Map<ProductVariantIngredient>(ingDto));
            }
        }

        private void SyncIngredients(ProductVariant variant, List<ProductVariantIngredientDTO> ingredientDtos)
        {
            ingredientDtos ??= new List<ProductVariantIngredientDTO>();

            var existingByIngredientId = variant.ProductVariantIngredients.ToDictionary(i => i.Ingredient_FK);
            var incomingIngredientIds = new HashSet<int>(ingredientDtos
                .Select(GetIngredientId)
                .Where(id => id > 0));

            RemoveMissingIngredients(variant, incomingIngredientIds);

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
                }
                else
                {
                    variant.ProductVariantIngredients.Add(new ProductVariantIngredient
                    {
                        ProductVariant = variant,
                        Ingredient_FK = ingredientId,
                        Quantity = dto.Quantity
                    });
                }
            }
        }

        private static int GetIngredientId(ProductVariantIngredientDTO dto)
        {
            return dto.Ingredient_FK > 0 ? dto.Ingredient_FK : dto.Id;
        }


        private void RemoveMissingIngredients(ProductVariant variant, HashSet<int> incomingIngredientIds)
        {
            var toRemove = variant.ProductVariantIngredients
                .Where(i => !incomingIngredientIds.Contains(i.Ingredient_FK))
                .ToList();

            if (toRemove.Count == 0) 
                return;

            _dbContext.ProductVariantIngredients.RemoveRange(toRemove);
        }
    }
}
