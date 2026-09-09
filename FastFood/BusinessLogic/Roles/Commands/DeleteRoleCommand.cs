using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;

namespace FastFood.BusinessLogic.Roles.Commands;

public class DeleteRoleCommand : BaseCommand<RoleDTO>
{
    private int Id { get; set; }
    public DeleteRoleCommand(int id)
    {
        Id = id;
    }

    public class Handler : BaseHandler<DeleteRoleCommand, RoleDTO>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<RoleDTO> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _dbContext.Roles.FindAsync(request.Id);
            if (role == null)
            {
                throw new Exception("Rola ne postoji.");
            }

            _dbContext.Roles.Remove(role);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<RoleDTO>(role);
        }
    }
}