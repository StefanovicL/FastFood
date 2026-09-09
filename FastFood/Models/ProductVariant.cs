namespace FastFood.Models;

public class ProductVariant
{
    public ProductVariant()
    {
        ProductVariantIngredients = new HashSet<ProductVariantIngredient>();
        ProductVariantOrders = new HashSet<ProductVariantOrder>();
    }

    public int Id { get; set; }
    public int Product_FK { get; set; }
    public int Size { get; set; } // npr Single=1, Double=2, Triple=3
    public int Price { get; set; }
    public string? OriginalFileName { get; set; }
    public string? StoredFileName { get; set; }

    public Product Product { get; set; }
    public ICollection<ProductVariantIngredient> ProductVariantIngredients { get; set; }
    public ICollection<ProductVariantOrder> ProductVariantOrders { get; set; }
}
