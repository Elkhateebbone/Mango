using Mango.Web.Models;

namespace Mango.Services.AuthAPI.Models.DTO
{
    public class UserResponseDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }
    }
}
