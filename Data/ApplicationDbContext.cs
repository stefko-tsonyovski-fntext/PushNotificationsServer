using Microsoft.EntityFrameworkCore;
using PWAPushNotificationsServer.Models;

namespace PWAPushNotificationsServer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Subscription> Subscriptions { get; set; }
    }
}
