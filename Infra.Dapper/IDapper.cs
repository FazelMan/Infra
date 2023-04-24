using Infra.Shared.Ioc;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Infra.Shared.Enums;

namespace Infra.Dapper
{
    public interface IDapper : ITransientDependency
    {
        Task<int> ExecuteAsync(string sp, CommandType commandType = CommandType.StoredProcedure);
        Task<int> ExecuteAsync(string sp, object param, CommandType commandType = CommandType.StoredProcedure);
        Task<int> ExecuteAsync(string sp, object param, int timeout, CommandType commandType = CommandType.StoredProcedure);
        Task<int> ExecuteAsync(string sp, IDbTransaction transaction, CommandType commandType = CommandType.StoredProcedure);
        Task<int> ExecuteAsync(string sp, object param, IDbTransaction transaction, CommandType commandType = CommandType.StoredProcedure);
        Task<int> ExecuteAsync(string sp, object param, int timeout, IDbTransaction transaction, CommandType commandType = CommandType.StoredProcedure);
        Task<T> GetAsync<T>(string sp, CommandType commandType = CommandType.StoredProcedure);
        Task<T> GetAsync<T>(string sp, object param, CommandType commandType = CommandType.StoredProcedure);
        Task<T> GetAsync<T>(string sp, object param, int timeout, CommandType commandType = CommandType.StoredProcedure);
        Task<T> ScalerAsync<T>(string sp, CommandType commandType = CommandType.StoredProcedure);
        Task<T> ScalerAsync<T>(string sp, object param, CommandType commandType = CommandType.StoredProcedure);
        Task<T> ScalerAsync<T>(string sp, object param, int timeout, CommandType commandType = CommandType.StoredProcedure);
        Task<T> ScalerAsync<T>(string sp, IDbTransaction transaction, CommandType commandType = CommandType.StoredProcedure);
        Task<T> ScalerAsync<T>(string sp, object param, IDbTransaction transaction, CommandType commandType = CommandType.StoredProcedure);
        Task<T> ScalerAsync<T>(string sp, object param, int timeout, IDbTransaction transaction, CommandType commandType = CommandType.StoredProcedure);
        Task<IEnumerable<T>> SelectAsync<T>(string sp, CommandType commandType = CommandType.StoredProcedure);
        Task<IEnumerable<T>> SelectAsync<T>(string sp, object param, CommandType commandType = CommandType.StoredProcedure);
        Task<IEnumerable<T>> SelectAsync<T>(string sp, object param, int timeout, CommandType commandType = CommandType.StoredProcedure);
        IDbTransaction NewTransaction();
        Task<int> InsertAsync(string sql, object param, IDbTransaction transaction);
        Task<T> ScalerByCommandAsync<T>(string sql, object param, IDbTransaction transaction);
        int Execute(string sp, CommandType commandType = CommandType.StoredProcedure);
        int Execute(string sp, object param, CommandType commandType = CommandType.StoredProcedure);
        int Execute(string sp, object param, int timeout, CommandType commandType = CommandType.StoredProcedure);
        int Execute(string sp, IDbTransaction transaction, CommandType commandType = CommandType.StoredProcedure);
        int Execute(string sp, object param, IDbTransaction transaction, CommandType commandType = CommandType.StoredProcedure);
        int Execute(string sp, object param, int timeout, IDbTransaction transaction, CommandType commandType = CommandType.StoredProcedure);
        T Get<T>(string sp, CommandType commandType = CommandType.StoredProcedure);
        T Get<T>(string sp, object param, CommandType commandType = CommandType.StoredProcedure);
        T Get<T>(string sp, object param, int timeout, CommandType commandType = CommandType.StoredProcedure);
        DynamicParameters Merge(params object[] parameters);
        IDbTransaction NewTran();
        T Scaler<T>(string sp);
        T Scaler<T>(string sp, object param);
        T Scaler<T>(string sp, object param, int timeout);
        T Scaler<T>(string sp, IDbTransaction transaction);
        T Scaler<T>(string sp, object param, IDbTransaction transaction);
        T Scaler<T>(string sp, object param, int timeout, IDbTransaction transaction);
        IEnumerable<T> Select<T>(string sp, CommandType commandType = CommandType.StoredProcedure);
        IEnumerable<T> Select<T>(string sp, object param, CommandType commandType = CommandType.StoredProcedure);
        IEnumerable<T> Select<T>(string sp, object param, int timeout, CommandType commandType = CommandType.StoredProcedure);
        Task<SqlMapper.GridReader> QueryMultipleAsync(string sp,
            CommandType commandType = CommandType.StoredProcedure);
        Task<SqlMapper.GridReader> QueryMultipleAsync(string sp, object param,
            CommandType commandType = CommandType.StoredProcedure);
        Task<SqlMapper.GridReader> QueryMultipleAsync(string sp, object param, int timeout,
            CommandType commandType = CommandType.StoredProcedure);
        SqlMapper.ICustomQueryParameter GetTvp<T>(IEnumerable<T> models, string type);
        Task<int> Log(string message, LogType logType, string business = null);
        Task<int> LogMobile(string type, string baseUrl, string url, string body = null, string header = null, int id_User = 0);
    }
}