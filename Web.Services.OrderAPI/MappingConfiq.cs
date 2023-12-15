using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.DTO;

namespace Mango.Services.ShoppingCartAPI
{
    public class MappingConfiq
    {
        public static MapperConfiguration RegisterMap()
        {
            var mappingConfig = new MapperConfiguration(confiq =>
            {

                confiq.CreateMap<OrderHeaderDTO, CartHeaderDTO>().ForMember(dest=>dest.CartTotal,u=>u.MapFrom(src=>src.CartTotal)).ReverseMap();
                confiq.CreateMap<CartDetailsDTO, OrderDetailsDTO>().ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));
                confiq.CreateMap<OrderHeader, OrderHeaderDTO>().ReverseMap();
                confiq.CreateMap<OrderDetailsDTO, OrderDetails>().ReverseMap();
            });
            return mappingConfig;

        }
    }
}
