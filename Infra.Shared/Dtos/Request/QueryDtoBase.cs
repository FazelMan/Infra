namespace Infra.Shared.Dtos.Request
{
    public class QueryDtoBase
    {
        public bool Asc { get; set; }
        
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 50;
    }
}