using System.Collections.Generic;

namespace Infra.Shared.Dtos.Shared
{
    public class PagedList<T> where T : class
    {
        public PagedList()
        {
            Data = new List<T>();
        }

        public IEnumerable<T> Data { get; set; }

        public int TotalCount { get; set; }
        public object ExtraData { get; set; }
    }

    public class PricePagedList<T> : PagedList<T> where T : class
    {
        public PriceDto Price { get; set; }
    }
}
