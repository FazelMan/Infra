using System.Collections.Generic;
using Nest;

namespace Infra.ElasticSearch.Dtos
{
    public class ElasticQueryContainer
    {
        #region [[ Properties ]]

        public List<QueryContainer> orQuery { get; set; }
        public List<QueryContainer> andQuery { get; set; }
        public List<QueryContainer> notQuery { get; set; }
        public ElasticQueryContainer()
        {
            orQuery = new List<QueryContainer>();
            andQuery = new List<QueryContainer>();
            notQuery = new List<QueryContainer>();
        }

        #endregion
    }
}
