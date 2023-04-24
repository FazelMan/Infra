using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Infra.EntityFramework.Dtos.Entities.Auditing;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infra.EntityFramework.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public Repository(DbContext context)
        {
            _dbContext = context ?? throw new ArgumentException(nameof(context));
            _dbSet = _dbContext.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool enableTracking = true)
        {
            IQueryable<T> query = _dbSet;
            if (!enableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return orderBy(query).AnyAsync();

            return query.AnyAsync();
        }

        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool enableTracking = true,
            bool ignoreQueryFilters = false)
        {
            IQueryable<T> query = _dbSet;

            if (!enableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (ignoreQueryFilters) query = query.IgnoreQueryFilters();

            if (orderBy != null) return orderBy(query).FirstOrDefaultAsync();

            return query.FirstOrDefaultAsync();
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool enableTracking = true,
            bool ignoreQueryFilters = false)
        {
            IQueryable<T> query = _dbSet;

            if (!enableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (ignoreQueryFilters) query = query.IgnoreQueryFilters();

            if (orderBy != null) return orderBy(query).FirstOrDefault();

            return query.FirstOrDefault();
        }

        public async Task<T> InsertAsync(T entity, CancellationToken cancellationToken = default)
        {
            var model = await _dbSet.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return model.Entity;
        }

        public Task InsertAsync(params T[] entities)
        {
            _dbSet.AddRangeAsync(entities);
            return _dbContext.SaveChangesAsync();
        }


        public Task InsertAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            _dbSet.AddRangeAsync(entities, cancellationToken);
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task UpdateAsync(params T[] entities)
        {
            _dbSet.UpdateRange(entities);
            return _dbContext.SaveChangesAsync();
        }

        public Task UpdateAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            _dbSet.UpdateRange(entities);
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(entity);
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            var model = _dbSet.Where(predicate);
            _dbSet.RemoveRange(model);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task DeleteAsync(params T[] entities)
        {
            _dbSet.RemoveRange(entities);
            return _dbContext.SaveChangesAsync();
        }

        public Task DeleteAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            _dbSet.RemoveRange(entities);
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task SoftDeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = _dbSet.Where(predicate).ToList();
            foreach (var entity in entities)
            {
                _softDelete(entity);
            }

            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task SoftDeleteAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                _softDelete(entity);
            }

            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public T Insert(T entity)
        {
            var result = _dbSet.Add(entity);
            return result.Entity;
        }

        public void Insert(params T[] entities)
        {
            _dbSet.AddRange(entities);
        }

        public void Insert(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Update(params T[] entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public void Update(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Delete(Expression<Func<T, bool>> predicate)
        {
            var model = _dbSet.FirstOrDefault(predicate);
            if (model != null) _dbSet.Remove(model);
        }

        public void Delete(params T[] entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public void Delete(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public void SoftDelete(Expression<Func<T, bool>> predicate)
        {
            var entities = _dbSet.Where(predicate).ToList();
            foreach (var entity in entities)
            {
                _softDelete(entity);
            }
        }

        public void SoftDelete(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _softDelete(entity);
            }
        }

        private void _softDelete(T entity)
        {
            var propertyInfo = entity.GetType().GetProperty("IsRemoved");

            if (propertyInfo is null)
                throw new Exception("Soft Delete is not supported for entity " + typeof(T).FullName);

            propertyInfo.SetValue(entity, Convert.ChangeType(true, propertyInfo.PropertyType), null);

            _dbSet.Update(entity);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}