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
    public partial class ExamTemplateAnswers
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

        protected IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer> examTemplateAnswers;

        protected RadzenDataGrid<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer> grid0;
        protected int count;

        protected string search = "";

        [Inject]
        protected SecurityService Security { get; set; }
        [Parameter]
        public int ExamTemplateQuestionId { get; set; }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            await grid0.Reload();
        }

        protected async Task Grid0LoadData(LoadDataArgs args)
        {
            try
            {
                gridLoading = true;

                var result = await IOGKFExamsDbService.GetExamTemplateAnswers(filter: $@"(contains(ExamTemplateAnswer1,""{search}"")) and {(string.IsNullOrEmpty(args.Filter)? "true" : args.Filter)} and ExamTemplateQuestionsId eq {ExamTemplateQuestionId}", expand: "ExamTemplateQuestion", orderby: $"{args.OrderBy}", top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null);
                examTemplateAnswers = result.Value.AsODataEnumerable();
                count = result.Count;
                gridLoading = false;

            }
            catch (System.Exception ex)
            {
                gridLoading = false;

                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load ExamTemplateAnswers {ex.Message}" });
            }
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddExamTemplateAnswer>("Add ExamTemplateAnswer", new Dictionary<string, object>() { {"ExamTemplateQuestionsId", ExamTemplateQuestionId} });
            await grid0.Reload();
        }

        protected async Task EditRow(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer args)
        {
            await DialogService.OpenAsync<EditExamTemplateAnswer>("Edit ExamTemplateAnswer", new Dictionary<string, object> { {"ExamTemplateAnswerId", args.ExamTemplateAnswerId} }, new DialogOptions { Width = "80%" });
            await grid0.Reload();
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer examTemplateAnswer)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await IOGKFExamsDbService.DeleteExamTemplateAnswer(examTemplateAnswerId:examTemplateAnswer.ExamTemplateAnswerId);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete ExamTemplateAnswer"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await IOGKFExamsDbService.ExportExamTemplateAnswersToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "ExamTemplateQuestion",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "ExamTemplateAnswers");
            }

            if (args == null || args.Value == "xlsx")
            {
                await IOGKFExamsDbService.ExportExamTemplateAnswersToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "ExamTemplateQuestion",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "ExamTemplateAnswers");
            }
        }

        protected bool gridLoading { get; set; }

        protected async System.Threading.Tasks.Task RefreshGridButtonClick(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            await grid0.Reload();
        }

        protected async System.Threading.Tasks.Task RefreshGridButtonMouseEnter(Microsoft.AspNetCore.Components.ElementReference args)
        {
            TooltipService.Open(args, "Refresh Data", new TooltipOptions { Position = TooltipPosition.Top });
        }

        protected async System.Threading.Tasks.Task RefreshGridButtonMouseLeave(Microsoft.AspNetCore.Components.ElementReference args)
        {
            TooltipService.Close();
        }
    }
}