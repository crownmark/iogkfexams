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
    public partial class EditNotificationTemplate
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
        public int NotificationTemplateId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            notificationTemplate = await IOGKFExamsDbService.GetNotificationTemplateByNotificationTemplateId(notificationTemplateId:NotificationTemplateId);
        }
        protected bool errorVisible;
        protected IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate notificationTemplate;

        protected IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.Language> languagesForLanguageId;


        protected int languagesForLanguageIdCount;
        protected IOGKFExams.Server.Models.IOGKFExamsDb.Language languagesForLanguageIdValue;

        [Inject]
        protected SecurityService Security { get; set; }
        protected async Task languagesForLanguageIdLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await IOGKFExamsDbService.GetLanguages(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(LanguageName, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                languagesForLanguageId = result.Value.AsODataEnumerable();
                languagesForLanguageIdCount = result.Count;

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Language" });
            }
        }
        protected async Task FormSubmit()
        {
            try
            {
                await IOGKFExamsDbService.UpdateNotificationTemplate(notificationTemplateId:NotificationTemplateId, notificationTemplate);
                DialogService.Close(notificationTemplate);
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

        protected async System.Threading.Tasks.Task TokensChange(string args)
        {
            notificationTemplate.MessageBody = notificationTemplate.MessageBody + " " + args;
        }
        protected async System.Threading.Tasks.Task TokensChangeSubject(string args)
        {
            notificationTemplate.Subject = notificationTemplate.Subject + " " + args;
        }
    }
}