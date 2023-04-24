namespace Infra.EntityFramework.Dtos.Entities.Auditing
{
    public interface IState
    {
        bool IsRemoved { get; set; }
    }
}
