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
    public partial class EditExam
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

        [Parameter]
        public int ExamId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            exam = await IOGKFExamsDbService.GetExamByExamId(examId:ExamId);
        }
        protected bool errorVisible;
        protected IOGKFExams.Server.Models.IOGKFExamsDb.Exam exam;

        protected IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus> examStatusesForExamStatusId;


        protected int examStatusesForExamStatusIdCount;
        protected IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus examStatusesForExamStatusIdValue;

        [Inject]
        protected SecurityService Security { get; set; }
        protected async Task examStatusesForExamStatusIdLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await IOGKFExamsDbService.GetExamStatuses(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(Title, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                examStatusesForExamStatusId = result.Value.AsODataEnumerable();
                examStatusesForExamStatusIdCount = result.Count;

                if (!object.Equals(exam.ExamStatusId, null))
                {
                    var valueResult = await IOGKFExamsDbService.GetExamStatuses(filter: $"ExamStatusId eq {exam.ExamStatusId}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        examStatusesForExamStatusIdValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load ExamStatus" });
            }
        }
        protected async Task FormSubmit()
        {
            try
            {
                await IOGKFExamsDbService.UpdateExam(examId:ExamId, exam);
                DialogService.Close(exam);
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