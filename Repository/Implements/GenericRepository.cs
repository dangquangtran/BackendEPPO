﻿using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repository.Implements
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        internal bef4qvhxkgrn0oa7ipg0Context context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(bef4qvhxkgrn0oa7ipg0Context context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }
        public async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await dbSet.FirstOrDefaultAsync(filter);
        }
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            if (filter != null)
            {
                return await dbSet.CountAsync(filter);
            }
            return await dbSet.CountAsync();
        }
        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await dbSet.AnyAsync(filter);
        }

        // Updated Get method with pagination (synchronous)
        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            int? pageIndex = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Implementing pagination
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;

                query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            return query.ToList();
        }

        // Asynchronous Get method with pagination (async)
        public virtual async Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            int? pageIndex = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Implementing pagination
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;

                query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            return await query.ToListAsync();
        }

        public virtual TEntity GetByID(object id, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            // Bao gồm các thuộc tính liên quan
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            // Tìm kiếm theo id
            var keyPropertyName = context.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties[0].Name;

            return query.FirstOrDefault(e => EF.Property<object>(e, keyPropertyName).Equals(id));
        }
        public virtual async Task<TEntity> GetByIDAsync(object id, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            // Include related properties if any
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            // Retrieve the key name from the model
            var keyPropertyName = context.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties[0].Name;

            // Perform the query asynchronously and return the result
            return await query.FirstOrDefaultAsync(e => EF.Property<object>(e, keyPropertyName).Equals(id));
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        //public virtual void Delete(object id)
        //{
        //    TEntity entityToDelete = dbSet.Find(id);
        //    Delete(entityToDelete);
        //}

        //public virtual void Delete(TEntity entityToDelete)
        //{
        //    if (context.Entry(entityToDelete).State == EntityState.Detached)
        //    {
        //        dbSet.Attach(entityToDelete);
        //    }
        //    dbSet.Remove(entityToDelete);
        //}
        public async Task<IEnumerable<TEntity>> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate, string includeProperties = "")
        {
            var query = context.Set<TEntity>().Where(predicate);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.ToListAsync();
        }
        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual IQueryable<TEntity> GetQueryable(
      Expression<Func<TEntity, bool>> filter = null,
      Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
      string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return query;
        }

        }


}
