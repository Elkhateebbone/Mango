using Mango.Services.AuthAPI.Models;

namespace Mango.Services.AuthAPI.Service
{
    public interface IjwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser,IEnumerable<string> roles);

    }
}
