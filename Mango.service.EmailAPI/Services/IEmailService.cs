using Mango.Services.RewardAPI.Message;

namespace Mango.service.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDTO cartDTO);
        Task RegisterUserEmailandLog(string Email);
        Task LogOrderPlaced(RewardsMessage rewardsMessage);
    }
}
