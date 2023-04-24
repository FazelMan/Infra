using System.Collections.Generic;
using System.Threading.Tasks;
using Infra.Shared.Dtos.Shared.Elastic;
using Infra.ElasticSearch.Dtos;
using Nest;

namespace Infra.ElasticSearch.Repository.Interfaces
{
    public interface IElasticRepository<TDocument> where TDocument : class, ISuggest //: ISingletonDependency
    {
        ElasticClient Client { get; }
        string IndexName { get; }

        Task<bool> IndexDataAsync(TDocument data, string indexName = null);
        Task<bool> DeleteAsync(string id, string indexName = null);
        Task<BulkResponse> BulkDeleteAsync(IEnumerable<TDocument> documents, string indexName = null);
        Task<BulkResponse> BulkInsertAsync(IEnumerable<TDocument> documents, string indexName = null);
        Task<BulkResponse> BulkUpdateAsync(IEnumerable<TDocument> documents, string indexName = null);

        Task<CreateIndexResponse> CreateIndex(string indexName = null);
        
        Task<List<TDocument>> GetByIdAsync(string id);
        Task<List<TDocument>> GetByFieldFiltersAsync(IList<FieldFilter> filters, string indexName = null);
        Task<List<TDocument>> GetAllByCombinationFiltersAsync(CombinedFilter filter, string indexName = null);
        Task<List<TDocument>> SearchAllAsync(string queryTerm, string indexName = null);
        Task<ISearchResponse<TDocument>> FuzzySearchAsync(string queryTerm, string indexName = null, int pageNumber = 0, int pageSize = 10);
        Task<long> GetCountAsync(string filter = "*");

        Task<Dictionary<string, string>> GetAllByAggregationsAsync(Aggregations aggregations,
            List<ElasticFilter> filters, string indexName = null);
        Task<List<TDocument>> GetAllByRangesQueryUsingAndAsync(IList<RangeFilter> filters, string indexName = null);
        Task<List<TDocument>> GetAllByFiltersAsync(List<ElasticFilter> filters, string indexName = null);
        Task<List<TDocument>> GetAllAsync(QueryContainerDescriptor<TDocument> query, string indexName = null);
        Task<List<TDocument>> GetAllAsync(SearchDescriptor<TDocument> search, string indexName = null);
    }
}