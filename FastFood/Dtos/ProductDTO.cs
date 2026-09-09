namespace FastFood.Dtos;

public class ProductDTO
{
    public ProductDTO()
    {
        ProductVariants = new HashSet<ProductVariantDTO>();
        ProductOrders = new HashSet<ProductVariantOrderDTO>();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<ProductVariantDTO> ProductVariants { get; set; }
    public ICollection<ProductVariantOrderDTO> ProductOrders { get; set; }
}
