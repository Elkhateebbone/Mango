using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;

namespace Mango.Services.ProductAPI
{
    public class MappingConfiq
    {
        public static MapperConfiguration RegisterMap()
        {
            var mappingConfig = new MapperConfiguration(confiq =>
            {

                confiq.CreateMap<Product, ProductDTO>().ReverseMap();
            }

            )  ;
            return mappingConfig;

        }
    }
}
