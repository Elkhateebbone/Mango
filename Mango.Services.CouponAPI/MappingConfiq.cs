using AutoMapper;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;

namespace Mango.Services.CouponAPI
{
    public class MappingConfiq
    {
        public static MapperConfiguration RegisterMap()
        {
            var mappingConfig = new MapperConfiguration(confiq =>
            {

                confiq.CreateMap<Coupon, CouponDto>();
                confiq.CreateMap<CouponDto, Coupon>();
            }

            )  ;
            return mappingConfig;

        }
    }
}
