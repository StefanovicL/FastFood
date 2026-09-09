using FastFood.BusinessLogic.Ingredients.Commands;
using FastFood.BusinessLogic.Ingredients.Queries;
using FastFood.Dtos;
using FastFood.Models;
using FastFood.ReinforcedTypings.Generator;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class IngredientController : ControllerBase
{
    private readonly FastFoodContext _context;
    private readonly IMediator _mediator;

    public IngredientController(FastFoodContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    [HttpGet]
    [AngularMethod(typeof(IngredientDTO))]
    public async Task<ActionResult<List<IngredientDTO>>> GetAllIngredients()
    {
        var result = await _mediator.Send(new GetAllIngredientsQuery());

        return Ok(result);
    }

    [HttpGet]
    [AngularMethod(typeof(IngredientDTO))]
    public async Task<ActionResult<List<IngredientDTO>>> GetProductIngredientsByProductId(int id)
    {
        var result = await _mediator.Send(new GetProductIngredientsByProductVariantIdQuery(id));

        return Ok(result);
    }

    [HttpPost]
    [AngularMethod(typeof(IngredientDTO))]
    public async Task<ActionResult<IngredientDTO>> CreateIngredient([FromBody] IngredientDTO dto)
    {
        var result = await _mediator.Send(new CreateIngredientCommand(dto));

        return Ok(result);
    }

    [HttpPost]
    [AngularMethod(typeof(IngredientDTO))]
    public async Task<ActionResult<IngredientDTO>> UpdateIngredient([FromBody] IngredientDTO dto)
    {
        var result = await _mediator.Send(new UpdateIngredientCommand(dto));

        return Ok(result);
    }

    [HttpPost]
    [AngularMethod(typeof(IngredientDTO))]
    public async Task<ActionResult<int>> DeleteIngredient([FromBody] int id)
    {
        var result = await _mediator.Send(new DeleteIngredientCommand(id));

        return Ok(result);
    }
}
