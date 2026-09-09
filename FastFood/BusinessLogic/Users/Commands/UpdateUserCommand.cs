using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.BusinessLogic.Users.Commands;

public class UpdateUserCommand : BaseCommand<UserDTO>
{
    private UserDTO Dto { get; set; }
    public UpdateUserCommand(UserDTO dto)
    {
        Dto = dto;
    }

    public class Handler : BaseHandler<UpdateUserCommand, UserDTO>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<UserDTO> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == request.Dto.Id, cancellationToken);
            if (user == null)
            {
                throw new Exception("User ne postoji.");
            }

            user.Username = request.Dto.Username;

            if (!string.IsNullOrEmpty(request.Dto.Password))
            {
                user.Password = request.Dto.Password;
            }

            if (request.Dto.RoleId > 0 && !user.UserRoles.Any(ur => ur.Role_FK == request.Dto.RoleId))
            {
                var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == request.Dto.RoleId, cancellationToken);
                if (role == null)
                {
                    throw new Exception("Rola ne postoji.");
                }

                _dbContext.UserRoles.RemoveRange(user.UserRoles);
                user.UserRoles.Add(new UserRole { User_FK = user.Id, Role_FK = role.Id });
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<UserDTO>(user);
        }
    }
}