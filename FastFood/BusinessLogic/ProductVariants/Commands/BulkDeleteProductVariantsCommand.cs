using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.BusinessLogic.ProductVariants.Commands;

public class BulkDeleteProductVariantsCommand : BaseCommand<List<int>>
{
    public IReadOnlyCollection<int> Ids { get; }

    public BulkDeleteProductVariantsCommand(IEnumerable<int> ids)
    {
        Ids = ids?
            .Where(id => id > 0)
            .Distinct()
            .ToList() ?? new List<int>();
    }

    public class Handler : BaseHandler<BulkDeleteProductVariantsCommand, List<int>>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<List<int>> Handle(BulkDeleteProductVariantsCommand request, CancellationToken cancellationToken)
        {
            if (request.Ids.Count == 0)
            {
                return new List<int>();
            }

            var productVariants = await _dbContext.ProductVariants
                .Where(pv => request.Ids.Contains(pv.Id))
                .ToListAsync(cancellationToken);

            if (productVariants.Count == 0)
            {
                return new List<int>();
            }

            var deletedIds = productVariants.Select(pv => pv.Id).ToList();
            var affectedProductIds = productVariants.Select(pv => pv.Product_FK).Distinct().ToList();
            var productsWithRemainingVariants = await _dbContext.ProductVariants
                .Where(pv => affectedProductIds.Contains(pv.Product_FK) && !deletedIds.Contains(pv.Id))
                .Select(pv => pv.Product_FK)
                .Distinct()
                .ToListAsync(cancellationToken);
            var productsToDelete = await _dbContext.Products
                .Where(p => affectedProductIds.Contains(p.Id) && !productsWithRemainingVariants.Contains(p.Id))
                .ToListAsync(cancellationToken);

            var productVariantIngredients = await _dbContext.ProductVariantIngredients
                .Where(pvi => request.Ids.Contains(pvi.ProductVariant_FK))
                .ToListAsync(cancellationToken);

            var productVariantOrders = await _dbContext.ProductVariantOrders
                .Where(pvo => request.Ids.Contains(pvo.ProductVariant_FK))
                .ToListAsync(cancellationToken);

            if (productVariantIngredients.Count > 0)
            {
                _dbContext.ProductVariantIngredients.RemoveRange(productVariantIngredients);
            }

            if (productVariantOrders.Count > 0)
            {
                _dbContext.ProductVariantOrders.RemoveRange(productVariantOrders);
            }

            _dbContext.ProductVariants.RemoveRange(productVariants);
            _dbContext.Products.RemoveRange(productsToDelete);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return deletedIds;
        }
    }
}
