using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;

namespace FastFood.BusinessLogic.Products.Commands;

public class DeleteProductCommand : BaseCommand<ProductDTO>
{
    private int Id { get; set; }
    public DeleteProductCommand(int id)
    {
        Id = id;
    }

    public class Handler : BaseHandler<DeleteProductCommand, ProductDTO>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<ProductDTO> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.FindAsync(request.Id);
            if (product == null)
            {
                throw new Exception("Proizvod ne postoji.");
            }

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<ProductDTO>(product);
        }
    }
}