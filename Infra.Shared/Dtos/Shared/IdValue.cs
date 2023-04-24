namespace Infra.Shared.Dtos.Shared;

public class IdValue<TId, TValue>
{
    public TId Id { get; set; }
    public TValue Value { get; set; }
}