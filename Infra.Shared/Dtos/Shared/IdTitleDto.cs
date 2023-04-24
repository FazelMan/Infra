namespace Infra.Shared.Dtos.Shared;

public class IdTitleDto<TId, TTitle>
{
    public TId Id { get; set; }
    public TTitle Title { get; set; }
}

public class IdTitleValueDto<TId, TTitle, TValue> : IdTitleDto<TId, TTitle>
{
    public TValue Value { get; set; }
}
