using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO);
        Task<ResponseDTO?> ResgisterAsync(RegestirationrequestDTO regestirationrequestDTO);
        Task<ResponseDTO?> AssignRoleAsync(RegestirationrequestDTO regestirationrequestDTO);

    }
}
