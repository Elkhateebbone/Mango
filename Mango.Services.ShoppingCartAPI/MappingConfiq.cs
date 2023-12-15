using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;

namespace Mango.Services.ShoppingCartAPI
{
    public class MappingConfiq
    {
        public static MapperConfiguration RegisterMap()
        {
            var mappingConfig = new MapperConfiguration(confiq =>
            {

                confiq.CreateMap<CartHeader, CartHeaderDTO>().ReverseMap();
                confiq.CreateMap<CartDetails, CartDetailsDTO>().ReverseMap();

            }

            )  ;
            return mappingConfig;

        }
    }
}
