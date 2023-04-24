using Infra.Shared.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using Infra.Shared.Enums;
using Dapper;

namespace Infra.Dapper
{
    public class Dapper : IDisposable, IDapper
    {
        private readonly IDbConnection _dbDbConnection;
        private readonly int _timeout;

        public Dapper(IDbConnection dbConnection)
        {
            _dbDbConnection = dbConnection;
            
            if (_dbDbConnection.State == ConnectionState.Closed)
                _dbDbConnection.Open();

            _timeout = 30;
        }

        public Task<int> ExecuteAsync(string sp, CommandType commandType = CommandType.StoredProcedure)
        {
            return ExecuteAsync(sp, (object)null, (int)_timeout, commandType);
        }

        public Task<int> ExecuteAsync(string sp, object param, CommandType commandType = CommandType.StoredProcedure)
        {
            return ExecuteAsync(sp, param, (int)_timeout, commandType);
        }

        public Task<int> ExecuteAsync(string sp, object param, int timeout,
            CommandType commandType = CommandType.StoredProcedure)
        {
            return SqlMapper.ExecuteAsync((IDbConnection)this._dbDbConnection, sp, param,
                (IDbTransaction)(IDbTransaction)null, (int?)timeout,
                (CommandType?)commandType);
        }

        public Task<int> ExecuteAsync(string sp, IDbTransaction transaction,
            CommandType commandType = CommandType.StoredProcedure)
        {
            return this.ExecuteAsync(sp, null, _timeout, transaction, commandType);
        }

        public Task<int> ExecuteAsync(string sp, object param, IDbTransaction transaction,
            CommandType commandType = CommandType.StoredProcedure)
        {
            return this.ExecuteAsync(sp, param, _timeout, transaction, commandType);
        }

        public Task<int> ExecuteAsync(string sp, object param, int timeout, IDbTransaction transaction,
            CommandType commandType = CommandType.StoredProcedure)
        {
            return SqlMapper.ExecuteAsync((IDbConnection)this._dbDbConnection, sp, param, transaction, (int?)timeout,
                (CommandType?)commandType);
        }

        public Task<T> GetAsync<T>(string sp, CommandType commandType = CommandType.StoredProcedure)
        {
            return this.GetAsync<T>(sp, null, _timeout, commandType);
        }

        public Task<T> GetAsync<T>(string sp, object param, CommandType commandType = CommandType.StoredProcedure)
        {
            return this.GetAsync<T>(sp, param, _timeout, commandType);
        }

        public Task<T> GetAsync<T>(string sp, object param, int timeout,
            CommandType commandType = CommandType.StoredProcedure)
        {
            return SqlMapper.QueryFirstOrDefaultAsync<T>((IDbConnection)this._dbDbConnection, sp, param,
                (IDbTransaction)(IDbTransaction)null,
                (int?)timeout, (CommandType?)commandType);
        }

        public Task<T> ScalerAsync<T>(string sp, CommandType commandType = CommandType.StoredProcedure)
        {
            return this.ScalerAsync<T>(sp, null, _timeout, commandType);
        }

        public Task<T> ScalerAsync<T>(string sp, object param, CommandType commandType = CommandType.StoredProcedure)
        {
            return this.ScalerAsync<T>(sp, param, _timeout, commandType);
        }

        public Task<T> ScalerAsync<T>(string sp, object param, int timeout,
            CommandType commandType = CommandType.StoredProcedure)
        {
            return SqlMapper.ExecuteScalarAsync<T>((IDbConnection)this._dbDbConnection, sp, param,
                (IDbTransaction)(IDbTransaction)null,
                (int?)timeout, (CommandType?)commandType);
        }

        public Task<T> ScalerAsync<T>(string sp, IDbTransaction transaction,
            CommandType commandType = CommandType.StoredProcedure)
        {
            return this.ScalerAsync<T>(sp, null, _timeout, transaction, commandType);
        }

        public Task<T> ScalerAsync<T>(string sp, object param, IDbTransaction transaction,
            CommandType commandType = CommandType.StoredProcedure)
        {
            return this.ScalerAsync<T>(sp, param, _timeout, transaction, commandType);
        }

        public Task<T> ScalerAsync<T>(string sp, object param, int timeout, IDbTransaction transaction,
            CommandType commandType = CommandType.StoredProcedure)
        {
            return SqlMapper.ExecuteScalarAsync<T>((IDbConnection)this._dbDbConnection, sp, param, transaction,
                (int?)timeout,
                (CommandType?)commandType);
        }

        public Task<IEnumerable<T>> SelectAsync<T>(string sp, CommandType commandType = CommandType.StoredProcedure)
        {
            return this.SelectAsync<T>(sp, null, _timeout, commandType);
        }

        public Task<IEnumerable<T>> SelectAsync<T>(string sp, object param,
            CommandType commandType = CommandType.StoredProcedure)
        {
            return this.SelectAsync<T>(sp, param, _timeout, commandType);
        }

        public Task<IEnumerable<T>> SelectAsync<T>(string sp, object param, int timeout,
            CommandType commandType = CommandType.StoredProcedure)
        {
            return SqlMapper.QueryAsync<T>((IDbConnection)this._dbDbConnection, sp, param,
                (IDbTransaction)(IDbTransaction)null, (int?)timeout,
                (CommandType?)commandType);
        }

        public IDbTransaction NewTransaction()
        {
            if (this._dbDbConnection.State == ConnectionState.Closed)
            {
                this._dbDbConnection.Open();
            }

            return this._dbDbConnection.BeginTransaction();
        }

        public Task<int> InsertAsync(string sql, object param, IDbTransaction transaction)
        {
            return SqlMapper.ExecuteAsync(this._dbDbConnection, sql, param, transaction);
        }

        public Task<T> ScalerByCommandAsync<T>(string sql, object param, IDbTransaction transaction)
        {
            return SqlMapper.ExecuteScalarAsync<T>((IDbConnection)this._dbDbConnection, sql, param, transaction,
                (int?)_timeout,
                (CommandType?)CommandType.Text);
        }

        public int Execute(string sp, CommandType commandType = CommandType.StoredProcedure)
        {
            return this.Execute(sp, null, _timeout, commandType);
        }

        public int Execute(string sp, object param, CommandType commandType = CommandType.StoredProcedure)
        {
            return this.Execute(sp, param, _timeout, commandType);
        }

        public int Execute(string sp, object param, int timeout, CommandType commandType = CommandType.StoredProcedure)
        {
            return this._dbDbConnection.Execute(sp, param, null, timeout, commandType);
        }

        public int Execute(string sp, IDbTransaction transaction, CommandType commandType = CommandType.StoredProcedure)
        {
            return this.Execute(sp, null, _timeout, transaction, commandType);
        }

        public int Execute(string sp, object param, IDbTransaction transaction,
            CommandType commandType = CommandType.StoredProcedure)
        {
            return this.Execute(sp, param, _timeout, transaction, commandType);
        }

        public int Execute(string sp, object param, int timeout, IDbTransaction transaction,
            CommandType commandType = CommandType.StoredProcedure)
        {
            return this._dbDbConnection.Execute(sp, param, transaction, timeout, commandType);
        }

        public T Get<T>(string sp, CommandType commandType = CommandType.StoredProcedure)
        {
            return this.Get<T>(sp, null, _timeout, commandType);
        }

        public T Get<T>(string sp, object param, CommandType commandType = CommandType.StoredProcedure)
        {
            return this.Get<T>(sp, param, _timeout);
        }

        public T Get<T>(string sp, object param, int timeout, CommandType commandType = CommandType.StoredProcedure)
        {
            return this._dbDbConnection.QueryFirstOrDefault<T>(sp, param, null, timeout, commandType);
        }

        public DynamicParameters Merge(params object[] parameters)
        {
            DynamicParameters dictionary = new DynamicParameters();

            foreach (object parameter in parameters)
            {
                foreach (var property in parameter.GetType().GetProperties())
                {
                    dictionary.Add(property.Name, property.GetValue(parameter));
                }
            }

            return dictionary;
        }

        public IDbTransaction NewTran()
        {
            return this._dbDbConnection.BeginTransaction();
        }

        public T Scaler<T>(string sp)
        {
            return this.Scaler<T>(sp, null, _timeout);
        }

        public T Scaler<T>(string sp, object param)
        {
            return this.Scaler<T>(sp, param, _timeout);
        }

        public T Scaler<T>(string sp, object param, int timeout)
        {
            return this._dbDbConnection.ExecuteScalar<T>(sp, param, null, timeout, CommandType.StoredProcedure);
        }

        public T Scaler<T>(string sp, IDbTransaction transaction)
        {
            return this.Scaler<T>(sp, null, _timeout, transaction);
        }

        public T Scaler<T>(string sp, object param, IDbTransaction transaction)
        {
            return this.Scaler<T>(sp, param, _timeout, transaction);
        }

        public T Scaler<T>(string sp, object param, int timeout, IDbTransaction transaction)
        {
            return this._dbDbConnection.ExecuteScalar<T>(sp, param, transaction, timeout, CommandType.StoredProcedure);
        }

        public IEnumerable<T> Select<T>(string sp, CommandType commandType = CommandType.StoredProcedure)
        {
            return this.Select<T>(sp, null, _timeout, commandType);
        }

        public IEnumerable<T> Select<T>(string sp, object param, CommandType commandType = CommandType.StoredProcedure)
        {
            return this.Select<T>(sp, param, _timeout, commandType);
        }

        public IEnumerable<T> Select<T>(string sp, object param, int timeout,
            CommandType commandType = CommandType.StoredProcedure)
        {
            return this._dbDbConnection.Query<T>(sp, param, null, true, timeout, commandType);
        }

        public Task<SqlMapper.GridReader> QueryMultipleAsync(string sp,
            CommandType commandType = CommandType.StoredProcedure)
        {
            return this.QueryMultipleAsync(sp, null, commandType);
        }

        public Task<SqlMapper.GridReader> QueryMultipleAsync(string sp, object param,
            CommandType commandType = CommandType.StoredProcedure)
        {
            return this.QueryMultipleAsync(sp, param, _timeout, commandType);
        }

        public Task<SqlMapper.GridReader> QueryMultipleAsync(string sp, object param, int timeout,
            CommandType commandType = CommandType.StoredProcedure)
        {
            return this._dbDbConnection.QueryMultipleAsync(sp, param, null, timeout, commandType);
        }


        public SqlMapper.ICustomQueryParameter GetTvp<T>(IEnumerable<T> models, string type)
        {
            var table = new DataTable();

            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                table.Columns.Add
                (
                    property.Name,
                    property.PropertyType.IsGenericType
                        ? property.PropertyType.GenericTypeArguments[0]
                        : property.PropertyType
                );
            }

            foreach (var model in models)
            {
                var row = table.NewRow();

                foreach (PropertyInfo property in properties)
                {
                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table.AsTableValuedParameter(type);
        }

        public Task<int> Log(string message, LogType logType, string business = null)
        {
            return this.ExecuteAsync("sp_InsertLog", new { title = message, Id_LogType = logType, business }, 8);
        }

        public Task<int> LogMobile(string type, string baseUrl, string url, string body = null, string header = null,
            int id_User = 0)
        {
            return this.ExecuteAsync("sp_InsertLogMobile", new { type, baseUrl, url, body, header, id_User }, 8);
        }

        private string GetKey(string sp, object param)
        {
            string key = sp;

            if (param != null)
            {
                PropertyInfo[] props = param.GetType().GetProperties();

                foreach (PropertyInfo prop in props)
                {
                    key += $"{prop.Name}{Convert.ToString(prop.GetValue(param))}";
                }
            }

            return key;
        }

        public void Dispose()
        {
            if (_dbDbConnection != null)
            {
                if (this._dbDbConnection.State == ConnectionState.Open)
                {
                    this._dbDbConnection.Close();
                }

                this._dbDbConnection.Dispose();
            }
        }
    }
}