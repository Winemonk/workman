using Workman.Core.Entities;

namespace Workman.Core.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<bool> Delete(object key);

        Task<bool> DeleteRange(params object[] keys);

        Task<TEntity?> Insert(TEntity entity);

        Task<IEnumerable<TEntity>> InsertRange(IEnumerable<TEntity> entities);

        Task<TEntity?> Query(object key);

        Task<IEnumerable<TEntity>> QueryRange(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryAction = null);

        Task<IEnumerable<TEntity>> QueryRange(params object[] keys);

        Task<PagedResult<TEntity>> PagedQuery(int pageNumber, int pageSize, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryAction = null);

        Task<TEntity> Update(TEntity entity);

        Task<IEnumerable<TEntity>> UpdateRange(IEnumerable<TEntity> entities);
        Task<bool> DeleteRange(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryAction);
    }
}