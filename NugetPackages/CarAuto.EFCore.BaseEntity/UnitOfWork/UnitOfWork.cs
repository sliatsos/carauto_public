using CarAuto.EFCore.BaseEntity.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CarAuto.EFCore.BaseEntity.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dbContext;
        private readonly ILogger<UnitOfWork> _logger;

        private readonly Dictionary<Type, object> _repositories = new();

        private bool _disposed;

        public UnitOfWork(DbContext entities, ILogger<UnitOfWork> logger)
        {
            _dbContext = entities ?? throw new ArgumentNullException(nameof(entities));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>()
            where TEntity : BaseEntity, new()
        {
            if (_repositories.ContainsKey(typeof(TEntity)))
            {
                return _repositories[typeof(TEntity)] as IGenericRepository<TEntity>;
            }

            IGenericRepository<TEntity> repository = new GenericRepository<TEntity>(_dbContext, _logger);
            _repositories.Add(typeof(TEntity), repository);
            return repository;
        }

        public async Task<IGenericRepository<TEntity>> GetRepositoryAsync<TEntity>()
            where TEntity : BaseEntity, new()
        {
            return await Task.Run(() => GetRepository<TEntity>()).ConfigureAwait(false);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [ExcludeFromCodeCoverage]
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed && disposing)
            {
                _dbContext.Dispose();
            }

            this._disposed = true;
        }

        protected void CleanCached()
        {
            _repositories.Clear();
        }
    }
}
