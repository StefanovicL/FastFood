namespace FastFood.Models;

public class ProductVariantIngredient
{
    public int Id { get; set; }
    public int ProductVariant_FK { get; set; }
    public int Ingredient_FK { get; set; }
    public int Quantity { get; set; } // koliko ide u toj varijanti burgera

    public ProductVariant ProductVariant { get; set; }
    public Ingredient Ingredient { get; set; }
}

