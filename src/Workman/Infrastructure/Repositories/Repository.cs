using Hearth.Prism.Toolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.ObjectModel;
using Workman.Core.Entities;
using Workman.Core.Repositories;
using Workman.Infrastructure.DbContexts;

namespace Workman.Infrastructure.Repositories
{
    internal class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly WorkmanDbContext _baseDbContext;
        protected readonly DbSet<TEntity> _baseDbSet;

        public Repository(WorkmanDbContext context)
        {
            _baseDbContext = context;
            _baseDbSet = _baseDbContext.Set<TEntity>();
        }

        public virtual async Task<bool> Delete(object key) =>
            await DbContextQueuedTask.Run(async () =>
            {
                TEntity? entity = await Query(key) ?? throw new ArgumentException("Entity not found");
                _baseDbSet.Remove(entity);
                int changes = 0;
                try
                {
                    changes = await _baseDbContext.SaveChangesAsync();
                }
                catch
                {
                    _baseDbSet.Add(entity);
                    throw;
                }
                return changes > 0;
            });

        public virtual async Task<bool> DeleteRange(params object[] keys) =>
            await DbContextQueuedTask.Run(async () =>
            {
                IEnumerable<TEntity> entities = _baseDbSet;
                if (keys != null && keys.Length > 0)
                {
                    entities = await QueryRange(keys);
                    if (entities == null || !entities.Any())
                    {
                        throw new ArgumentException("Entities not found");
                    }
                }
                _baseDbSet.RemoveRange(entities);

                int changes = 0;
                try
                {
                    changes = await _baseDbContext.SaveChangesAsync();
                }
                catch
                {
                    _baseDbSet.AddRange(entities);
                    throw;
                }
                return changes > 0;
            });

        public virtual async Task<bool> DeleteRange(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryAction) =>
            await DbContextQueuedTask.Run(async () =>
            {
                IEnumerable<TEntity> entities = _baseDbSet;
                entities = await QueryRange(queryAction);
                if (entities == null || !entities.Any())
                {
                    throw new ArgumentException("Entities not found");
                }
                _baseDbSet.RemoveRange(entities);

                int changes = 0;
                try
                {
                    changes = await _baseDbContext.SaveChangesAsync();
                }
                catch
                {
                    _baseDbSet.AddRange(entities);
                    throw;
                }
                return changes > 0;
            });

        public virtual async Task<TEntity?> Insert(TEntity entity) =>
            await DbContextQueuedTask.Run(async () =>
            {
                EntityEntry<TEntity> entityEntry = _baseDbSet.Add(entity);
                int changes = 0;
                try
                {
                    changes = await _baseDbContext.SaveChangesAsync();
                }
                catch
                {
                    _baseDbSet.Remove(entity);
                    throw;
                }
                if (changes == 0)
                {
                    throw new DbUpdateException("Failed to insert entity", new ReadOnlyCollection<EntityEntry<TEntity>>(new List<EntityEntry<TEntity>> { entityEntry }));
                }
                return entity;
            });

        public virtual async Task<IEnumerable<TEntity>> InsertRange(IEnumerable<TEntity> entities) =>
            await DbContextQueuedTask.Run(async () =>
            {
                _baseDbSet.AddRange(entities);
                int changes = 0;
                try
                {
                    changes = await _baseDbContext.SaveChangesAsync();
                }
                catch
                {
                    _baseDbSet.RemoveRange(entities);
                    throw;
                }
                return entities;
            });

        public virtual async Task<PagedResult<TEntity>> PagedQuery(int pageNumber, int pageSize, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryAction = null) =>
            await DbContextQueuedTask.Run(async () =>
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 25;
                IQueryable<TEntity> query = _baseDbSet.AsQueryable();
                if (queryAction != null)
                    query = queryAction.Invoke(query);
                int totalCount = query.Count();
                if (totalCount == 0)
                    return PagedResult<TEntity>.Empty;
                IEnumerable<TEntity> items = query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsEnumerable();
                return new PagedResult<TEntity>(pageNumber, pageSize, totalCount, items);
            });

        public async Task<IEnumerable<TEntity>> QueryRange(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryAction = null) =>
            await DbContextQueuedTask.Run(async () =>
            {
                IQueryable<TEntity> query = _baseDbSet.AsQueryable();
                if (queryAction != null)
                    query = queryAction.Invoke(query);
                var sql = query.ToQueryString();
                return query.AsEnumerable();
            });

        public virtual async Task<TEntity?> Query(object key) =>
            await DbContextQueuedTask.Run(async () => await _baseDbSet.FindAsync(key));

        public virtual async Task<IEnumerable<TEntity>> QueryRange(params object[] keys) =>
            await DbContextQueuedTask.Run(async () =>
            {
                if (keys == null || keys.Length == 0)
                    return _baseDbSet.AsEnumerable();
                return keys.Select(key => _baseDbSet.Find(key)!).Where(e => e != null);
            });

        public virtual async Task<TEntity> Update(TEntity entity) =>
            await DbContextQueuedTask.Run(async () =>
            {
                EntityEntry<TEntity> entityEntry = _baseDbSet.Update(entity);
                int changes = await _baseDbContext.SaveChangesAsync();
                if (changes == 0)
                {
                    throw new DbUpdateException("Failed to update entity", new ReadOnlyCollection<EntityEntry<TEntity>>(new List<EntityEntry<TEntity>> { entityEntry }));
                }
                return entity;
            });

        public virtual async Task<IEnumerable<TEntity>> UpdateRange(IEnumerable<TEntity> entities) =>
            await DbContextQueuedTask.Run(async () =>
            {
                _baseDbSet.UpdateRange(entities);
                int changes = await _baseDbContext.SaveChangesAsync();
                return entities;
            });
    }
}