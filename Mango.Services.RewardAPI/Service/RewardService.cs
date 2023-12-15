using Mango.Services.RewardAPI;
using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.service.EmailAPI.Services
{
    public class RewardService : IRewardsService
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;
        public RewardService(DbContextOptions<AppDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

      
        public async Task UpdateRewards(RewardsMessage rewardsMessage)
        {
            try
            {
                Rewards rewards = new()
                {
                    OrderId = rewardsMessage.OrderId,
                    RewardsActivities=rewardsMessage.RewardsActivity ,
                    UserId = rewardsMessage.UserId,
                    RewardsDate = DateTime.Now,
                };
               await  using var _db = new AppDbContext(_dbContextOptions);
             await  _db.Rewards.AddAsync(rewards);
                await _db.SaveChangesAsync();




            }
            catch (Exception)
            {

            }
        }
    }
}
