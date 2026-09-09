using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FastFood.BusinessLogic.Orders.Commands;

public class DeleteOrderCommand : BaseCommand<OrderDTO>
{
    private int Id { get; set; }
    public DeleteOrderCommand(int id)
    {
        Id = id;
    }

    public class Handler : BaseHandler<DeleteOrderCommand, OrderDTO>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<OrderDTO> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var order = await _dbContext.Orders
                .Include(o => o.ProductVariantOrders)
                    .ThenInclude(po => po.ProductVariant)
                        .ThenInclude(variant => variant.ProductVariantIngredients)
                            .ThenInclude(recipe => recipe.Ingredient)
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

            if (order == null)
            {
                throw new Exception("Porudzbina ne postoji.");
            }

            // Vracanje utrosenih sastojaka nazad u magacin
            foreach (var productVariantOrder in order.ProductVariantOrders)
            {
                foreach (var recipe in productVariantOrder.ProductVariant.ProductVariantIngredients)
                {
                    recipe.Ingredient.Quantity += recipe.Quantity * productVariantOrder.Quantity;
                }
            }

            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return _mapper.Map<OrderDTO>(order);
        }
    }
}