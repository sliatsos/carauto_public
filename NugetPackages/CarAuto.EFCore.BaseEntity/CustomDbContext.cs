using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarAuto.ClaimParser;

namespace CarAuto.EFCore.BaseEntity;

public class CustomDbContext : DbContext
{
    private readonly IClaimParser _claimParser;

    public CustomDbContext(DbContextOptions options, IClaimParser claimParser)
        : base(options)
    {
        _claimParser = claimParser ?? throw new ArgumentNullException(nameof(claimParser));
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesInternal();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        SaveChangesInternal();
        return base.SaveChanges();
    }

    public void SaveChangesInternal()
    {
        var entities = (from entry in ChangeTracker.Entries()
                        where entry.State == EntityState.Modified || entry.State == EntityState.Added
                        select entry);

        foreach (var entity in entities)
        {
            var baseEntity = (BaseEntity)entity.Entity;
            switch (entity.State)
            {
                case EntityState.Added:
                    baseEntity.CreatedAt = DateTime.UtcNow;
                    baseEntity.CreatedBy = _claimParser.GetUserId();
                    break;
                case EntityState.Modified:
                    baseEntity.ModifiedAt = DateTime.UtcNow;
                    baseEntity.ModifiedBy = _claimParser.GetUserId();
                    break;
            }
        }
    }
}
