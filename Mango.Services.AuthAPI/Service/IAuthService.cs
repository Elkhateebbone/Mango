using Mango.Services.AuthAPI.Models.DTO;

namespace Mango.Services.AuthAPI.Service
{
    public interface IAuthService
    {
        Task<string> Register(RegestirationrequestDTO regestirationrequestDTO);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<bool> AssignRole(string email , string roleName);

    }
}
