using AutoMapper;
using FastFood.Models;
using MediatR;

namespace FastFood.BusinessLogic.Base;

public abstract class BaseHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    protected readonly IMapper _mapper;
    protected readonly FastFoodContext _dbContext;

    public BaseHandler(FastFoodContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}