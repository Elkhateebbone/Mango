using Mango.Services.RewardAPI.Message;

namespace Mango.Services.RewardAPI
{
    public interface IRewardsService
    {
        Task UpdateRewards(RewardsMessage rewardsMessage);

    }
}
