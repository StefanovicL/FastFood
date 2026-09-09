using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;

namespace FastFood.BusinessLogic.Roles.Commands;

public class UpdateRoleCommand : BaseCommand<RoleDTO>
{
    private RoleDTO Dto { get; set; }
    public UpdateRoleCommand(RoleDTO dto)
    {
        Dto = dto;
    }

    public class Handler : BaseHandler<UpdateRoleCommand, RoleDTO>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<RoleDTO> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _dbContext.Roles.FindAsync(request.Dto.Id);
            if (role == null)
            {
                throw new Exception("Rola ne postoji.");
            }

            role = _mapper.Map<Role>(request.Dto);

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<RoleDTO>(role);
        }
    }
}