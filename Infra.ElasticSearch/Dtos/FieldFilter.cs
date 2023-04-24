using Infra.ElasticSearch.Enums;

namespace Infra.ElasticSearch.Dtos
{
    public class FieldFilter
    {
        #region [[ Properties ]]

        /// <summary>
        /// Field Name
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Filter Type
        /// </summary>
        public OperatorType OperatorType { get; set; }

        #endregion
    }
}
