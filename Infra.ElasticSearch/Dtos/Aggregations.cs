using Infra.ElasticSearch.Enums;

namespace Infra.ElasticSearch.Dtos
{

    public class Aggregations
    {
        #region [[ Properties ]]

        /// <summary>
        /// Group By Field
        /// </summary>
        public string AggregatorField { get; set; }

        /// <summary>
        /// Aggregation Type
        /// Aggregation Type
        /// </summary>
        public AggregationType AggregationType { get; set; }

        /// <summary>
        /// Aggregation Field
        /// </summary>
        public string AggregationField { get; set; }

        #endregion
        
    }
}
