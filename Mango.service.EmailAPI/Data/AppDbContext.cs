using Mango.service.EmailAPI.Models;
using Mango.Service.EmailAPI.Data;

using Microsoft.EntityFrameworkCore;

namespace Mango.Service.EmailAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<EmailLogger> EmailLoggers { get; set; }


    }
}
