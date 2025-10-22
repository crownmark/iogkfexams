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
    public partial class ExamLogin
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

        [Inject]
        public BatchFunctionsService BatchFunctionsService { get; set; }

        protected string error;
        protected string info;
        protected bool errorVisible;
        protected bool infoVisible;

        protected async System.Threading.Tasks.Task TakeExam(Radzen.LoginArgs args)
        {
            try
            {
                var exam = await IOGKFExamsDbService.GetExams(filter: $"ExamId eq {args.Username} and ExamSessionCode eq {args.Password}");
                if (!exam.Value.Any())
                {
                    error = $"Unable to find an exam with those credentials";
                    errorVisible = true;
                }
                else
                {
                    var myExam = exam.Value.First();
                    NavigationManager.NavigateTo($"take-exam/{exam.Value.First().ExamGuid}");
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                errorVisible = true;
            }
        }
    }
}