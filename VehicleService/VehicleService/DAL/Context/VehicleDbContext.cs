using CarAuto.ClaimParser;
using CarAuto.EFCore.BaseEntity;
using CarAuto.VehicleService.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarAuto.VehicleService.DAL.Context
{
    public class VehicleDbContext : CustomDbContext
    {
        public VehicleDbContext(DbContextOptions<VehicleDbContext> options, IClaimParser claimParser)
            : base(options, claimParser)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }
    }
}
