using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CarAuto.EFCore.BaseEntity.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : BaseEntity, new()
    {
        public GenericRepository(DbContext context, ILogger logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected DbContext Context { get; }

        protected ILogger Logger { get; }

        public TEntity InsertNewEntityFrom(TEntity fromEntity)
        {
            if (fromEntity == null)
            {
                throw GetArgumentNullExceptionAndLog(nameof(fromEntity));
            }

            // TODO: UserId
            //fromEntity.CreatedBy = UserId;
            Insert(fromEntity);
            return fromEntity;
        }

        public IQueryable<TEntity> Queryable
        {
            get
            {
                return Context.Set<TEntity>();
            }
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            var response = await Context.FindAsync<TEntity>(id).ConfigureAwait(false);
            if (response == null)
            {
                throw new KeyNotFoundException(id.ToString());
            }

            CheckEntity(response, true);
            return response;
        }

        public void Insert(TEntity entity)
        {
            CheckEntity(entity);
            Context.Set<TEntity>().Add(entity);
        }

        public async Task InsertAsync(TEntity entity)
        {
            CheckEntity(entity);
            await Context.Set<TEntity>().AddAsync(entity).ConfigureAwait(false);
        }

        public void InsertRange(IEnumerable<TEntity> entities)
        {
            CheckEntities(entities);
            Context.Set<TEntity>().AddRange(entities);
        }

        public async Task InsertRangeAsync(IEnumerable<TEntity> entities)
        {
            CheckEntities(entities);
            await Context.Set<TEntity>().AddRangeAsync(entities).ConfigureAwait(false);
        }

        public void Update(TEntity entityToUpdate)
        {
            CheckEntity(entityToUpdate);
            Context.Set<TEntity>().Update(entityToUpdate);
        }

        public void UpdateRange(IEnumerable<TEntity> entitiesToUpdate)
        {
            if (entitiesToUpdate == null)
            {
                throw new ArgumentNullException(nameof(entitiesToUpdate));
            }

            CheckEntities(entitiesToUpdate);
            Context.Set<TEntity>().UpdateRange(entitiesToUpdate);
        }

        public void Delete(TEntity entityToDelete)
        {
            CheckEntity(entityToDelete);
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                Context.Set<TEntity>().Attach(entityToDelete);
            }

            Context.Set<TEntity>().Remove(entityToDelete);
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            CheckEntities(entities);
            Context.Set<TEntity>().RemoveRange(entities);
        }

        public async Task DeleteAsync(Guid id)
        {
            TEntity entityToDelete = await Context.Set<TEntity>().FindAsync(id).ConfigureAwait(false);
            CheckEntity(entityToDelete, true);
            Context.Set<TEntity>().Remove(entityToDelete);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        protected ArgumentNullException GetArgumentNullExceptionAndLog(string param, [CallerMemberName] string callerName = "")
        {
            var ex = new ArgumentNullException(param);
            using (Logger.BeginScope(nameof(GenericRepository<TEntity>)))
            {
                Logger.LogError(ex, callerName + " cannot be called with null " + param + " parameter.");
            }

            return ex;
        }

        private void CheckEntity(TEntity entity, bool isResponse = false)
        {
            if (entity == null)
            {
                if (isResponse)
                {
                    throw new KeyNotFoundException(nameof(entity));
                }

                throw GetArgumentNullExceptionAndLog(nameof(entity));
            }
        }

        private void CheckEntities(IEnumerable<TEntity> entities, bool isResponse = false)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            foreach (var entity in entities)
            {
                CheckEntity(entity, isResponse);
            }
        }
    }
}
