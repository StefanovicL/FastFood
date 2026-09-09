using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.BusinessLogic.Roles.Queries;

public class GetAllRolesQuery : BaseQuery<List<RoleDTO>>
{
    public GetAllRolesQuery()
    {
    }

    public class Handler : BaseHandler<GetAllRolesQuery, List<RoleDTO>>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<List<RoleDTO>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Roles.ToListAsync(cancellationToken);

            return _mapper.Map<List<RoleDTO>>(result);
        }
    }
}