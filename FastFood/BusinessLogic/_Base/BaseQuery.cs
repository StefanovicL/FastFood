using MediatR;

namespace FastFood.BusinessLogic._Base;

public abstract class BaseQuery<TResponse> : IRequest<TResponse>
{
}
