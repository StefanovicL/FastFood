using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.BusinessLogic.ProductVariants.Commands;

public class DeleteProductVariantCommand : BaseCommand<ProductVariantDTO>
{
    public int Id { get; }

    public DeleteProductVariantCommand(int id) => Id = id;

    public class Handler : BaseHandler<DeleteProductVariantCommand, ProductVariantDTO>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<ProductVariantDTO> Handle(DeleteProductVariantCommand request, CancellationToken cancellationToken)
        {
            var productVariant = await _dbContext.ProductVariants
                .FirstOrDefaultAsync(pv => pv.Id == request.Id, cancellationToken);
            if (productVariant == null)
            {
                throw new Exception("Varijanta proizvoda ne postoji.");
            }

            var hasOtherVariants = await _dbContext.ProductVariants
                .AnyAsync(pv => pv.Product_FK == productVariant.Product_FK && pv.Id != productVariant.Id, cancellationToken);

            var productVariantIngredients = await _dbContext.ProductVariantIngredients
                .Where(pvi => pvi.ProductVariant_FK == request.Id)
                .ToListAsync(cancellationToken);

            var productVariantOrders = await _dbContext.ProductVariantOrders
                .Where(pvo => pvo.ProductVariant_FK == request.Id)
                .ToListAsync(cancellationToken);

            if (productVariantIngredients.Count > 0)
            {
                _dbContext.ProductVariantIngredients.RemoveRange(productVariantIngredients);
            }

            if (productVariantOrders.Count > 0)
            {
                _dbContext.ProductVariantOrders.RemoveRange(productVariantOrders);
            }

            _dbContext.ProductVariants.Remove(productVariant);

            if (!hasOtherVariants)
            {
                var product = await _dbContext.Products
                    .FirstOrDefaultAsync(p => p.Id == productVariant.Product_FK, cancellationToken);

                if (product != null)
                {
                    _dbContext.Products.Remove(product);
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ProductVariantDTO>(productVariant);
        }
    }
}
