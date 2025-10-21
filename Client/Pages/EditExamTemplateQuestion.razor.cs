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
    public partial class EditExamTemplateQuestion
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
        public int ExamTemplateQuestionsId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            examTemplateQuestion = await IOGKFExamsDbService.GetExamTemplateQuestionByExamTemplateQuestionsId(examTemplateQuestionsId:ExamTemplateQuestionsId);
        }
        protected bool errorVisible;
        protected IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion examTemplateQuestion;

        protected IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate> examTemplatesForExamTemplateId;

        protected IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.Language> languagesForLanguageId;

        protected IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> ranksForMinimumRankRequiredId;


        protected int examTemplatesForExamTemplateIdCount;
        protected IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate examTemplatesForExamTemplateIdValue;
        protected async Task examTemplatesForExamTemplateIdLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await IOGKFExamsDbService.GetExamTemplates(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(ExamTemplateTitle, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                examTemplatesForExamTemplateId = result.Value.AsODataEnumerable();
                examTemplatesForExamTemplateIdCount = result.Count;

                if (!object.Equals(examTemplateQuestion.ExamTemplateId, null))
                {
                    var valueResult = await IOGKFExamsDbService.GetExamTemplates(filter: $"ExamTemplateId eq {examTemplateQuestion.ExamTemplateId}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        examTemplatesForExamTemplateIdValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load ExamTemplate" });
            }
        }

        protected int languagesForLanguageIdCount;
        protected IOGKFExams.Server.Models.IOGKFExamsDb.Language languagesForLanguageIdValue;
        protected async Task languagesForLanguageIdLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await IOGKFExamsDbService.GetLanguages(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(LanguageName, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                languagesForLanguageId = result.Value.AsODataEnumerable();
                languagesForLanguageIdCount = result.Count;

                if (!object.Equals(examTemplateQuestion.LanguageId, null))
                {
                    var valueResult = await IOGKFExamsDbService.GetLanguages(filter: $"LanguageId eq {examTemplateQuestion.LanguageId}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        languagesForLanguageIdValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Language" });
            }
        }

        protected int ranksForMinimumRankRequiredIdCount;
        protected IOGKFExams.Server.Models.IOGKFExamsDb.Rank ranksForMinimumRankRequiredIdValue;

        [Inject]
        protected SecurityService Security { get; set; }
        protected async Task ranksForMinimumRankRequiredIdLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await IOGKFExamsDbService.GetRanks(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(RankName, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                ranksForMinimumRankRequiredId = result.Value.AsODataEnumerable();
                ranksForMinimumRankRequiredIdCount = result.Count;

                if (!object.Equals(examTemplateQuestion.MinimumRankRequiredId, null))
                {
                    var valueResult = await IOGKFExamsDbService.GetRanks(filter: $"RankId eq {examTemplateQuestion.MinimumRankRequiredId}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        ranksForMinimumRankRequiredIdValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Rank" });
            }
        }
        protected async Task FormSubmit()
        {
            try
            {
                await IOGKFExamsDbService.UpdateExamTemplateQuestion(examTemplateQuestionsId:ExamTemplateQuestionsId, examTemplateQuestion);
                DialogService.Close(examTemplateQuestion);
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