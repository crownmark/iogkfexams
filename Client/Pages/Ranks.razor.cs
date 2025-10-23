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
    public partial class Ranks
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

        protected IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> ranks;

        protected RadzenDataGrid<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> grid0;
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
                gridLoading = true;

                var result = await IOGKFExamsDbService.GetRanks(filter: $@"(contains(RankName,""{search}"")) and {(string.IsNullOrEmpty(args.Filter)? "true" : args.Filter)}", orderby: $"{args.OrderBy}", top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null);
                ranks = result.Value.AsODataEnumerable();
                count = result.Count;
                gridLoading = false;

            }
            catch (System.Exception ex)
            {
                gridLoading = false;

                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Ranks" });
            }
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddRank>("Add Rank", null);
            await grid0.Reload();
        }

        protected async Task EditRow(IOGKFExams.Server.Models.IOGKFExamsDb.Rank args)
        {
            await DialogService.OpenAsync<EditRank>("Edit Rank", new Dictionary<string, object> { {"RankId", args.RankId} });
            await grid0.Reload();
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, IOGKFExams.Server.Models.IOGKFExamsDb.Rank rank)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await IOGKFExamsDbService.DeleteRank(rankId:rank.RankId);

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
                    Detail = $"Unable to delete Rank"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await IOGKFExamsDbService.ExportRanksToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Ranks");
            }

            if (args == null || args.Value == "xlsx")
            {
                await IOGKFExamsDbService.ExportRanksToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Ranks");
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