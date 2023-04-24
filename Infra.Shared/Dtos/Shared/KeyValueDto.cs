namespace Infra.Shared.Dtos.Shared
{
    public class KeyValueDto<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
    }

}