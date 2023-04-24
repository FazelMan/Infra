using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;

namespace Infra.EntityFramework.Repository;

public interface IRepository<T> : IDisposable where T : class
{
    IQueryable<T> GetAll();
    IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);
  
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
        bool enableTracking = true);
    
    Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
        bool enableTracking = true,
        bool ignoreQueryFilters = false);

    T FirstOrDefault(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
        bool enableTracking = true,
        bool ignoreQueryFilters = false);

    Task<T> InsertAsync(T entity, CancellationToken cancellationToken = default);
    Task InsertAsync(params T[] entities);
    Task InsertAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(params T[] entities);
    Task UpdateAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task DeleteAsync(params T[] entities);
    Task DeleteAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task SoftDeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    T Insert(T entity);
    void Insert(params T[] entities);
    void Insert(IEnumerable<T> entities);

    void Update(T entity);
    void Update(params T[] entities);
    void Update(IEnumerable<T> entities);

    void Delete(T entity);
    void Delete(Expression<Func<T, bool>> predicate);
    void Delete(params T[] entities);
    void Delete(IEnumerable<T> entities);

    void SoftDelete(Expression<Func<T, bool>> predicate);
    void SoftDelete(IEnumerable<T> entities);
}