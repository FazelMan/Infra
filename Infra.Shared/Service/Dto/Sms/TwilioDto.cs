namespace Infra.Shared.Service.Dto.Sms
{
    public class TwilioDto
    {
        public string AccountSId { get; set; }
        public string AuthToken { get; set; }
        public string FromNumber { get; set; }
    }
}