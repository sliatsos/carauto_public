using CarAuto.EFCore.BaseEntity.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarAuto.EFCore.BaseEntity.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TEntity> GetRepository<TEntity>()
            where TEntity : BaseEntity, new();

        Task<IGenericRepository<TEntity>> GetRepositoryAsync<TEntity>()
            where TEntity : BaseEntity, new();

        void SaveChanges();

        Task SaveChangesAsync();
    }
}
