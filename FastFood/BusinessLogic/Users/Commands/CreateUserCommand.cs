using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.BusinessLogic.Users.Commands;

public class CreateUserCommand : BaseCommand<UserDTO>
{
    private UserDTO Dto { get; set; }
    public CreateUserCommand(UserDTO dto) 
    {
        Dto = dto;
    }
    
    public class Handler : BaseHandler<CreateUserCommand, UserDTO>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<UserDTO> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == request.Dto.RoleId, cancellationToken);

            if (role == null)
            {
                throw new Exception("Rola ne postoji.");
            }

            var user = _mapper.Map<User>(request.Dto);

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var userRole = new UserRole
            {
                User_FK = user.Id,
                Role_FK = role.Id
            };

            _dbContext.UserRoles.Add(userRole);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<UserDTO>(user);
        }
    }
}