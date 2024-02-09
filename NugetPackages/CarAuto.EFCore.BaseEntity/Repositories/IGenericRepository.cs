using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CarAuto.EFCore.BaseEntity.Repositories
{
    public interface IGenericRepository<TEntity>
        where TEntity : BaseEntity, new()
    {
        TEntity InsertNewEntityFrom(TEntity fromEntity);

        Task<TEntity> GetByIdAsync(Guid id);

        IQueryable<TEntity> Queryable { get; }

        void Insert(TEntity entity);

        Task InsertAsync(TEntity entity);

        void InsertRange(IEnumerable<TEntity> entities);

        Task InsertRangeAsync(IEnumerable<TEntity> entities);

        void Update(TEntity entityToUpdate);

        void UpdateRange(IEnumerable<TEntity> entitiesToUpdate);

        void Delete(TEntity entityToDelete);

        Task DeleteAsync(Guid id);

        void DeleteRange(IEnumerable<TEntity> entities);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        void SaveChanges();
    }
}
