using FastFood.Enums;

namespace FastFood.Dtos;

public class IngredientDTO
{
    public IngredientDTO()
    {
        ProductVariantIngredients = new HashSet<ProductVariantIngredientDTO>();
    }

    public int Id { get; set; }
    public int Quantity { get; set; } // ukupna kolicina u magacinu
    public string Name { get; set; }
    public eUnitType Unit { get; set; }

    public ICollection<ProductVariantIngredientDTO> ProductVariantIngredients { get; set; }
}
