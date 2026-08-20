using IOGKFExams.Server.Helpers;
using IOGKFExams.Server.Models;
using QuestPDF.Fluent;

namespace IOGKFExams.Server.Services
{
    public class ExamPdfService : IExamPdfService
    {
        public byte[] GenerateStudentExam(ExamPdfModel exam)
        {
            var document =
                new ExamPdfDocument(
                    exam,
                    includeAnswerKey: false);

            return document.GeneratePdf();
        }

        public byte[] GenerateAnswerKey(ExamPdfModel exam)
        {
            var document =
                new ExamPdfDocument(
                    exam,
                    includeAnswerKey: true);

            return document.GeneratePdf();
        }
    }
}
