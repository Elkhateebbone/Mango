using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDTO?> AssignRoleAsync(RegestirationrequestDTO regestirationrequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = regestirationrequestDTO,
                Url = SD.AuthAPIBase + "/api/auth/assignRole"
            });
        }

        public async Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {

            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDTO,
                Url = SD.AuthAPIBase + "/api/auth/login"
            });
        }

        public async Task<ResponseDTO?> ResgisterAsync(RegestirationrequestDTO regestirationrequestDTO)
        {
            try
            {
                return await _baseService.SendAsync(new RequestDTO()
                {
                    ApiType = SD.ApiType.POST,
                    Data = regestirationrequestDTO,
                    Url = SD.AuthAPIBase + "/api/auth/register"
                });
            }
            catch(Exception ex)
            {

            }
            return null;
        }
    }
}
