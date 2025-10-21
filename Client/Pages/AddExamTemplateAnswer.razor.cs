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
    public partial class AddExamTemplateAnswer
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
        public IOGKFExamsDbService IOGKFExamsDbService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            examTemplateAnswer = new IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer();
            examTemplateAnswer.ExamTemplateQuestionsId = ExamTemplateQuestionsId;
        }
        protected bool errorVisible;
        protected IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer examTemplateAnswer;

        protected IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion> examTemplateQuestionsForExamTemplateQuestionsId;


        protected int examTemplateQuestionsForExamTemplateQuestionsIdCount;
        protected IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion examTemplateQuestionsForExamTemplateQuestionsIdValue;

        [Inject]
        protected SecurityService Security { get; set; }
        [Parameter]
        public int ExamTemplateQuestionsId { get; set; }
        protected async Task examTemplateQuestionsForExamTemplateQuestionsIdLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await IOGKFExamsDbService.GetExamTemplateQuestions(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(Question, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                examTemplateQuestionsForExamTemplateQuestionsId = result.Value.AsODataEnumerable();
                examTemplateQuestionsForExamTemplateQuestionsIdCount = result.Count;

                if (!object.Equals(examTemplateAnswer.ExamTemplateQuestionsId, null))
                {
                    var valueResult = await IOGKFExamsDbService.GetExamTemplateQuestions(filter: $"ExamTemplateQuestionsId eq {examTemplateAnswer.ExamTemplateQuestionsId}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        examTemplateQuestionsForExamTemplateQuestionsIdValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load ExamTemplateQuestion" });
            }
        }
        protected async Task FormSubmit()
        {
            try
            {
                await IOGKFExamsDbService.CreateExamTemplateAnswer(examTemplateAnswer);
                DialogService.Close(examTemplateAnswer);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}