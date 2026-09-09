using FastFood.Enums;

namespace FastFood.Dtos;

public class OrderDTO
{
    public OrderDTO()
    {
        ProductVariantOrders = new HashSet<ProductVariantOrderDTO>();
    }

    public int Id { get; set; }
    public DateTime OrderReceivedDateTime { get; set; }
    public DateTime? OrderReadyDateTime { get; set; }
    public DateTime? OrderDeliveredDateTime { get; set; }
    public DateTime? OrderCancelledDateTime { get; set; }
    public eOrderState State { get; set; }
    public decimal TotalPrice { get; set; }
    public int OrderNumber { get; set; }

    public ICollection<ProductVariantOrderDTO> ProductVariantOrders { get; set; }
}