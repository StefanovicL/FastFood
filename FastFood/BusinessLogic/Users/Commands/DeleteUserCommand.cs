using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.BusinessLogic.Users.Commands;

public class DeleteUserCommand : BaseCommand<UserDTO>
{
    private int Id { get; set; }
    public DeleteUserCommand(int id)
    {
        Id = id;
    }

    public class Handler : BaseHandler<DeleteUserCommand, UserDTO>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<UserDTO> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            if (user == null)
            {
                throw new Exception("User ne postoji.");
            }

            // UserRole rows must be removed explicitly: the FK constraint is not ON DELETE CASCADE in the database.
            _dbContext.UserRoles.RemoveRange(user.UserRoles);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<UserDTO>(user);
        }
    }
}