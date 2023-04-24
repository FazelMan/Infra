using System.Collections.Generic;

namespace Infra.ElasticSearch.Dtos
{
    public class CombinedFilter
    {
        #region [[ Properties ]]

        /// <summary>
        /// List of And Fields
        /// </summary>
        public List<FieldFilter> AndItems { get; set; }

        /// <summary>
        /// List of Or Fields
        /// </summary>
        public List<FieldFilter> OrItems { get; set; }

        /// <summary>
        /// List of Not Fields
        /// </summary>
        public List<FieldFilter> NotItems { get; set; }

        #endregion
    }
}
