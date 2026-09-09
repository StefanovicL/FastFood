using AutoMapper;
using FastFood.BusinessLogic._Base;
using FastFood.BusinessLogic.Base;
using FastFood.Dtos;
using FastFood.Models;

namespace FastFood.BusinessLogic.Products.Commands;

public class CreateProductCommand : BaseCommand<ProductDTO>
{
    private ProductDTO Dto { get; set; }
    public CreateProductCommand(ProductDTO dto)
    {
        Dto = dto;
    }

    public class Handler : BaseHandler<CreateProductCommand, ProductDTO>
    {
        public Handler(FastFoodContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<ProductDTO> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<Product>(request.Dto);

            _dbContext.Products.Add(product);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ProductDTO>(product);
        }
    }
}