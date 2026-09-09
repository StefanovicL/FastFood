using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.BusinessLogic.Users.Queries;

public class GetAllUsersQuery : BaseQuery<List<UserDTO>>
{
    public GetAllUsersQuery()
    {
    }

    public class Handler : BaseHandler<GetAllUsersQuery, List<UserDTO>>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<List<UserDTO>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<UserDTO>>(result);
        }
    }
}