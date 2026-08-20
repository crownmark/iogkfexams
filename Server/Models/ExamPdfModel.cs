namespace IOGKFExams.Server.Models
{
    public class ExamPdfModel
    {
        public int ExamId { get; set; } = 0;
        public string ExamTitle { get; set; } = "";
        public string StudentName { get; set; } = "";
        public string Rank { get; set; } = "";
        public string Language { get; set; } = "";

        public List<ExamPdfQuestion> Questions { get; set; } = new();
    }

    public class ExamPdfQuestion
    {
        public int QuestionNumber { get; set; }
        public string Question { get; set; } = "";
        public byte[]? Image { get; set; }

        public List<ExamPdfAnswer> Answers { get; set; } = new();
    }

    public class ExamPdfAnswer
    {
        public string Answer { get; set; } = "";
        public bool IsCorrect { get; set; }
    }
}
