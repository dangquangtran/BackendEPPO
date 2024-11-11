using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            int? pageIndex = null,
            int? pageSize = null);

        //Do Huu Thuan
        Task<IEnumerable<TEntity>> GetAsync(  
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            int? pageIndex = null,
            int? pageSize = null);


        TEntity GetByID(object id, string includeProperties = "");

        void Insert(TEntity entity);

        //void Delete(object id);

        //void Delete(TEntity entityToDelete);

        void Update(TEntity entityToUpdate);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null);

        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);
        Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter);

    }
}
