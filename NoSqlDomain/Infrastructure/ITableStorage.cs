using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NoSqlDomain
{
    public interface ITableStorage<TEntity>
    {
        Task<TEntity> Post(TEntity entity);

        IQueryable<TEntity> GetAll();

        Task<TEntity> GetByRowAndPartitionKeys(string rowKey, string partitionKey);

        Task<TEntity> Put(TEntity entity);

        Task Delete(string rowKey, string partitionKey);

        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> query);
    }
}
