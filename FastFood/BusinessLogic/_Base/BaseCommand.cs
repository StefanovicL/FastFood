using MediatR;

namespace FastFood.BusinessLogic._Base;

public abstract class BaseCommand<TResponse> : IRequest<TResponse>
{
}
