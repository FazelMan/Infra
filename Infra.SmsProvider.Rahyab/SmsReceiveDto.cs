namespace Infra.SmsProvider.Rahyab
{
    public class SmsReceiveDto
    {
        public int MessageNumber { get; set; }
        public string Message { get; set; }
        public string SenderNumber { get; set; }
        public string ReceiverNumber { get; set; }
    }
}
