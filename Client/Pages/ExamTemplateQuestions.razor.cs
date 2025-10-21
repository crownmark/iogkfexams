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
    public partial class ExamTemplateQuestions
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

        protected IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion> examTemplateQuestions;

        protected RadzenDataGrid<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion> grid0;
        protected int count;

        protected string search = "";

        [Inject]
        protected SecurityService Security { get; set; }
        [Parameter]
        public int ExamTemplateId { get; set; }

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
                var result = await IOGKFExamsDbService.GetExamTemplateQuestions(filter: $@"(contains(Question,""{search}"")) and {(string.IsNullOrEmpty(args.Filter)? "true" : args.Filter)} and ExamTemplateId eq {ExamTemplateId}", expand: "ExamTemplate,Language,Rank", orderby: $"{args.OrderBy}", top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null);
                examTemplateQuestions = result.Value.AsODataEnumerable();
                count = result.Count;
            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load ExamTemplateQuestions" });
            }
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddExamTemplateQuestion>("Add Template Question", new Dictionary<string, object>() { {"ExamTemplateId", ExamTemplateId} }, new DialogOptions { Width = "80%" });
            await grid0.Reload();
        }

        protected async Task EditRow(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion args)
        {
            await DialogService.OpenAsync<EditExamTemplateQuestion>("Edit TemplateQuestion", new Dictionary<string, object> { {"ExamTemplateQuestionsId", args.ExamTemplateQuestionsId} }, new DialogOptions { Width = "80%" });
            await grid0.Reload();
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion examTemplateQuestion)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await IOGKFExamsDbService.DeleteExamTemplateQuestion(examTemplateQuestionsId:examTemplateQuestion.ExamTemplateQuestionsId);

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
                    Detail = $"Unable to delete ExamTemplateQuestion"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await IOGKFExamsDbService.ExportExamTemplateQuestionsToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "ExamTemplate,Language,Rank",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "ExamTemplateQuestions");
            }

            if (args == null || args.Value == "xlsx")
            {
                await IOGKFExamsDbService.ExportExamTemplateQuestionsToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "ExamTemplate,Language,Rank",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "ExamTemplateQuestions");
            }
        }
    }
}