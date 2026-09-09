using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Hubs;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FastFood.BusinessLogic.Orders.Commands;

public sealed class OrderValidationException : Exception
{
    public OrderValidationException(string message) : base(message)
    {
    }
}

public class CreateOrderCommand : BaseCommand<OrderDTO>
{
    private OrderDTO Dto { get; set; }
    public CreateOrderCommand(OrderDTO dto)
    {
        Dto = dto;
    }

    public class Handler : BaseHandler<CreateOrderCommand, OrderDTO>
    {
        private readonly IOrderHubNotifier _orderHubNotifier;

        public Handler(FastFoodContext dbContext, IMapper mapper, IOrderHubNotifier orderHubNotifier) : base(dbContext, mapper)
        {
            _orderHubNotifier = orderHubNotifier;
        }

        public override async Task<OrderDTO> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var requestedLines = request.Dto.ProductVariantOrders?.ToList() ?? new List<ProductVariantOrderDTO>();
            if (!requestedLines.Any())
            {
                throw new OrderValidationException("The order must contain at least one product.");
            }

            if (requestedLines.Any(line => line.Quantity <= 0))
            {
                throw new OrderValidationException("Every ordered product must have a positive quantity.");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var orderedQuantities = requestedLines
                .GroupBy(line => line.ProductVariant_FK)
                .ToDictionary(group => group.Key, group => group.Sum(line => line.Quantity));

            var productVariants = await _dbContext.ProductVariants
                .Include(variant => variant.Product)
                .Include(variant => variant.ProductVariantIngredients)
                    .ThenInclude(recipe => recipe.Ingredient)
                .Where(variant => orderedQuantities.Keys.Contains(variant.Id))
                .ToListAsync(cancellationToken);

            var missingVariantIds = orderedQuantities.Keys.Except(productVariants.Select(variant => variant.Id)).ToList();
            if (missingVariantIds.Any())
            {
                throw new OrderValidationException($"Product variants not found: {string.Join(", ", missingVariantIds)}.");
            }

            var ingredientRequirements = productVariants
                .SelectMany(variant => variant.ProductVariantIngredients.Select(recipe => new
                {
                    Ingredient = recipe.Ingredient,
                    Quantity = recipe.Quantity * orderedQuantities[variant.Id]
                }))
                .GroupBy(requirement => requirement.Ingredient.Id)
                .Select(group => new
                {
                    Ingredient = group.First().Ingredient,
                    Quantity = group.Sum(requirement => requirement.Quantity)
                })
                .ToList();

            var shortages = ingredientRequirements
                .Where(requirement => requirement.Ingredient.Quantity < requirement.Quantity)
                .Select(requirement =>
                    $"{requirement.Ingredient.Name}: required {requirement.Quantity} {GetUnitLabel(requirement.Ingredient.Unit)}, available {requirement.Ingredient.Quantity} {GetUnitLabel(requirement.Ingredient.Unit)}")
                .ToList();

            if (shortages.Any())
            {
                throw new OrderValidationException($"Insufficient inventory: {string.Join("; ", shortages)}.");
            }

            var order = _mapper.Map<Order>(request.Dto);

            order.OrderReceivedDateTime = DateTime.Now;
            order.OrderReadyDateTime = null;
            order.OrderDeliveredDateTime = null;
            order.OrderCancelledDateTime = null;

            // OrderNumber
            var startOfDay = order.OrderReceivedDateTime.Date;
            var nextDay = startOfDay.AddDays(1);

            var lastNumberToday = await _dbContext.Orders
                .Where(o => o.OrderReceivedDateTime >= startOfDay && o.OrderReceivedDateTime < nextDay)
                .MaxAsync(o => (int?)o.OrderNumber) ?? 0;

            order.OrderNumber = lastNumberToday + 1;

            // TotalPrice
            order.TotalPrice = order.ProductVariantOrders.Sum(x => x.UnitPrice * x.Quantity);

            foreach (var requirement in ingredientRequirements)
            {
                requirement.Ingredient.Quantity -= requirement.Quantity;
            }

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var orderDto = _mapper.Map<OrderDTO>(order);
            await _orderHubNotifier.NotifyOrderCreated(orderDto);

            return orderDto;
        }

        private static string GetUnitLabel(int unit) => unit switch
        {
            1 => "g",
            2 => "ml",
            3 => "pcs",
            _ => "units"
        };
    }
}