using AutoMapper;
using FastFood.Dtos;
using FastFood.Enums;
using FastFood.Models;

namespace FastFood.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        LoadStandardMappings();
    }

    private void LoadStandardMappings()
    {
        CreateMap<User, UserDTO>().ReverseMap();

        CreateMap<Product, ProductDTO>().ReverseMap();

        CreateMap<Order, OrderDTO>()
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => (eOrderState)src.State))
            .ReverseMap()
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => (int)src.State));

        CreateMap<Role, RoleDTO>()
            .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<Ingredient, IngredientDTO>()
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => (eUnitType)src.Unit))
            .ForMember(dest => dest.ProductVariantIngredients, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => (int)src.Unit));

        CreateMap<ProductVariant, ProductVariantDTO>()
            .ForMember(dest => dest.Size, opt => opt.MapFrom(src => (eProductSize)src.Size))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
            .ForMember(dest => dest.ProductVariantOrders, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.Size, opt => opt.MapFrom(src => (int)src.Size));

        CreateMap<ProductVariantIngredient, ProductVariantIngredientDTO>()
            .ForMember(dest => dest.ProductVariant, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<ProductVariantOrder, ProductVariantOrderDTO>()
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<UserRole, UserRoleDTO>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ReverseMap();
    }

}