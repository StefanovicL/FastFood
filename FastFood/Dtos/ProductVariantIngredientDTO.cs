namespace FastFood.Dtos;

public class ProductVariantIngredientDTO
{
    public int Id { get; set; }
    public int ProductVariant_FK { get; set; }
    public int Ingredient_FK { get; set; }
    public int Quantity { get; set; } // koliko ide u toj varijanti burgera

    public ProductVariantDTO? ProductVariant { get; set; }
    public IngredientDTO? Ingredient { get; set; }
}
