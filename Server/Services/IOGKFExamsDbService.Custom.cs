using DocumentFormat.OpenXml.InkML;
using IOGKFExams.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace IOGKFExams.Server
{
    public partial class IOGKFExamsDbService
    {
        public async Task<ExamPdfModel?> GetExamForPdf(string examGuid)
        {
            // Get the actual exam instance
            var exams = context.Exams.Include(x => x.Rank).Include(x => x.Language).Where(x => x.ExamGuid == examGuid);

            var exam = exams.FirstOrDefault();

            if (exam == null)
                return null;

            // Get questions assigned to this exam
            var questionsResult = context.ExamQuestions.Where(x => x.ExamId == exam.ExamId);

            var questions = questionsResult.ToList();

            var model = new ExamPdfModel
            {
                ExamId = exam.ExamId,
                ExamTitle = "IOGKF Black Belt Examination",

                // Adjust these property names to your actual Exams table
                StudentName = exam.StudentFirstName + " " + exam.StudentLastName ?? "",
                Rank = exam.Rank?.RankName ?? "",
                Language = exam.Language?.LanguageName ?? ""
            };

            int questionNumber = 1;

            foreach (var question in questions)
            {
                var answersResult = context.ExamAnswers.Where(x => x.ExamQuestionsId == question.ExamQuestionsId); 

                var pdfQuestion = new ExamPdfQuestion
                {
                    QuestionNumber = questionNumber++,
                    Question = question.Question,
                    Answers = answersResult
                        .Select(answer => new ExamPdfAnswer
                        {
                            Answer = answer.ExamAnswer1,
                            IsCorrect = answer.IsCorrectAnswer
                        })
                        .ToList()
                };

                model.Questions.Add(pdfQuestion);
            }

            return model;
        }
    }
}
