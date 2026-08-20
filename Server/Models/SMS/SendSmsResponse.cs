namespace IOGKFExams.Server.Models.Sms
{
    public class SendSmsResponse
    {
        public bool Success { get; set; }

        public string? MessageSid { get; set; }

        public string? Status { get; set; }

        public string? ErrorMessage { get; set; }
    }
}