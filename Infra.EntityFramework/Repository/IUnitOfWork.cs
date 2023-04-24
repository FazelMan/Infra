using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infra.EntityFramework.Repository;

public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    TContext Context { get; }
}

public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

    int Commit();
    Task<int> CommitAsync();
}