using FastFood.Enums;

namespace FastFood.Dtos;

public class ProductVariantDTO
{
    public ProductVariantDTO()
    {
        ProductVariantIngredients = new HashSet<ProductVariantIngredientDTO>();
        ProductVariantOrders = new HashSet<ProductVariantOrderDTO>();
    }

    public int Id { get; set; }
    public int Product_FK { get; set; }
    public eProductSize Size { get; set; }
    public int Price { get; set; }
    public string? OriginalFileName { get; set; }
    public string? StoredFileName { get; set; }
    public string? ProductName { get; set; }

    public ICollection<ProductVariantIngredientDTO> ProductVariantIngredients { get; set; }
    public ICollection<ProductVariantOrderDTO> ProductVariantOrders { get; set; }
}
