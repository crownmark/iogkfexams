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
    public partial class AddExam
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
        [Inject]
        public BatchFunctionsService BatchFunctionsService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            exam = new IOGKFExams.Server.Models.IOGKFExamsDb.Exam();
        }
        protected bool errorVisible;
        protected IOGKFExams.Server.Models.IOGKFExamsDb.Exam exam;

        protected IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus> examStatusesForExamStatusId;

        protected bool sendExam { get; set; } = false;

        protected int examStatusesForExamStatusIdCount;
        protected IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus examStatusesForExamStatusIdValue;
        protected bool creatingExam { get; set; } = false;
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

        protected int examTemplatesCount;
        protected int selectedTemplate;
        protected IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate> examTemplates;

        protected IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.Country> countries;

        protected int countriesCount;

        protected IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.Language> languages;

        protected int languagesCount;

        protected IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> ranks;

        protected int ranksCount;

        protected async Task examTemplatesLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await IOGKFExamsDbService.GetExamTemplates(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(ExamTemplateTitle, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}') and Active eq true", orderby: $"{args.OrderBy}");
                examTemplates = result.Value.AsODataEnumerable();
                examTemplatesCount = result.Count;

                

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
                if (sendExam)
                {
                    DialogService.OpenAsync("", ds =>
                    {
                        RenderFragment content = dialogContent =>
                        {
                            dialogContent.OpenComponent<RadzenRow>(0);
                            dialogContent.AddComponentParameter(1, nameof(RadzenRow.ChildContent), (RenderFragment)(rowContent =>
                            {
                                rowContent.OpenComponent<RadzenColumn>(0);
                                rowContent.AddComponentParameter(1, nameof(RadzenColumn.Size), 12);
                                rowContent.AddComponentParameter(2, nameof(RadzenRow.ChildContent), (RenderFragment)(columnContent =>
                                {
                                    columnContent.AddContent(0, "Creating Exam and Sending to Student.  This will take a moment.  Please wait...");
                                }));
                                rowContent.CloseComponent();
                            }));

                            dialogContent.CloseComponent();
                        };
                        return content;
                    }, new DialogOptions() { ShowTitle = false, Style = "min-height:auto;min-width:auto;width:auto", CloseDialogOnEsc = false });
                }
                else
                {
                    creatingExam = true;
                    DialogService.OpenAsync("", ds =>
                    {
                        RenderFragment content = dialogContent =>
                        {
                            dialogContent.OpenComponent<RadzenRow>(0);
                            dialogContent.AddComponentParameter(1, nameof(RadzenRow.ChildContent), (RenderFragment)(rowContent =>
                            {
                                rowContent.OpenComponent<RadzenColumn>(0);
                                rowContent.AddComponentParameter(1, nameof(RadzenColumn.Size), 12);
                                rowContent.AddComponentParameter(2, nameof(RadzenRow.ChildContent), (RenderFragment)(columnContent =>
                                {
                                    columnContent.AddContent(0, "Creating Exam.  This will take a moment.  Please wait...");
                                }));
                                rowContent.CloseComponent();
                            }));

                            dialogContent.CloseComponent();
                        };
                        return content;
                    }, new DialogOptions() { ShowTitle = false, Style = "min-height:auto;min-width:auto;width:auto", CloseDialogOnEsc = false });
                }
                StateHasChanged();
                exam.CreatedDate = DateTimeOffset.UtcNow;
                exam.ExamStatusId = 1;
                exam.ExamGuid = Guid.NewGuid().ToString();
                var newExam = await IOGKFExamsDbService.CreateExam(exam);
                newExam.ExamSessionCode = BatchFunctionsService.GenerateExamSessionCode(newExam.ExamId);
                await IOGKFExamsDbService.UpdateExam(newExam.ExamId, newExam);

                //var template = await IOGKFExamsDbService.GetExamTemplateByExamTemplateId("ExamTemplateQuestions($expand=ExamTemplateAnswers)", selectedTemplate);
                var template = await IOGKFExamsDbService.GetExamTemplateByExamTemplateId("", selectedTemplate);
                var response = await BatchFunctionsService.CreateSingleExam(template.ExamTemplateId, newExam.ExamId, sendExam);
                if (response.IsSuccessStatusCode)
                {
                    NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Summary = $"Success", Detail = $"Exam Created" });
                    if (sendExam)
                    {
                        if (!string.IsNullOrEmpty(exam.StudentMobilePhoneE164))
                        {
                            try
                            {
                                //Send SMS MEssage
                                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Summary = $"Success", Detail = $"Exam Sent To Member Phone" });

                            }
                            catch (Exception ex)
                            {
                                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Error Sending Exam to Member Phone" });

                            }
                        }
                        if (!string.IsNullOrEmpty(exam.StudentEmail))
                        {
                            try
                            {
                                //Send Email Message
                                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Summary = $"Success", Detail = $"Exam Sent to Member Email" });

                            }
                            catch (Exception ex)
                            {
                                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Error Sending Exam to Member Email" });

                            }
                        }
                    }
                    creatingExam = false;
                    sendExam = false;
                    DialogService.Close();
                    
                    DialogService.Close(exam);

                }
                else
                {
                    DialogService.Close();
                    creatingExam = false;
                    sendExam = false;
                    errorVisible = true;
                }
            }
            catch (Exception ex)
            {
                DialogService.Close();
                errorVisible = true;
                creatingExam = false;
                sendExam = false;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }


        protected async Task countriesLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await IOGKFExamsDbService.GetCountries(new Query { Top = args.Top, Skip = args.Skip, Filter = args.Filter, OrderBy = args.OrderBy });

                countries = result.Value.AsODataEnumerable();
                countriesCount = result.Count;
            }
            catch (Exception)
            {
                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = "Unable to load" });
            }
        }

        protected async System.Threading.Tasks.Task ValidatePhone(System.String args)
        {
        }

        protected async System.Threading.Tasks.Task CreateAndSendButtonClick(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            sendExam = true;
            FormSubmit();
        }


        protected async Task languagesLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await IOGKFExamsDbService.GetLanguages(new Query { Top = args.Top, Skip = args.Skip, Filter = $"contains(LanguageName, \"{(!string.IsNullOrEmpty(args.Filter) ? args.Filter: "")}\")", OrderBy = args.OrderBy });

                languages = result.Value.AsODataEnumerable();
                languagesCount = result.Count;
            }
            catch (Exception)
            {
                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = "Unable to load" });
            }
        }


        protected async Task ranksLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await IOGKFExamsDbService.GetRanks(new Query { Top = args.Top, Skip = args.Skip, Filter = $"contains(RankName, \"{(!string.IsNullOrEmpty(args.Filter) ? args.Filter: "")}\")", OrderBy = args.OrderBy });

                ranks = result.Value.AsODataEnumerable();
                ranksCount = result.Count;
            }
            catch (Exception)
            {
                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = "Unable to load" });
            }
        }
    }
}