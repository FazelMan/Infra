using System.ComponentModel;

namespace Infra.Shared.Enums
{
    public enum PaginationType
    {
        [Description("CountPerPage and CurrentPageNumber")]
        Old,

        [Description("PageSize and PageNumber")]
        New
    }
}