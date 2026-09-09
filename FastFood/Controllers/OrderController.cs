using FastFood.BusinessLogic.Orders.Commands;
using FastFood.BusinessLogic.Orders.Queries;
using FastFood.Dtos;
using FastFood.Models;
using FastFood.ReinforcedTypings.Generator;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly FastFoodContext _context;
    private readonly IMediator _mediator;

    public OrderController(FastFoodContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    [HttpGet]
    [AngularMethod(typeof(OrderDTO))]
    public async Task<ActionResult<List<OrderDTO>>> GetAllOrders()
    {
        var result = await _mediator.Send(new GetAllOrdersQuery());

        return Ok(result);
    }

    [HttpGet]
    [AngularMethod(typeof(OrderDTO))]
    public async Task<ActionResult<List<OrderDTO>>> GetActiveOrders()
    {
        var result = await _mediator.Send(new GetActiveOrdersQuery());

        return Ok(result);
    }

    [HttpPost]
    [AngularMethod(typeof(OrderDTO))]
    public async Task<ActionResult<OrderDTO>> CreateOrder([FromBody] OrderDTO dto)
    {
        try
        {
            var result = await _mediator.Send(new CreateOrderCommand(dto));

            return Ok(result);
        }
        catch (OrderValidationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost]
    [AngularMethod(typeof(OrderDTO))]
    public async Task<ActionResult<OrderDTO>> UpdateOrder([FromBody] OrderDTO dto)
    {
        var result = await _mediator.Send(new UpdateOrderCommand(dto));

        return Ok(result);
    }

    [HttpPost]
    [AngularMethod(typeof(OrderDTO))]
    public async Task<ActionResult<int>> DeleteOrder([FromBody] int id)
    {
        var result = await _mediator.Send(new DeleteOrderCommand(id));

        return Ok(result);
    }
}
