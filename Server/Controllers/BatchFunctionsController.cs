using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.OData.Query;
using IOGKFExams.Server.Models;
using IOGKFExams.Server.Data;
using IOGKFExams.Server.Models.IOGKFExamsDb;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Text;

namespace IOGKFExams.Server.Controllers
{
    public class BatchFunctionsController : Controller
    {
        private readonly IWebHostEnvironment environment;
        private readonly IOGKFExamsDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private string baseUrl;

        public BatchFunctionsController(IWebHostEnvironment environment, IOGKFExamsDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.environment = environment;
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            var request = httpContextAccessor.HttpContext?.Request;
            if (request != null)
            {
                this.baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            }
        }

        [HttpGet("BatchFunctions/createsingleexam")]
        public async Task<IActionResult> CreateSingleExam([FromQuery] int templateId, [FromQuery] int examId)
        {
            try
            {
                var exam = context.Exams.Find(examId);
                if (exam != null)
                {
                    var template = await context.ExamTemplates.FindAsync(templateId);
                    var questions = await context.ExamTemplateQuestions.Where(x => x.ExamTemplateId == template.ExamTemplateId).ToListAsync();
                    foreach (var question in questions)
                    {
                        var newQuestion = await context.ExamQuestions.AddAsync(new ExamQuestion()
                        {
                            Active = true,
                            ExamId = examId,
                            LanguageId = question.LanguageId,
                            MinimumRankRequiredId = question.MinimumRankRequiredId,
                            Question = question.Question,
                        });
                        await context.SaveChangesAsync();
                        var answers = await context.ExamTemplateAnswers.Where(x => x.ExamTemplateQuestionsId == question.ExamTemplateQuestionsId).ToListAsync();
                        foreach(var answer in answers)
                        {
                            await context.ExamAnswers.AddAsync(new ExamAnswer()
                            {
                                ExamAnswer1 = answer.ExamTemplateAnswer1,
                                IsCorrectAnswer = answer.IsCorrectAnswer,
                                IsSelectedAnswer = false,
                                ExamQuestionsId = newQuestion.Entity.ExamQuestionsId,
                                
                            });
                        }
                        await context.SaveChangesAsync();
                    }
                }
                else
                {
                    return StatusCode(500, $"Unable to Find Exam with ID: {examId}");
                }
                return Ok();
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }
    }
}
