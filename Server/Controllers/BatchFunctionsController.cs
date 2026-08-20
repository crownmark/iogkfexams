using IOGKFExams.Server.Data;
using IOGKFExams.Server.Helpers;
using IOGKFExams.Server.Models;
using IOGKFExams.Server.Models.IOGKFExamsDb;
using IOGKFExams.Server.Models.Sms;
using IOGKFExams.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Radzen;
using System.Security.Cryptography;
using System.Text;

namespace IOGKFExams.Server.Controllers
{
    public class BatchFunctionsController : Controller
    {
        private readonly IWebHostEnvironment environment;
        private readonly IOGKFExamsDbContext context;
        private readonly IOGKFExams.Server.IOGKFExamsDbService _examDbService;
        private readonly IExamPdfService _pdfService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private string baseUrl;
        private readonly IConfiguration configuration;


        public BatchFunctionsController(IWebHostEnvironment environment, IOGKFExamsDbContext context, IHttpContextAccessor httpContextAccessor,IConfiguration configuration, IOGKFExams.Server.IOGKFExamsDbService examDbService,
        IExamPdfService pdfService)
        {
            this.environment = environment;
            this.context = context;
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
            this._examDbService = examDbService;
            this._pdfService = pdfService;
            var request = httpContextAccessor.HttpContext?.Request;
            if (request != null)
            {
                this.baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            }
        }
        public static int GenerateExamSessionCode(int examId)
        {
            // Combine examId with a precise UTC timestamp for uniqueness
            string input = $"{examId}_{DateTime.UtcNow:yyyyMMddHHmmssfff}";

            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                int hashValue = BitConverter.ToInt32(hashBytes, 0);
                int positiveHash = Math.Abs(hashValue);

                // Ensure result is always a 6-digit number between 100000–999999
                return 100000 + (positiveHash % 900000);
            }
        }

        [HttpGet("BatchFunctions/SendExamSms")]
        public async Task<SendSmsResponse> SendExamSms(string phoneNumber, string message)
        {
            var _httpClient = new HttpClient();
       
            var request = new SendSmsRequest
            {
                To = phoneNumber,
                Message = message
            };

            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/sms/send",
                    request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content
                           .ReadFromJsonAsync<SendSmsResponse>()
                       ?? new SendSmsResponse
                       {
                           Success = false,
                           ErrorMessage =
                               "Invalid response from SMS service."
                       };
            }

            var error =
                await response.Content
                    .ReadFromJsonAsync<SendSmsResponse>();

            return error ?? new SendSmsResponse
            {
                Success = false,
                ErrorMessage =
                    $"SMS request failed: {response.StatusCode}"
            };
        }

        [HttpGet("BatchFunctions/SendExamEmail")]
        public async Task<IActionResult> SendExamEmail([FromQuery] int examId)
        {
            try
            {
                var exam = context.Exams.Find(examId);
                if (exam != null)
                {
                    // Send Email Message to Student
                    if (!string.IsNullOrEmpty(exam.StudentEmail))
                    {
                        try
                        {
                            var notificationTemplate = context.NotificationTemplates.Where(x => x.LanguageId == exam.LanguageId && x.Active == true && x.Channel == "Email").FirstOrDefault();
                            if (notificationTemplate != null)
                            {

                                var tokens = new Dictionary<string, string>
                                {
                                    { "StudentFirstName", exam.StudentFirstName },
                                    { "StudentLastName", exam.StudentLastName },
                                    { "ExamId", exam.ExamId.ToString() },
                                    { "ExamSessionCode", exam.ExamSessionCode.ToString() },
                                    { "ExamUrl", $"https://demoapp.crown.software/take-exam/{exam.ExamGuid}" }
                                };
                                var updatedNotificationTemplate = TokenReplacementHelper.ReplaceTokens(notificationTemplate, tokens);
                                // Send Email Logic Here    
                                await SendEmailAsync(exam.StudentEmail, updatedNotificationTemplate.Subject, updatedNotificationTemplate.MessageBody);
                            }
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, $"Error sending email to {exam.StudentEmail} for Exam with ID: {examId}.  Error: {ex.Message}");

                        }

                    }
                    else
                    {
                        return StatusCode(500, $"Student Email is empty for Exam with ID: {examId}");
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

        [HttpGet("BatchFunctions/createsingleexam")]
        public async Task<IActionResult> CreateSingleExam([FromQuery] int templateId, [FromQuery] int examId, [FromQuery] bool sendExam)
        {
            try
            {
                var exam = context.Exams.Find(examId);
                if (exam != null)
                {
                    var template = await context.ExamTemplates.FindAsync(templateId);
                    var questions = await context.ExamTemplateQuestions.Where(x => x.ExamTemplateId == template.ExamTemplateId && x.MinimumRankRequiredId <= exam.StudentRankId).ToListAsync();
                    foreach (var question in questions)
                    {
                        var newQuestion = await context.ExamQuestions.AddAsync(new ExamQuestion()
                        {
                            Active = true,
                            ExamId = examId,
                            LanguageId = question.LanguageId,
                            MinimumRankRequiredId = question.MinimumRankRequiredId,
                            Question = question.Question,
                            QuestionImageUrl = question.QuestionImageUrl,
                        });
                        await context.SaveChangesAsync();
                        var answers = await context.ExamTemplateAnswers.Where(x => x.ExamTemplateQuestionsId == question.ExamTemplateQuestionsId).ToListAsync();
                        foreach (var answer in answers)
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

                    //Create PDF for Exam
                    await GenerateExamPdf(exam.ExamGuid);

                    // Send Email Message to Student
                    if (!string.IsNullOrEmpty(exam.StudentEmail) && sendExam)
                    {
                        try
                        {
                            var notificationTemplate = context.NotificationTemplates.Where(x => x.LanguageId == exam.LanguageId && x.Active == true && x.Channel == "Email").FirstOrDefault();
                            if (notificationTemplate != null)
                            {

                                var tokens = new Dictionary<string, string>
                                {
                                    { "StudentFirstName", exam.StudentFirstName },
                                    { "StudentLastName", exam.StudentLastName },
                                    { "ExamId", exam.ExamId.ToString() },
                                    { "ExamSessionCode", exam.ExamSessionCode.ToString() },
                                    { "ExamUrl", $"https://demoapp.crown.software/take-exam/{exam.ExamGuid}" }
                                };
                                var updatedNotificationTemplate = TokenReplacementHelper.ReplaceTokens(notificationTemplate, tokens);
                                // Send Email Logic Here    
                                await SendEmailAsync(exam.StudentEmail, updatedNotificationTemplate.Subject, updatedNotificationTemplate.MessageBody);
                            }
                        }
                        catch (Exception ex)
                        {
                            
                        }

                    }
                    // Send SMS Message to Student    
                    if (!string.IsNullOrEmpty(exam.StudentMobilePhoneE164) && sendExam)
                    {
                        try
                        {
                            var notificationTemplate = context.NotificationTemplates.Where(x => x.LanguageId == exam.LanguageId && x.Active == true && x.Channel == "SMS").FirstOrDefault();
                            if (notificationTemplate != null)
                            {

                                var tokens = new Dictionary<string, string>
                                {
                                    { "StudentFirstName", exam.StudentFirstName },
                                    { "StudentLastName", exam.StudentLastName },
                                    { "ExamId", exam.ExamId.ToString() },
                                    { "ExamSessionCode", exam.ExamSessionCode.ToString() },
                                    { "ExamUrl", $"https://demoapp.crown.software/take-exam/{exam.ExamGuid}" }
                                };
                                var updatedNotificationTemplate = TokenReplacementHelper.ReplaceTokens(notificationTemplate, tokens);
                                // Send SMS Logic Here   
                                await SendExamSms(exam.StudentMobilePhoneE164, updatedNotificationTemplate.MessageBody);
                            }
                        }
                        catch (Exception ex)
                        {

                        }

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

        [HttpGet("BatchFunctions/GenerateExamPdf/{examGuid}")]
        public async Task GenerateExamPdf(string examGuid)
        {
            var examForPdf = await _examDbService.GetExamForPdf(examGuid);
            var exam = await context.Exams.FindAsync(examForPdf.ExamId);

            if (examForPdf == null)
                throw new Exception("Unable to generate PDF. Exam was not found.");

            var folder = Path.Combine(
                environment.WebRootPath,
                "GeneratedExams");

            Directory.CreateDirectory(folder);

            // Student exam
            var studentPdf =
                _pdfService.GenerateStudentExam(examForPdf);

            var studentFileName =
                $"IOGKF-Exam-{examForPdf.ExamId}.pdf";

            var studentFilePath =
                Path.Combine(folder, studentFileName);

            await System.IO.File.WriteAllBytesAsync(
                studentFilePath,
                studentPdf);

            exam.StudentPdfExam = $"{baseUrl}/GeneratedExams/{studentFileName}";


            // Answer key
            var answerKeyPdf =
                _pdfService.GenerateAnswerKey(examForPdf);

            var answerKeyFileName =
                $"IOGKF-Exam-{examForPdf.ExamId}-Key.pdf";

            var answerKeyFilePath =
                Path.Combine(folder, answerKeyFileName);

            await System.IO.File.WriteAllBytesAsync(
                answerKeyFilePath,
                answerKeyPdf);

            exam.InstructorPdfExamKey = $"{baseUrl}/GeneratedExams/{answerKeyFileName}";
            context.Exams.Update(exam);
            context.SaveChanges();
        }

        [HttpGet("BatchFunctions/SendEmailAsync/{to}/{subject}/{body}")]
        public async Task SendEmailAsync(string to, string subject, string body)
        {

            var mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.From = new System.Net.Mail.MailAddress(configuration.GetValue<string>("Smtp:User"));
            mailMessage.Body = body;
            mailMessage.Subject = subject;
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
            mailMessage.IsBodyHtml = true;
            mailMessage.To.Add(to);

            var client = new System.Net.Mail.SmtpClient(configuration.GetValue<string>("Smtp:Host"))
            {
                UseDefaultCredentials = false,
                EnableSsl = configuration.GetValue<bool>("Smtp:Ssl"),
                Port = configuration.GetValue<int>("Smtp:Port"),
                Credentials = new System.Net.NetworkCredential(configuration.GetValue<string>("Smtp:User"), configuration.GetValue<string>("Smtp:Password"))
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}
