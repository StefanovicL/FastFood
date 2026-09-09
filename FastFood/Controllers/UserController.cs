using FastFood.BusinessLogic.Users.Commands;
using FastFood.BusinessLogic.Users.Queries;
using FastFood.Dtos;
using FastFood.Models;
using FastFood.ReinforcedTypings.Generator;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly FastFoodContext _context;
    private readonly IMediator _mediator;

    public UserController(FastFoodContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    [HttpGet]
    [AngularMethod(typeof(UserDTO))]
    public async Task<ActionResult<List<UserDTO>>> GetAllUsers()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());

        return Ok(result);
    }

    [HttpPost]
    [AngularMethod(typeof(UserDTO))]
    public async Task<ActionResult<UserDTO>> CreateUser([FromBody] UserDTO dto)
    {
        var result = await _mediator.Send(new CreateUserCommand(dto));

        return Ok(result);
    }

    [HttpPost]
    [AngularMethod(typeof(UserDTO))]
    public async Task<ActionResult<UserDTO>> UpdateUser([FromBody] UserDTO dto)
    {
        var result = await _mediator.Send(new UpdateUserCommand(dto));

        return Ok(result);
    }

    [HttpPost]
    [AngularMethod(typeof(UserDTO))]
    public async Task<ActionResult<int>> DeleteUser([FromBody] int id)
    {
        var result = await _mediator.Send(new DeleteUserCommand(id));

        return Ok(result);
    }
}
