using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Reflection;
using System.Threading.Tasks;
using Infra.Shared.Dtos.Request;
using Infra.Shared.Dtos.Shared;
using Infra.Shared.Enums;
using Dapper;

namespace Infra.Dapper.Extensions
{
    public static class DapperHelper
    {
        public static async Task<PagedList<TResultDto>> GetPaginatedData<TRequestDto, TResultDto>(
            this IDapper dapper,
            TRequestDto request,
            string query,
            CommandType commandType = CommandType.StoredProcedure,
            PaginationType paginationType = PaginationType.Old)
            where TResultDto : class
            where TRequestDto : QueryDtoBase
        {
            var dynamicParams = new DynamicParameters();

            var parameters = new Dictionary<string, object>();
            
            parameters.TryAdd("CountPerPage", request.PageSize);
            parameters.TryAdd("CurrentPageNumber", request.PageNumber);
            parameters.TryAdd("Asc", request.Asc);

            var requestType = request.GetType();
            foreach (var pi in requestType.GetProperties())
            {
                parameters.TryAdd(pi.Name, pi.GetValue(request, null));
            }

            parameters.TryAdd("IsTotalCount", false);

            if (paginationType == PaginationType.Old)
            {
                if (parameters.ContainsKey("PageSize"))
                    parameters.Remove("PageSize");

                if (parameters.ContainsKey("PageNumber"))
                    parameters.Remove("PageNumber");
            }

            if (paginationType == PaginationType.New)
            {
                if (parameters.ContainsKey("CountPerPage"))
                    parameters.Remove("CountPerPage");

                if (parameters.ContainsKey("CurrentPageNumber"))
                    parameters.Remove("CurrentPageNumber");
            }

            var data = await dapper.SelectAsync<TResultDto>(query, new DynamicParameters(parameters), commandType);

            parameters.Remove("IsTotalCount");
            parameters.TryAdd("IsTotalCount", true);

            var totalCount = await dapper.GetAsync<int>(query, new DynamicParameters(parameters), commandType);

            return new PagedList<TResultDto>
            {
                Data = data,
                TotalCount = totalCount
            };
        }

        public static async Task<PagedList<TResultDto>> GetPaginatedData<TRequestDto, TQueryDto, TResultDto>(
            this IDapper dapper,
            TRequestDto request,
            TQueryDto queryDto,
            string query,
            CommandType commandType = CommandType.StoredProcedure,
            PaginationType paginationType = PaginationType.Old)
            where TResultDto : class
            where TRequestDto : QueryDtoBase
        {
            var dynamicParams = new DynamicParameters();

            var parameters = new Dictionary<string, object>();

            parameters.TryAdd("CountPerPage", request.PageSize);
            parameters.TryAdd("CurrentPageNumber", request.PageNumber);
            parameters.TryAdd("Asc", request.Asc);

            var requestType = request.GetType();
            foreach (var pi in requestType.GetProperties())
            {
                parameters.TryAdd(pi.Name, pi.GetValue(request, null));
            }

            var queryDtoType = queryDto.GetType();
            foreach (var pi in queryDtoType.GetProperties())
            {
                parameters.TryAdd(pi.Name, pi.GetValue(queryDto, null));
            }

            parameters.TryAdd("IsTotalCount", false);

            if (paginationType == PaginationType.Old)
            {
                if (parameters.ContainsKey("PageSize"))
                    parameters.Remove("PageSize");

                if (parameters.ContainsKey("PageNumber"))
                    parameters.Remove("PageNumber");
            }

            if (paginationType == PaginationType.New)
            {
                if (parameters.ContainsKey("CountPerPage"))
                    parameters.Remove("CountPerPage");

                if (parameters.ContainsKey("CurrentPageNumber"))
                    parameters.Remove("CurrentPageNumber");
            }

            var data = await dapper.SelectAsync<TResultDto>(query, new DynamicParameters(parameters), commandType);

            parameters.Remove("IsTotalCount");
            parameters.TryAdd("IsTotalCount", true);

            var totalCount = await dapper.GetAsync<int>(query, new DynamicParameters(parameters), commandType);

            return new PagedList<TResultDto>
            {
                Data = data,
                TotalCount = totalCount
            };
        }
    }
}