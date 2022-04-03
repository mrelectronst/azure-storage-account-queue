using Microsoft.Azure.Cosmos.Table;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NoSqlDomain
{
    public class TableStorageRepository<TEntity> : ITableStorage<TEntity> where TEntity : TableEntity, new()
    {
        private readonly CloudTableClient _cloudTableClient;
        private readonly CloudTable _cloudTable;

        public TableStorageRepository()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(ConnectionStrings.AzStorageConnectionString); //Fill ConnectionString in appsetting.json
            this._cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            this._cloudTable = _cloudTableClient.GetTableReference(typeof(TEntity).Name);

            _cloudTable.CreateIfNotExists();
        }

        public async Task Delete(string rowKey, string partitionKey)
        {
            var entity = await GetByRowAndPartitionKeys(rowKey, partitionKey);

            var DeleteOp = TableOperation.Delete(entity);

            await _cloudTable.ExecuteAsync(DeleteOp);
        }

        public IQueryable<TEntity> GetAll()
        {
            return _cloudTable.CreateQuery<TEntity>().AsQueryable();
        }

        public async Task<TEntity> GetByRowAndPartitionKeys(string rowKey, string partitionKey)
        {
            var GetOp = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);

            var execute = await _cloudTable.ExecuteAsync(GetOp);

            return execute.Result as TEntity;
        }

        public async Task<TEntity> Post(TEntity entity)
        {
            var InsertOp = TableOperation.InsertOrMerge(entity);

            var execute = await _cloudTable.ExecuteAsync(InsertOp);

            return execute.Result as TEntity;
        }

        public async Task<TEntity> Put(TEntity entity)
        {
            var PutOp = TableOperation.Replace(entity);

            var execute = await _cloudTable.ExecuteAsync(PutOp);

            return execute.Result as TEntity;
        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> query)
        {
            return _cloudTable.CreateQuery<TEntity>().Where(query);
        }
    }
}
