using FastFood.BusinessLogic.Roles.Commands;
using FastFood.BusinessLogic.Roles.Queries;
using FastFood.Dtos;
using FastFood.Models;
using FastFood.ReinforcedTypings.Generator;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly FastFoodContext _context;
    private readonly IMediator _mediator;

    public RoleController(FastFoodContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    [HttpGet]
    [AngularMethod(typeof(RoleDTO))]
    public async Task<ActionResult<List<RoleDTO>>> GetAllRoles()
    {
        var result = await _mediator.Send(new GetAllRolesQuery());

        return Ok(result);
    }

    [HttpPost]
    [AngularMethod(typeof(RoleDTO))]
    public async Task<ActionResult<RoleDTO>> CreateRole([FromBody] RoleDTO dto)
    {
        var result = await _mediator.Send(new CreateRoleCommand(dto));

        return Ok(result);
    }

    [HttpPost]
    [AngularMethod(typeof(RoleDTO))]
    public async Task<ActionResult<RoleDTO>> UpdateRole([FromBody] RoleDTO dto)
    {
        var result = await _mediator.Send(new UpdateRoleCommand(dto));

        return Ok(result);
    }

    [HttpPost]
    [AngularMethod(typeof(RoleDTO))]
    public async Task<ActionResult<int>> DeleteRole([FromBody] int id)
    {
        var result = await _mediator.Send(new DeleteRoleCommand(id));

        return Ok(result);
    }
}
