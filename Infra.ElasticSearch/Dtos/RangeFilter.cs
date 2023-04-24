using Infra.ElasticSearch.Enums;

namespace Infra.ElasticSearch.Dtos
{
    public class RangeFilter
    {
        #region [[ Properties ]]

        /// <summary>
        /// Field Name
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Threshold Value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Range filter type
        /// </summary>
        public RangeFilterType RangeFilterType { get; set; }

        /// <summary>
        /// Comparison Operator Type
        /// </summary>
        public ComparisonOperatorType ComparisonOperatorType { get; set; }

        /// <summary>
        /// IsNested
        /// </summary>
        public bool IsNested { get; set; }

        /// <summary>
        /// Parent Field Name
        /// </summary>
        public string Parent { get; set; }
        
        #endregion
    }
}
