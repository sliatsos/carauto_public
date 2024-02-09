using CarAuto.ClaimParser;
using CarAuto.EFCore.BaseEntity;
using CarAuto.UserService.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarAuto.UserService.DAL.Context
{
    public class UserDbContext : CustomDbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options, IClaimParser claimParser)
            : base(options, claimParser)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Salesperson> Salespersons { get; set; }
    }
}
