using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace IOGKFExams.Client.Pages
{
    public partial class TakeExam
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        public IOGKFExamsDbService IOGKFExamsDbService { get; set; }

        [Parameter]
        public string ExamGUID { get; set; }

        protected IOGKFExams.Server.Models.IOGKFExamsDb.Exam exam;

        protected IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer> examAnswers;

        protected bool allowSave { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await IOGKFExamsDbService.GetExams(expand: "ExamQuestions($expand=ExamAnswers)", filter: $"ExamGuid eq '{ExamGUID}'", orderby: null, top: 1, skip: null, count: null);
                exam = result.Value.FirstOrDefault();
                if (exam != null && exam.ExamStatusId != 3)
                {
                    if (AllowCompletingExam())
                    {
                        allowSave = true;
                    }
                    else
                    {
                        allowSave = false;

                    }
                }
                else
                {
                    NavigationManager.NavigateTo($"/exam-results/{exam.ExamGuid}");

                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Exam" });

            }
            examAnswers = (await IOGKFExamsDbService.GetExamAnswers()).Value;
        }

        protected async System.Threading.Tasks.Task AnswerCheckBoxChange(bool args, Server.Models.IOGKFExamsDb.ExamAnswer answer, ICollection<Server.Models.IOGKFExamsDb.ExamAnswer> answers)
        {
            try
            {
                foreach (var item in answers)
                {
                    if(item.ExamAnswerId == answer.ExamAnswerId)
                    {
                        await IOGKFExamsDbService.UpdateExamAnswer(answer.ExamAnswerId, answer);

                    }
                    else
                    {
                        item.IsSelectedAnswer = false;
                        await IOGKFExamsDbService.UpdateExamAnswer(item.ExamAnswerId, item);

                    }
                }
                if (AllowCompletingExam())
                {
                    allowSave = true;
                }
                else
                {
                    allowSave = false;

                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to save answer" });

            }
        }

        protected async System.Threading.Tasks.Task SubmitExamClick(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            try
            {
                if (AllowCompletingExam())
                {
                    exam.CompletedDate = DateTimeOffset.UtcNow;
                    exam.ExamStatusId = 3;
                    exam.ExamGrade = GradeExam();
                    await IOGKFExamsDbService.UpdateExam(exam.ExamId, exam);
                    NavigationManager.NavigateTo($"/exam-results/{exam.ExamGuid}");
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Warning, Summary = $"Incomplete", Detail = $"Unable to complete Exam.  All questions must be answered." });

                }


            }
            catch (Exception ex)
            {
                exam.CompletedDate = null;
                exam.ExamStatusId = 2;
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to complete Exam" });

            }
        }

        protected bool AllowCompletingExam()
        {
            try
            {
                int countOfIncompleteQuestions = 0;
                foreach (var question in exam.ExamQuestions) 
                {
                    if(question.ExamAnswers.Where(x => x.IsSelectedAnswer == true).Any())
                    {

                    }
                    else
                    {
                        countOfIncompleteQuestions++;
                    }
                }
                if (countOfIncompleteQuestions > 0)
                {
                    return false;

                }
                else
                {
                    return true;

                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        protected decimal GradeExam()
        {
            try
            {
                var totalQuestions = exam.ExamQuestions.Count();
                int correctAnswers = 0;
                foreach(var question in exam.ExamQuestions)
                {
                    if(question.ExamAnswers.Where(x => x.IsSelectedAnswer == true && x.IsCorrectAnswer == true).Any())
                    {
                        correctAnswers++;
                    }
                }
                return (decimal)correctAnswers / totalQuestions * 100;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}