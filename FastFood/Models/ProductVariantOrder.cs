namespace FastFood.Models;

public class ProductVariantOrder
{
    public int Id { get; set; }
    public int ProductVariant_FK { get; set; }
    public int Order_FK { get; set; }
    public int Quantity { get; set; }
    public int UnitPrice { get; set; }

    public ProductVariant ProductVariant { get; set; }
    public Order Order { get; set; }
}
