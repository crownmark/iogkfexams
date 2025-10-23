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
    public partial class ExamResults
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
        protected IOGKFExamsDbService IOGKFExamsDbService { get; set; }

        [Parameter]
        public string ExamGUID { get; set; }

        protected Server.Models.IOGKFExamsDb.Exam exam { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await IOGKFExamsDbService.GetExams(expand: "", filter: $"ExamGuid eq '{ExamGUID}'", orderby: null, top: 1, skip: null, count: null);
                exam = result.Value.FirstOrDefault();

            }
            catch (Exception ex)
            {

                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to get Exam Results" });

            }
        }
    }
}