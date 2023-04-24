namespace Infra.EntityFramework.Repository;

public interface IRepositoryFactory
{
    IRepository<T> GetRepository<T>() where T : class;
}