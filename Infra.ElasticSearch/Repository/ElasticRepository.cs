using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infra.Shared;
using Infra.Shared.Dtos.Shared.Elastic;
using Infra.Shared.Helpers;
using Infra.ElasticSearch.Dtos;
using Infra.ElasticSearch.Enums;
using Infra.ElasticSearch.Repository.Interfaces;
using Nest;

namespace Infra.ElasticSearch.Repository
{
    public class ElasticRepository<TDocument> : IElasticRepository<TDocument>
        where TDocument : class, ISuggest, IElasticDocument
    {
        private ElasticClient _client = null;
        private ConnectionSettings _settings;
        private static readonly object LockObject = new object();
        private static Field IdField = new Field("id");
        private static Field TimestampField = new Field("createdDate");

        public ElasticClient Client
        {
            get
            {
                lock (LockObject)
                {
                    if (_client == null)
                    {
                        var node = new Uri( Host.Config["ElasticSearchSecret:Uri"]);

                        _settings = new ConnectionSettings(node)
                            .DefaultIndex(IndexName.ToLower())
                            .BasicAuthentication(
                                Host.Config["ElasticSearchSecret:Username"],
                                Host.Config["ElasticSearchSecret:Password"])
                            .RequestTimeout(TimeSpan.FromMinutes(5))
                            .PingTimeout(TimeSpan.FromMinutes(5))
                            .DisableDirectStreaming();

                        _client = new ElasticClient(_settings);
                    }

                    return _client;
                }
            }
        }

        public string IndexName => SharedConstants.Elastic.DEFAULT_INDEX;

        public async Task<bool> IndexDataAsync(TDocument document, string indexName = null)
        {
            if (this.Client == null)
            {
                throw new ArgumentNullException("Elastic Client");
            }

            var response = await this.Client.IndexAsync(document,
                c => c
                    .Index(indexName.ToLower() ?? IndexName.ToLower())
            );

            if (response.IsValid == false && response.ServerError != null)
            {
                throw new Exception(response.ServerError.Error.Reason);
            }

            return response.IsValid;
        }

        public async Task<bool> DeleteAsync(string id, string indexName = null)
        {
            if (this.Client == null)
            {
                throw new ArgumentNullException("Elastic Client");
            }

            var response = await this.Client.DeleteAsync(
                new DeleteRequest(
                    indexName.ToLower() ?? IndexName.ToLower()
                    , id.Trim())
            );

            if (response.IsValid == false && response.ServerError != null)
            {
                throw new Exception(response.ServerError.Error.Reason);
            }

            return response.IsValid;
        }

        public async Task<BulkResponse> BulkDeleteAsync(IEnumerable<TDocument> documents, string indexName = null)
        {
            return await this.Client.DeleteManyAsync<TDocument>(documents, IndexName.ToLower());
        }

        public async Task<CreateIndexResponse> CreateIndex(string indexName = null)
        {
            if ((await Client.Indices.ExistsAsync(indexName.ToLower())).Exists)
                await Client.Indices.DeleteAsync(indexName.ToLower());

            return await Client.Indices.CreateAsync(indexName.ToLower(), c => c
                .Map<TDocument>(m => m.AutoMap()));
        }

        public async Task<BulkResponse> BulkInsertAsync(IEnumerable<TDocument> documents, string indexName = null)
        {
            var request = new BulkDescriptor();

            foreach (var document in documents)
            {
                request
                    .Index<TDocument>(op => op
                        .Index(indexName.ToLower() ?? IndexName.ToLower())
                        .Document(document));
            }

            return await Client.BulkAsync(request);
        }

        public async Task<BulkResponse> BulkUpdateAsync(IEnumerable<TDocument> documents, string indexName = null)
        {
            var descriptor = new BulkDescriptor();

            foreach (var document in documents)
            {
                descriptor.Update<TDocument, object>(u => u
                    .Index(indexName.ToLower() ?? IndexName.ToLower())
                    .Id(document.Id)
                    .Doc(document));
            }

            return await _client.BulkAsync(descriptor);
        }

        public async Task<List<TDocument>> GetByIdAsync(string id)
        {
            QueryContainer queryById = new TermQuery() { Field = IdField, Value = id.Trim() };

            var response = await this.Client
                .SearchAsync<TDocument>(s => s
                    .Query(q => q
                        .MatchAll() && queryById));

            List<TDocument> typedList =
                Enumerable.ToList<TDocument>(response.Hits.Select(hit => ConvertHitToDocument(hit)));

            return typedList;
        }

        public async Task<List<TDocument>> GetByFieldFiltersAsync(IList<FieldFilter> filters, string indexName = null)
        {
            QueryContainer filters_complete = null;

            foreach (var item in filters)
            {
                filters_complete = CreateSimpleQuery(item, filters_complete) as QueryContainer;
            }

            var response = await this.Client.SearchAsync<TDocument>(s => s
                .Index(indexName.ToLower() ?? IndexName.ToLower())
                .Query(_ => filters_complete).Scroll("60s")
                .From(0)
                .Size(int.MaxValue));

            List<TDocument> typedList =
                Enumerable.ToList<TDocument>(response.Hits.Select(hit => ConvertHitToDocument(hit)));

            return typedList;
        }

        public async Task<List<TDocument>> GetAllByCombinationFiltersAsync(CombinedFilter filter,
            string indexName = null)
        {
            ElasticQueryContainer elasticQueryContainer = GetSearchQuery(filter);

            //Run the combined query:
            var response = await this.Client.SearchAsync<TDocument>(s => s
                .Index(indexName.ToLower() ?? IndexName.ToLower())
                .Query(q => q
                    .Bool(bq => bq
                        .Should(elasticQueryContainer.orQuery.ToArray())
                        .Must(elasticQueryContainer.andQuery.ToArray())
                        .MustNot(elasticQueryContainer.notQuery.ToArray())
                    )));

            //Translate the hits and return the list
            List<TDocument> typedList =
                Enumerable.ToList<TDocument>(response.Hits.Select(hit => ConvertHitToDocument(hit)));

            return typedList;
        }

        public async Task<List<TDocument>> GetAllByFiltersAsync(List<ElasticFilter> filters, string indexName = null)
        {
            if (this.Client == null)
            {
                throw new Exception();
            }

            ElasticQueryContainer elasticQueryContainer = GetSearchQueryByFilters(filters);

            //Run the combined query:
            var response = await this.Client.SearchAsync<TDocument>(s => s
                .Index(indexName.ToLower() ?? IndexName.ToLower())
                .Query(q => q
                    .Bool(bq => bq
                        .Should(elasticQueryContainer.orQuery.ToArray())
                        .Must(elasticQueryContainer.andQuery.ToArray())
                        .MustNot(elasticQueryContainer.notQuery.ToArray())
                    ))
                .From(0).Size(int.MaxValue));

            //Translate the hits and return the list
            List<TDocument> typedList =
                Enumerable.ToList<TDocument>(response.Hits.Select(hit => ConvertHitToDocument(hit)));

            return typedList;
        }

        public QueryContainer GetQueryContainerByFilterAsync(ElasticFilter filter)
        {
            QueryContainer filters_complete = null;

            QueryContainer partial = null;

            switch (filter.RangeFilterType)
            {
                case RangeFilterType.Numeric:
                    partial &= ExecuteNumericRangesQuery(filter) as QueryContainer;
                    break;
                case RangeFilterType.Long:
                    partial &= ExecuteLongRangesQuery(filter) as QueryContainer;
                    break;
                case RangeFilterType.Term:
                    partial &= ExecuteTermRangesQuery(filter) as QueryContainer;
                    break;
                case RangeFilterType.Date:
                    partial &= ExecuteDateRangesQuery(filter) as QueryContainer;
                    break;
                default:
                    break;
            }

            if (filter.IsNested.HasValue && filter.IsNested.Value)
            {
                filters_complete &= new QueryContainerDescriptor<TDocument>()
                    .Nested(n => n.Path(filter.Parent).Query(q => q.Bool(bq => bq.Filter(partial))));
            }
            else if (partial != null)
            {
                filters_complete &= partial;
            }

            return filters_complete;
        }

        public async Task<List<TDocument>> GetAllByRangesQueryUsingAndAsync(IList<RangeFilter> filters,
            string indexName = null)
        {
            QueryContainer filters_complete = null;

            foreach (var filter in filters)
            {
                QueryContainer partial = null;

                switch (filter.RangeFilterType)
                {
                    case RangeFilterType.Numeric:
                        filters_complete &= ExecuteNumericRangesQuery(filter) as QueryContainer;
                        break;
                    case RangeFilterType.Long:
                        filters_complete &= ExecuteLongRangesQuery(filter) as QueryContainer;
                        break;
                    case RangeFilterType.Term:
                        filters_complete &= ExecuteTermRangesQuery(filter) as QueryContainer;
                        break;
                    case RangeFilterType.Date:
                        filters_complete &= ExecuteDateRangesQuery(filter) as QueryContainer;
                        break;
                    default:
                        break;
                }

                if (filter.IsNested)
                {
                    filters_complete &= new QueryContainerDescriptor<TDocument>().Nested(n =>
                        n.Path(filter.Parent).Query(q2 => q2.Bool(bq => bq.Filter(partial))));
                }
                else if (partial != null)
                {
                    filters_complete &= partial;
                }
            }

            var response =
                await this.Client.SearchAsync<TDocument>(s => s
                    .Index(indexName.ToLower() ?? IndexName.ToLower())
                    .Query(_ => filters_complete).Scroll("60s")
                    .From(0)
                    .Size(int.MaxValue));

            //Translate the hits and return the list
            return Enumerable.ToList<TDocument>(response.Hits.Select(hit => ConvertHitToDocument(hit)));
        }

        public async Task<Dictionary<string, string>> GetAllByAggregationsAsync(
            Aggregations aggregations, List<ElasticFilter> filters, string indexName = null)
        {
            var result = new Dictionary<string, string>();

            ElasticQueryContainer elasticQueryContainer = GetSearchQueryByFilters(filters);

            switch (aggregations.AggregationType)
            {
                case AggregationType.Count:
                {
                    await ExecuteCountAggregation(aggregations, result, elasticQueryContainer, indexName.ToLower());
                    break;
                }

                case AggregationType.Avg:
                {
                    await ExecuteAvgAggregation(aggregations, result, elasticQueryContainer, indexName.ToLower());
                    break;
                }

                case AggregationType.Sum:
                {
                    await ExecuteSumAggregation(aggregations, result, elasticQueryContainer, indexName.ToLower());
                    break;
                }

                case AggregationType.Min:
                {
                    await ExecuteMinAggregation(aggregations, result, elasticQueryContainer, indexName.ToLower());
                    break;
                }

                case AggregationType.Max:
                {
                    await ExecuteMaxAggregation(aggregations, result, elasticQueryContainer, indexName.ToLower());
                    break;
                }

                default:
                    break;
            }

            return result;
        }

        public async Task<List<TDocument>> SearchAllAsync(string queryTerm, string indexName = null)
        {
            var queryResult = await this.Client.SearchAsync<TDocument>(d =>
                d.AllIndices()
                    .Index(indexName.ToLower() ?? IndexName.ToLower())
                    .AllIndices()
                    .Query(q => q.QueryString(e =>
                        e.Query(queryTerm))));

            return Enumerable.Distinct<TDocument>(queryResult
                    .Hits
                    .Select(c => c.Source))
                .ToList();
        }

        public async Task<ISearchResponse<TDocument>> FuzzySearchAsync(string queryTerm, string indexName = null,
            int pageNumber = 0, int pageSize = 10)
        {
            var response = await this.Client.SearchAsync<TDocument>(s => s
                .Index(indexName.ToLower() ?? IndexName.ToLower())
                .From(pageNumber)
                .Size(pageSize)
                .Query(q => q
                    .Match(m => m.Field(c => c.IndexedText)
                        .Query(queryTerm)
                        .Boost(3.1)
                        .Fuzziness(Fuzziness.Auto)
                    ))
            );

            return response;
        }

        public async Task<long> GetCountAsync(string filter = "*")
        {
            var searchRequest = new SearchRequest<TDocument>(Indices.Parse(IndexName.ToLower()))
            {
                Size = 0,
                Query = new QueryContainer(
                    new SimpleQueryStringQuery
                    {
                        Query = filter
                    }
                ),
                Sort = new List<ISort>
                {
                    //new  SortField { Field = IdField, Order = SortOrder.Descending }
                }
            };

            var searchResponse = await this.Client.SearchAsync<TDocument>(searchRequest);

            return searchResponse.Total;
        }

        public async Task<List<TDocument>> GetAllAsync(QueryContainerDescriptor<TDocument> query,
            string indexName = null)
        {
            var response = await this.Client.SearchAsync<TDocument>(s => s
                .Index(indexName.ToLower() ?? IndexName.ToLower())
                .Query(q => q = query)
                .Size(int.MaxValue)
                .From(0));

            //Translate the hits and return the list
            List<TDocument> typedList =
                Enumerable.ToList<TDocument>(response.Hits.Select(hit => ConvertHitToDocument(hit)));

            return typedList;
        }

        public async Task<List<TDocument>> GetAllAsync(SearchDescriptor<TDocument> search, string indexName = null)
        {
            var response = await this.Client.SearchAsync<TDocument>
            (s =>
            {
                s = search;

                return s
                    .Index(indexName .ToLower()?? IndexName.ToLower())
                    .Size(int.MaxValue)
                    .From(0);
            });

            //Translate the hits and return the list
            List<TDocument> typedList =
                Enumerable.ToList<TDocument>(response.Hits.Select(hit => ConvertHitToDocument(hit)));

            return typedList;
        }

        private TDocument ConvertHitToDocument(IHit<TDocument> hit)
        {
            Func<IHit<TDocument>, TDocument> func = (x) =>
            {
                hit.Source.Id = hit.Id;
                return hit.Source;
            };

            return func.Invoke(hit);
        }

        private IQueryContainer CreateSimpleQuery(FieldFilter filter, QueryContainer queryContainer)
        {
            switch (filter.OperatorType)
            {
                case OperatorType.And:
                    queryContainer &= new TermQuery() { Field = filter.Field, Value = filter.Value };

                    break;
                case OperatorType.Or:
                    queryContainer |= new TermQuery() { Field = filter.Field, Value = filter.Value };

                    break;
                default:
                    break;
            }

            return queryContainer;
        }

        private ElasticQueryContainer GetSearchQuery(CombinedFilter searchRequest)
        {
            //Populate ElasticQueryContainer : orQuery, andQuery, notQuery dynamically
            var elasticQueryContainer = new ElasticQueryContainer();

            foreach (var item in searchRequest.AndItems)
            {
                elasticQueryContainer.andQuery.Add(new TermQuery() { Field = item.Field, Value = item.Value });
            }

            foreach (var item in searchRequest.OrItems)
            {
                elasticQueryContainer.orQuery.Add(new TermQuery() { Field = item.Field, Value = item.Value });
            }

            foreach (var item in searchRequest.NotItems)
            {
                elasticQueryContainer.notQuery.Add(new TermQuery() { Field = item.Field, Value = item.Value });
            }

            return elasticQueryContainer;
        }

        private ElasticQueryContainer GetSearchQueryByFilters(List<ElasticFilter> searchRequest)
        {
            //Populate ElasticQueryContainer : orQuery, andQuery, notQuery dynamically
            var elasticQueryContainer = new ElasticQueryContainer();

            if (searchRequest != null)
            {
                foreach (var item in searchRequest)
                {
                    switch (item.FilterType)
                    {
                        case FilterType.Is:
                            switch (item.OperatorType)
                            {
                                case OperatorType.And:
                                    elasticQueryContainer.andQuery.Add(new TermQuery()
                                        { Field = item.Field, Value = item.Values.FirstOrDefault() });
                                    break;
                                case OperatorType.Or:
                                    elasticQueryContainer.orQuery.Add(new TermQuery()
                                        { Field = item.Field, Value = item.Values.FirstOrDefault() });
                                    break;
                                case OperatorType.Not:
                                    break;
                                default:
                                    break;
                            }

                            break;
                        case FilterType.IsNot:
                            //TODO:
                            break;
                        case FilterType.IsOneOf:
                            switch (item.OperatorType)
                            {
                                case OperatorType.And:
                                    elasticQueryContainer.andQuery.Add(new TermsQuery()
                                        { Field = item.Field, Terms = item.Values });
                                    break;
                                case OperatorType.Or:
                                    elasticQueryContainer.orQuery.Add(new TermsQuery()
                                        { Field = item.Field, Terms = item.Values });
                                    break;
                                case OperatorType.Not:
                                    break;
                                default:
                                    break;
                            }

                            break;
                        case FilterType.IsNotOneOf:
                            //TODO:
                            break;
                        case FilterType.IsBetween:
                            switch (item.OperatorType)
                            {
                                case OperatorType.And:
                                    elasticQueryContainer.andQuery.Add(GetQueryContainerByFilterAsync(item));
                                    break;
                                case OperatorType.Or:
                                    elasticQueryContainer.orQuery.Add(GetQueryContainerByFilterAsync(item));
                                    break;
                                case OperatorType.Not:
                                    break;
                                default:
                                    break;
                            }

                            break;
                        case FilterType.IsNotBetween:
                            //TODO:
                            break;
                        case FilterType.Exists:
                            //TODO:
                            break;
                        default:
                            break;
                    }
                }
            }

            return elasticQueryContainer;
        }

        private IQueryContainer ExecuteNumericRangesQuery(ElasticFilter filter)
        {
            var dataFilter = new QueryContainer();

            switch (filter.ComparisonOperatorType)
            {
                case ComparisonOperatorType.GreaterThanOrEquals:
                    dataFilter = new QueryContainerDescriptor<TDocument>().Range(t =>
                        t.Field(filter.Field)
                            .GreaterThanOrEquals((double?)filter.Values.FirstOrDefault()));
                    break;
                case ComparisonOperatorType.GreaterThan:
                    dataFilter = new QueryContainerDescriptor<TDocument>().Range(t =>
                        t.Field(filter.Field)
                            .GreaterThan((double?)filter.Values.FirstOrDefault()));
                    break;
                case ComparisonOperatorType.LessThan:
                    dataFilter = new QueryContainerDescriptor<TDocument>().Range(t =>
                        t.Field(filter.Field)
                            .LessThan((double?)filter.Values.FirstOrDefault()));
                    break;
                case ComparisonOperatorType.LessThanOrEquals:
                    dataFilter = new QueryContainerDescriptor<TDocument>().Range(t =>
                        t.Field(filter.Field)
                            .LessThanOrEquals((double?)filter.Values.FirstOrDefault()));
                    break;
                default:
                    break;
            }

            return dataFilter;
        }

        private IQueryContainer ExecuteDateRangesQuery(ElasticFilter filter)
        {
            var dataFilter = new QueryContainer();

            var date = (DateTime)filter.Values.FirstOrDefault();

            var threshold = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute,
                date.Millisecond / 1000);

            switch (filter.ComparisonOperatorType)
            {
                case ComparisonOperatorType.GreaterThanOrEquals:
                    dataFilter = new QueryContainerDescriptor<TDocument>().DateRange(t =>
                        t.Field(filter.Field)
                            .GreaterThanOrEquals(threshold));
                    break;
                case ComparisonOperatorType.GreaterThan:
                    dataFilter = new QueryContainerDescriptor<TDocument>().DateRange(t =>
                        t.Field(filter.Field)
                            .GreaterThan(threshold));
                    break;
                case ComparisonOperatorType.LessThan:
                    dataFilter = new QueryContainerDescriptor<TDocument>().DateRange(t =>
                        t.Field(filter.Field)
                            .LessThan(threshold));
                    break;
                case ComparisonOperatorType.LessThanOrEquals:
                    dataFilter = new QueryContainerDescriptor<TDocument>().DateRange(t =>
                        t.Field(filter.Field)
                            .LessThanOrEquals(threshold));
                    break;
                default:
                    break;
            }

            return dataFilter;
        }

        private IQueryContainer ExecuteTermRangesQuery(ElasticFilter filter)
        {
            var dataFilter = new QueryContainer();

            switch (filter.ComparisonOperatorType)
            {
                case ComparisonOperatorType.GreaterThanOrEquals:
                    dataFilter = new QueryContainerDescriptor<TDocument>().TermRange(t =>
                        t.Field(filter.Field)
                            .GreaterThanOrEquals(filter.Values.FirstOrDefault().ToString()));
                    break;
                case ComparisonOperatorType.GreaterThan:
                    dataFilter = new QueryContainerDescriptor<TDocument>().TermRange(t =>
                        t.Field(filter.Field)
                            .GreaterThan(filter.Values.FirstOrDefault().ToString()));
                    break;
                case ComparisonOperatorType.LessThan:
                    dataFilter = new QueryContainerDescriptor<TDocument>().TermRange(t =>
                        t.Field(filter.Field)
                            .LessThan(filter.Values.FirstOrDefault().ToString()));
                    break;
                case ComparisonOperatorType.LessThanOrEquals:
                    dataFilter = new QueryContainerDescriptor<TDocument>().TermRange(t =>
                        t.Field(filter.Field)
                            .LessThanOrEquals(filter.Values.FirstOrDefault().ToString()));
                    break;
                default:
                    break;
            }

            return dataFilter;
        }

        private IQueryContainer ExecuteLongRangesQuery(ElasticFilter filter)
        {
            var dataFilter = new QueryContainer();

            switch (filter.ComparisonOperatorType)
            {
                case ComparisonOperatorType.GreaterThanOrEquals:
                    dataFilter = new QueryContainerDescriptor<TDocument>().LongRange(t =>
                        t.Field(filter.Field)
                            .GreaterThanOrEquals((long?)filter.Values.FirstOrDefault()));
                    break;
                case ComparisonOperatorType.GreaterThan:
                    dataFilter = new QueryContainerDescriptor<TDocument>().LongRange(t =>
                        t.Field(filter.Field)
                            .GreaterThan((long?)filter.Values.FirstOrDefault()));
                    break;
                case ComparisonOperatorType.LessThan:
                    dataFilter = new QueryContainerDescriptor<TDocument>().LongRange(t =>
                        t.Field(filter.Field)
                            .LessThan((long?)filter.Values.FirstOrDefault()));
                    break;
                case ComparisonOperatorType.LessThanOrEquals:
                    dataFilter = new QueryContainerDescriptor<TDocument>().LongRange(t =>
                        t.Field(filter.Field)
                            .LessThanOrEquals((long?)filter.Values.FirstOrDefault()));
                    break;
                default:
                    break;
            }

            return dataFilter;
        }

        private IQueryContainer ExecuteNumericRangesQuery(RangeFilter filter)
        {
            var dataFilter = new QueryContainer();

            switch (filter.ComparisonOperatorType)
            {
                case ComparisonOperatorType.GreaterThanOrEquals:
                    dataFilter = new QueryContainerDescriptor<TDocument>().Range(t =>
                        t.Field(filter.Field)
                            .GreaterThanOrEquals((double?)filter.Value));
                    break;
                case ComparisonOperatorType.GreaterThan:
                    dataFilter = new QueryContainerDescriptor<TDocument>().Range(t =>
                        t.Field(filter.Field)
                            .GreaterThan((double?)filter.Value));
                    break;
                case ComparisonOperatorType.LessThan:
                    dataFilter = new QueryContainerDescriptor<TDocument>().Range(t =>
                        t.Field(filter.Field)
                            .LessThan((double?)filter.Value));
                    break;
                case ComparisonOperatorType.LessThanOrEquals:
                    dataFilter = new QueryContainerDescriptor<TDocument>().Range(t =>
                        t.Field(filter.Field)
                            .LessThanOrEquals((double?)filter.Value));
                    break;
                default:
                    break;
            }

            return dataFilter;
        }

        private IQueryContainer ExecuteDateRangesQuery(RangeFilter filter)
        {
            var dataFilter = new QueryContainer();

            switch (filter.ComparisonOperatorType)
            {
                case ComparisonOperatorType.GreaterThanOrEquals:
                    dataFilter = new QueryContainerDescriptor<TDocument>().DateRange(t =>
                        t.Field(filter.Field)
                            .GreaterThanOrEquals((DateMath)filter.Value));
                    break;
                case ComparisonOperatorType.GreaterThan:
                    dataFilter = new QueryContainerDescriptor<TDocument>().DateRange(t =>
                        t.Field(filter.Field)
                            .GreaterThan((DateMath)filter.Value));
                    break;
                case ComparisonOperatorType.LessThan:
                    dataFilter = new QueryContainerDescriptor<TDocument>().DateRange(t =>
                        t.Field(filter.Field)
                            .LessThan((DateMath)filter.Value));
                    break;
                case ComparisonOperatorType.LessThanOrEquals:
                    dataFilter = new QueryContainerDescriptor<TDocument>().DateRange(t =>
                        t.Field(filter.Field)
                            .LessThanOrEquals((DateMath)filter.Value));
                    break;
                default:
                    break;
            }

            return dataFilter;
        }

        private IQueryContainer ExecuteTermRangesQuery(RangeFilter filter)
        {
            var dataFilter = new QueryContainer();

            switch (filter.ComparisonOperatorType)
            {
                case ComparisonOperatorType.GreaterThanOrEquals:
                    dataFilter = new QueryContainerDescriptor<TDocument>().TermRange(t =>
                        t.Field(filter.Field)
                            .GreaterThanOrEquals(filter.Value.ToString()));
                    break;
                case ComparisonOperatorType.GreaterThan:
                    dataFilter = new QueryContainerDescriptor<TDocument>().TermRange(t =>
                        t.Field(filter.Field)
                            .GreaterThan(filter.Value.ToString()));
                    break;
                case ComparisonOperatorType.LessThan:
                    dataFilter = new QueryContainerDescriptor<TDocument>().TermRange(t =>
                        t.Field(filter.Field)
                            .LessThan(filter.Value.ToString()));
                    break;
                case ComparisonOperatorType.LessThanOrEquals:
                    dataFilter = new QueryContainerDescriptor<TDocument>().TermRange(t =>
                        t.Field(filter.Field)
                            .LessThanOrEquals(filter.Value.ToString()));
                    break;
                default:
                    break;
            }

            return dataFilter;
        }

        private IQueryContainer ExecuteLongRangesQuery(RangeFilter filter)
        {
            var dataFilter = new QueryContainer();

            switch (filter.ComparisonOperatorType)
            {
                case ComparisonOperatorType.GreaterThanOrEquals:
                    dataFilter = new QueryContainerDescriptor<TDocument>().LongRange(t =>
                        t.Field(filter.Field)
                            .GreaterThanOrEquals((long?)filter.Value));
                    break;
                case ComparisonOperatorType.GreaterThan:
                    dataFilter = new QueryContainerDescriptor<TDocument>().LongRange(t =>
                        t.Field(filter.Field)
                            .GreaterThan((long?)filter.Value));
                    break;
                case ComparisonOperatorType.LessThan:
                    dataFilter = new QueryContainerDescriptor<TDocument>().LongRange(t =>
                        t.Field(filter.Field)
                            .LessThan((long?)filter.Value));
                    break;
                case ComparisonOperatorType.LessThanOrEquals:
                    dataFilter = new QueryContainerDescriptor<TDocument>().LongRange(t =>
                        t.Field(filter.Field)
                            .LessThanOrEquals((long?)filter.Value));
                    break;
                default:
                    break;
            }

            return dataFilter;
        }

        private async Task ExecuteSumAggregation(
            Aggregations aggregations, Dictionary<string, string> result, ElasticQueryContainer elasticQueryContainer,
            string indexName = null)
        {
            var agg_name = string.Concat("agg_", aggregations.AggregationField);

            var g_name = string.Concat("g_", aggregations.AggregatorField);

            var response = await this.Client.SearchAsync<TDocument>(s => s
                .Index(indexName.ToLower() ?? IndexName.ToLower())
                .Query(q => q
                    .Bool(bq => bq
                        .Should(elasticQueryContainer.orQuery.ToArray())
                        .Must(elasticQueryContainer.andQuery.ToArray())
                        .MustNot(elasticQueryContainer.notQuery.ToArray())
                    ))
                .Aggregations(a => a
                    .Terms(g_name, st => st
                        .Field(aggregations.AggregatorField)
                        .Size(int.MaxValue)
                        .Aggregations(aa => aa
                            .Sum(agg_name, m => m
                                .Field(aggregations.AggregationField)
                            )
                        )
                    ))
            );

            foreach (var term in response.Aggregations.Terms(g_name).Buckets)
            {
                result.Add(term.Key.ToString(), term.Sum(agg_name).ValueAsString);
            }
        }

        private async Task ExecuteAvgAggregation(
            Aggregations aggregations, Dictionary<string, string> result, ElasticQueryContainer elasticQueryContainer,
            string indexName = null)
        {
            var agg_name = string.Concat("agg_", aggregations.AggregationField);

            var g_name = string.Concat("g_", aggregations.AggregatorField);

            var response = await this.Client.SearchAsync<TDocument>(s => s
                .Index(indexName.ToLower() ?? IndexName.ToLower())
                .Query(q => q
                    .Bool(bq => bq
                        .Should(elasticQueryContainer.orQuery.ToArray())
                        .Must(elasticQueryContainer.andQuery.ToArray())
                        .MustNot(elasticQueryContainer.notQuery.ToArray())
                    ))
                .Aggregations(a => a
                    .Terms(g_name, st => st
                        .Field(aggregations.AggregatorField)
                        .Size(int.MaxValue)
                        .Aggregations(aa => aa
                            .Average(agg_name, m => m
                                .Field(aggregations.AggregationField)
                            )
                        )
                    ))
            );

            foreach (var term in response.Aggregations.Terms(g_name).Buckets)
            {
                result.Add(term.Key.ToString(), term.Average(agg_name).ValueAsString);
            }
        }

        private async Task ExecuteCountAggregation(
            Aggregations aggregations, Dictionary<string, string> result, ElasticQueryContainer elasticQueryContainer,
            string indexName = null)
        {
            var agg_nickname = string.Concat("agg_", aggregations.AggregationField);

            var response = await this.Client.SearchAsync<TDocument>(s => s
                .Index(indexName.ToLower() ?? IndexName.ToLower())
                .Aggregations(a => a
                    .Terms(agg_nickname, st => st
                        .Field(aggregations.AggregationField)
                        .Size(int.MaxValue)
                        .ExecutionHint(TermsAggregationExecutionHint.GlobalOrdinals)
                    )
                )
            );

            foreach (var item in response.Aggregations.Terms(agg_nickname).Buckets)
            {
                result.Add(item.Key, item.DocCount.ToString());
            }
        }

        private async Task ExecuteMinAggregation(
            Aggregations aggregations, Dictionary<string, string> result, ElasticQueryContainer elasticQueryContainer,
            string indexName = null)
        {
            var agg_name = string.Concat("agg_", aggregations.AggregationField);

            var g_name = string.Concat("g_", aggregations.AggregatorField);

            var response = await this.Client.SearchAsync<TDocument>(s => s
                .Index(indexName.ToLower() ?? IndexName.ToLower())
                .Query(q => q
                    .Bool(bq => bq
                        .Should(elasticQueryContainer.orQuery.ToArray())
                        .Must(elasticQueryContainer.andQuery.ToArray())
                        .MustNot(elasticQueryContainer.notQuery.ToArray())
                    ))
                .Aggregations(a => a
                    .Terms(g_name, st => st
                        .Field(aggregations.AggregatorField)
                        .Size(int.MaxValue)
                        .Aggregations(aa => aa
                            .Min(agg_name, m => m
                                .Field(aggregations.AggregationField)
                            )
                        )
                    ))
            );

            foreach (var term in response.Aggregations.Terms(g_name).Buckets)
            {
                result.Add(term.Key.ToString(), term.Min(agg_name).ValueAsString);
            }
        }

        private async Task ExecuteMaxAggregation(
            Aggregations aggregations, Dictionary<string, string> result, ElasticQueryContainer elasticQueryContainer,
            string indexName = null)
        {
            var agg_name = string.Concat("agg_", aggregations.AggregationField);

            var g_name = string.Concat("g_", aggregations.AggregatorField);

            var response = await this.Client.SearchAsync<TDocument>(s => s
                .Index(indexName.ToLower() ?? IndexName.ToLower())
                .Query(q => q
                    .Bool(bq => bq
                        .Should(elasticQueryContainer.orQuery.ToArray())
                        .Must(elasticQueryContainer.andQuery.ToArray())
                        .MustNot(elasticQueryContainer.notQuery.ToArray())
                    ))
                .Aggregations(a => a
                    .Terms(g_name, st => st
                        .Field(aggregations.AggregatorField)
                        .Size(int.MaxValue)
                        .Aggregations(aa => aa
                            .Max(agg_name, m => m
                                .Field(aggregations.AggregationField)
                            )
                        )
                    ))
            );

            foreach (var term in response.Aggregations.Terms(g_name).Buckets)
            {
                result.Add(term.Key.ToString(), term.Max(agg_name).ValueAsString);
            }
        }
    }
}