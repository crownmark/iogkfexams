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
    public partial class NotificationTemplates
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

        protected IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate> notificationTemplates;

        protected RadzenDataGrid<IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate> grid0;
        protected int count;

        protected string search = "";

        [Inject]
        protected SecurityService Security { get; set; }

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
                var result = await IOGKFExamsDbService.GetNotificationTemplates(filter: $@"(contains(Subject,""{search}"") or contains(MessageBody,""{search}"") or contains(Channel,""{search}"")) and {(string.IsNullOrEmpty(args.Filter)? "true" : args.Filter)}", expand: "Language", orderby: $"{args.OrderBy}", top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null);
                notificationTemplates = result.Value.AsODataEnumerable();
                count = result.Count;
            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load NotificationTemplates" });
            }
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddNotificationTemplate>("Add NotificationTemplate", options: new DialogOptions { Resizable = false, Draggable = true });
            await grid0.Reload();
        }

        protected async Task EditRow(IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate args)
        {
            await DialogService.OpenAsync<EditNotificationTemplate>("Edit NotificationTemplate", new Dictionary<string, object> { {"NotificationTemplateId", args.NotificationTemplateId} }, new DialogOptions { Resizable = false, Draggable = true });
            await grid0.Reload();
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, IOGKFExams.Server.Models.IOGKFExamsDb.NotificationTemplate notificationTemplate)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await IOGKFExamsDbService.DeleteNotificationTemplate(notificationTemplateId:notificationTemplate.NotificationTemplateId);

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
                    Detail = $"Unable to delete NotificationTemplate"
                });
            }
        }
    }
}