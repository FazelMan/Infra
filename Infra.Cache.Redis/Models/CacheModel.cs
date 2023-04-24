using System;

namespace Infra.Cache.Redis.Models
{
    [Serializable()]
    public class CacheModel<T>
    {
        public CacheModel(T data, int hits = 0, int version = 0, DateTime? created = null, DateTime? lastmodified = null)
        {
            this.Data = data;
            this.Hits = hits;
            this.Version = version;
            this.Created = created ?? DateTime.UtcNow;
            this.LastModified = lastmodified ?? DateTime.UtcNow;
        }

        public T Data { get; set; }
        public int Version { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public int Hits { get; set; }
    }
}
