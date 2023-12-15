using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IProductService
    {
       Task<ResponseDTO> GetCouponAsync(string Couponcode);
        Task<ResponseDTO> GetAllCouponsAsync();
        Task<ResponseDTO> GetCouponByIdAsync(int id);
        Task<ResponseDTO> UpdateCouponAsync(CouponDto couponDto);
        Task<ResponseDTO> DeleteCouponAsync(int id);
        Task<ResponseDTO> CreateCouponAsync(CouponDto couponDto);

    }
}
