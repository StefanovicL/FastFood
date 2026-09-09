using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;

namespace FastFood.BusinessLogic.Roles.Commands;

public class CreateRoleCommand : BaseCommand<RoleDTO>
{
    private RoleDTO Dto { get; set; }
    public CreateRoleCommand(RoleDTO dto)
    {
        Dto = dto;
    }

    public class Handler : BaseHandler<CreateRoleCommand, RoleDTO>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<RoleDTO> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = _mapper.Map<Role>(request.Dto);

            _dbContext.Roles.Add(role);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<RoleDTO>(role);
        }
    }
}