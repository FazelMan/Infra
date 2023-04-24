namespace Infra.EntityFramework.Dtos.Entities.Auditing
{
    public abstract class AuditedEntity : IState
    {
        public bool IsRemoved { get; set; }
    }
}