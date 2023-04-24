namespace Infra.ElasticSearch.Enums
{
    public enum RangeFilterType
    {
        /// <summary>
        /// Numeric range
        /// </summary>
        Numeric = 1,
        /// <summary>
        /// Long range
        /// </summary>
        Long = 2,
        /// <summary>
        /// Term range
        /// </summary>
        Term = 3,
        /// <summary>
        /// Date range
        /// </summary>
        Date = 4,
    }
}
