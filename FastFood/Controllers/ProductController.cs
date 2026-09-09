using FastFood.BusinessLogic.Products.Commands;
using FastFood.BusinessLogic.Products.Queries;
using FastFood.BusinessLogic.ProductVariants.Commands;
using FastFood.Dtos;
using FastFood.Models;
using FastFood.ReinforcedTypings.Generator;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reinforced.Typings.Attributes;

namespace FastFood.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly FastFoodContext _context;
    private readonly IMediator _mediator;
    private readonly IWebHostEnvironment _env;

    public ProductController(FastFoodContext context, IMediator mediator, IWebHostEnvironment env)
    {
        _context = context;
        _mediator = mediator;
        _env = env;
    }

    [HttpGet]
    [AngularMethod(typeof(List<ProductDTO>))]
    public async Task<ActionResult<List<ProductDTO>>> GetAllProducts()
    {
        var result = await _mediator.Send(new GetAllProductsQuery());

        return Ok(result);
    }

    [HttpPost]
    [AngularMethod(typeof(ProductDTO))]
    public async Task<ActionResult<ProductDTO>> CreateProduct([FromBody] ProductDTO dto)
    {
        var result = await _mediator.Send(new CreateProductCommand(dto));

        return Ok(result);
    }

    [HttpPost]
    [AngularMethod(typeof(ProductDTO))]
    public async Task<ActionResult<ProductDTO>> UpdateProduct([FromBody] ProductDTO dto)
    {
        var result = await _mediator.Send(new UpdateProductCommand(dto));

        return Ok(result);
    }

    [HttpPost]
    [AngularMethod(typeof(ProductVariantDTO))]
    public async Task<ActionResult<ProductVariantDTO>> UpdateProductVariant([FromBody] ProductVariantDTO dto)
    {
        var result = await _mediator.Send(new UpdateProductVariantCommand(dto));

        return Ok(result);
    }

    [HttpPost]
    [AngularMethod(typeof(ProductVariantDTO))]
    public async Task<ActionResult<ProductVariantDTO>> DeleteProductVariant([FromBody] int id)
    {
        var result = await _mediator.Send(new DeleteProductVariantCommand(id));

        return Ok(result);
    }

    [HttpPost]
    [AngularMethod(typeof(List<int>))]
    public async Task<ActionResult<List<int>>> BulkDeleteProductVariants([FromBody] List<int> ids)
    {
        var result = await _mediator.Send(new BulkDeleteProductVariantsCommand(ids));

        return Ok(result);
    }

    [HttpPost]
    [AngularMethod(typeof(ProductDTO))]
    public async Task<ActionResult<int>> DeleteProduct([FromBody] int id)
    {
        var result = await _mediator.Send(new DeleteProductCommand(id));

        return Ok(result);
    }

    // Excluded from TS generation: multipart upload isn't supported by the Reinforced.Typings generator.
    [HttpPost]
    [TsIgnore]
    public async Task<ActionResult> UploadProductImage([FromForm] int id, [FromForm] IFormFile file)
    {
        var product = await _context.Products
            .Include(p => p.ProductVariants)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        var ext = Path.GetExtension(file.FileName);
        var storedFileName = Guid.NewGuid().ToString() + ext;
        var originalFileName = file.FileName;

        var imagesFolder = Path.Combine(_env.ContentRootPath, "..", "fast-food-app", "src", "assets", "demo", "images", "burgers");
        Directory.CreateDirectory(imagesFolder);

        var filePath = Path.Combine(imagesFolder, storedFileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        foreach (var variant in product.ProductVariants)
        {
            variant.OriginalFileName = originalFileName;
            variant.StoredFileName = storedFileName;
        }

        await _context.SaveChangesAsync();

        return Ok();
    }
}
