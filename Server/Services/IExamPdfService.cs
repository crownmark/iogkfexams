using IOGKFExams.Server.Models;

namespace IOGKFExams.Server.Services
{
    public interface IExamPdfService
    {
        byte[] GenerateStudentExam(ExamPdfModel exam);
        byte[] GenerateAnswerKey(ExamPdfModel exam);
    }
}
