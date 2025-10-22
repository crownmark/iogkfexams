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

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await IOGKFExamsDbService.GetExams(expand: "ExamQuestions($expand=ExamAnswers)", filter: $"ExamGuid eq '{ExamGUID}'", orderby: null, top: 1, skip: null, count: null);
                exam = result.Value.FirstOrDefault();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Exam" });

            }
            examAnswers = (await IOGKFExamsDbService.GetExamAnswers()).Value;
        }

        protected async System.Threading.Tasks.Task AnswerCheckBoxChange(bool args, Server.Models.IOGKFExamsDb.ExamAnswer answer)
        {
            try
            {
                await IOGKFExamsDbService.UpdateExamAnswer(answer.ExamAnswerId, answer);
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to save answer" });

            }
        }
    }
}