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
    public partial class EditLanguage
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
        public int LanguageId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            language = await IOGKFExamsDbService.GetLanguageByLanguageId(languageId:LanguageId);
        }
        protected bool errorVisible;
        protected IOGKFExams.Server.Models.IOGKFExamsDb.Language language;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await IOGKFExamsDbService.UpdateLanguage(languageId:LanguageId, language);
                DialogService.Close(language);
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