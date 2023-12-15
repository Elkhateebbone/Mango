using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface ICartService
    {
       Task<ResponseDTO?> GetCartByUserIdAsync(string UserId);
        Task<ResponseDTO?> UpsertCartAsync(CartDTO cartDTO);
        Task<ResponseDTO?> RemoveFromCartAsync(int cartDetalsId);
        Task<ResponseDTO?> ApplyCouponAsync(CartDTO cartDTO);
        Task<ResponseDTO?> CartEmail(CartDTO cartDTO);


    }
}
