namespace FastFood.Dtos;

public class ProductVariantOrderDTO
{
    public int Id { get; set; }
    public int ProductVariant_FK { get; set; }
    public int Order_FK { get; set; }
    public int Quantity { get; set; }
    public int UnitPrice { get; set; }

    public ProductVariantDTO? ProductVariant { get; set; }
    public OrderDTO? Order { get; set; }
}
