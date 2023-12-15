using Mango.service.EmailAPI.Models;
using Mango.Service.EmailAPI.Data;
using Mango.Services.OrderAPI.Models.DTO;
using Mango.Services.RewardAPI.Message;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.service.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;
        public EmailService(DbContextOptions<AppDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }
        public async Task EmailCartAndLog(CartDTO cartDTO)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("<br/> Cart Email Requested");
            message.AppendLine("<br/>Total " + cartDTO.CartHeader.CartTotal);
            message.AppendLine("<br/>");
            message.AppendLine("<ul>");
            foreach(var item in cartDTO.CartDetails)
            {
                message.Append("<li>");
                message.Append(item.Product.Name +"X" + item.Count );
                message.Append("</li>");
            }
            message.Append("</ul>");

            await LogAndEmail(message.ToString(),cartDTO.CartHeader.Email);

        }

        public async Task LogOrderPlaced(RewardsMessage rewardsDTO)
        {
            string message = "New Order Placed <br/> order ID:" + rewardsDTO.OrderId;
            await LogAndEmail(message, "elkhateebelkhairi565@gmail.com");

        }

        public Task RegisterUserEmailandLog(string Email)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> LogAndEmail(string message , string Email)
        {
            try
            {
                EmailLogger emailLogger = new()
                {
                    Email = Email,
                    EmailSent = DateTime.Now,
                    Message = message
                };
               await  using var _db = new AppDbContext(_dbContextOptions);
             await  _db.EmailLoggers.AddAsync(emailLogger);
                await _db.SaveChangesAsync();



                return true;

            }
            catch (Exception)
            {

               return false;
            }
        }
    }
}
