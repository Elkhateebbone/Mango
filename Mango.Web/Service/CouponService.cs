using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class CouponService : IProductService
    {
        private readonly IBaseService _baseService;
        public CouponService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDTO> DeleteCouponAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.CouponAPIBase + "/api/coupon/" + id
            });
        }

        public async Task<ResponseDTO> GetAllCouponsAsync()
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/coupon"
            });
        }

        public async Task<ResponseDTO> GetCouponAsync(string couponCode)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/coupon/GetByCode" + couponCode
            });
        }



        public async Task<ResponseDTO> GetCouponByIdAsync(int id)
        {
            {
                return await _baseService.SendAsync(new RequestDTO()
                {
                    ApiType = SD.ApiType.GET,
                    Url = SD.CouponAPIBase + "/api/coupon/" + id
                });
            }
        }

        public async Task<ResponseDTO> CreateCouponAsync(CouponDto couponDto)
        {
            {
                return await _baseService.SendAsync(new RequestDTO()
                {
                    ApiType = SD.ApiType.POST,
                    Data = couponDto,
                    Url = SD.CouponAPIBase + "/api/coupon"
                });
            }
        }

        public async Task<ResponseDTO> UpdateCouponAsync(CouponDto couponDto)
        {
            {
                return await _baseService.SendAsync(new RequestDTO()
                {
                    ApiType = SD.ApiType.PUT,
                    Data = couponDto,
                    Url = SD.CouponAPIBase + "/api/coupon"
                });
            }
        }
    }                                                                                                               
}
