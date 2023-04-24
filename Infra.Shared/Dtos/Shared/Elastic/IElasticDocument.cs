using System;

namespace Infra.Shared.Dtos.Shared.Elastic
{
    public interface IElasticDocument
    {
        string Id { get; set; }
        DateTime? CreatedDate { get; set; }
        DateTime? ModifiedDate { get; set; }
    }
}
