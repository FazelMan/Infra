namespace Infra.Shared.Dtos.Shared
{
    public class ErrorDto
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string? Extra { get; set; }
    }
}