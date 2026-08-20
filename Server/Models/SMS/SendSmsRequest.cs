namespace IOGKFExams.Server.Models.Sms
{
    public class SendSmsRequest
    {
        public string To { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
