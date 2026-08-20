using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using IOGKFExams.Server.Helpers;

namespace IOGKFExams.Client.Pages
{
    public partial class Exams
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
        protected BatchFunctionsService BatchFunctionsService { get; set; }

        [Inject]
        public IOGKFExamsDbService IOGKFExamsDbService { get; set; }

        [Inject]
        public SmsService SmsService { get; set; }

        protected IEnumerable<IOGKFExams.Server.Models.IOGKFExamsDb.Exam> exams;

        protected RadzenDataGrid<IOGKFExams.Server.Models.IOGKFExamsDb.Exam> grid0;
        protected int count;

        protected bool gridLoading {  get; set; }

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
                var result = await IOGKFExamsDbService.GetExams(filter: $@"{(string.IsNullOrEmpty(args.Filter)? "true" : args.Filter)}", expand: "ExamStatus,Country", orderby: $"{args.OrderBy}", top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null);
                exams = result.Value.AsODataEnumerable();
                count = result.Count;
                gridLoading = false;

            }
            catch (System.Exception ex)
            {
                gridLoading = false;

                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Exams" });
            }
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddExam>("Create Exam", null);
            await grid0.Reload();
        }

        protected async Task EditRow(IOGKFExams.Server.Models.IOGKFExamsDb.Exam args)
        {
            await DialogService.OpenAsync<EditExam>("Edit Exam", new Dictionary<string, object> { {"ExamId", args.ExamId} });
            await grid0.Reload();
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, IOGKFExams.Server.Models.IOGKFExamsDb.Exam exam)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
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
                                    columnContent.AddContent(0, "Deleting exam.  This will take a moment.  Please wait...");
                                }));
                                rowContent.CloseComponent();
                            }));

                            dialogContent.CloseComponent();
                        };
                        return content;
                    }, new DialogOptions() { ShowTitle = false, Style = "min-height:auto;min-width:auto;width:auto", CloseDialogOnEsc = false });
                    var examQuestions = await IOGKFExamsDbService.GetExamQuestions(filter: $@"ExamId eq {exam.ExamId}");
                    foreach(var examQuestion in examQuestions.Value.ToList())
                    {
                        var examAnswers = await IOGKFExamsDbService.GetExamAnswers(filter: $@"ExamQuestionsId eq {examQuestion.ExamQuestionsId}");
                        foreach(var examAnswer in examAnswers.Value.ToList())
                        {
                            var deleteAnswerResult = await IOGKFExamsDbService.DeleteExamAnswer(examAnswer.ExamAnswerId);
                        }
                        var deleteQuestionResult = await IOGKFExamsDbService.DeleteExamQuestion(examQuestion.ExamQuestionsId);
                    }
                    var deleteResult = await IOGKFExamsDbService.DeleteExam(examId:exam.ExamId);
                    DialogService.Close();
                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                DialogService.Close();

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Exam. Error: {ex.Message}"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await IOGKFExamsDbService.ExportExamsToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "ExamStatus",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Exams");
            }

            if (args == null || args.Value == "xlsx")
            {
                await IOGKFExamsDbService.ExportExamsToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "ExamStatus",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Exams");
            }
        }

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

        protected async System.Threading.Tasks.Task SendExamSMS(Microsoft.AspNetCore.Components.Web.MouseEventArgs args, IOGKFExams.Server.Models.IOGKFExamsDb.Exam exam)
        {
            try
            {
                // Send Email Message to Student
                if (!string.IsNullOrEmpty(exam.StudentMobilePhoneE164))
                {
                    try
                    {
                        var notificationTemplates = await IOGKFExamsDbService.GetNotificationTemplates(filter: $"LanguageId eq {exam.LanguageId} and Active eq true and Channel eq 'SMS'");
                        var notificationTemplate = notificationTemplates.Value.FirstOrDefault();
                        if (notificationTemplate != null)
                        {

                            var tokens = new Dictionary<string, string>
                                {
                                    { "StudentFirstName", exam.StudentFirstName },
                                    { "StudentLastName", exam.StudentLastName },
                                    { "ExamId", exam.ExamId.ToString() },
                                    { "ExamSessionCode", exam.ExamSessionCode.ToString() },
                                    { "ExamUrl", $"https://demoapp.crown.software/take-exam/{exam.ExamGuid}" }
                                };
                            var updatedNotificationTemplate = TokenReplacementHelper.ReplaceTokens(notificationTemplate, tokens);
                            // Send Email Logic Here    
                            var result = await SmsService.SendSms(exam.StudentMobilePhoneE164, updatedNotificationTemplate.MessageBody);
                            if (result.Success)
                            {
                                NotificationService.Notify(new NotificationMessage
                                {
                                    Severity = NotificationSeverity.Success,
                                    Summary = $"Success",
                                    Detail = $"Exam SMS sent successfully"
                                });
                            }
                            else
                            {
                                NotificationService.Notify(new NotificationMessage
                                {
                                    Severity = NotificationSeverity.Error,
                                    Summary = $"Error",
                                    Detail = $"Unable to send Exam SMS.  Error: {result.ErrorMessage}"
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        NotificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = $"Error",
                            Detail = $"Unable to send Exam.  Error: {ex.Message}"
                        });
                    }

                }
               
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to send Exam"
                });
            }
        }

        protected async System.Threading.Tasks.Task SendExamEmail(Microsoft.AspNetCore.Components.Web.MouseEventArgs args, IOGKFExams.Server.Models.IOGKFExamsDb.Exam exam)
        {
            try
            {
                var result = await BatchFunctionsService.SendExamEmail(exam.ExamId);
                if (result.IsSuccessStatusCode)
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = $"Success",
                        Detail = $"Exam sent successfully"
                    });
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = $"Error",
                        Detail = $"{result.ReasonPhrase}"
                    });
                }
                
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to send Exam"
                });
            }
        }

        protected async System.Threading.Tasks.Task SendSMSMouseEnter(Microsoft.AspNetCore.Components.ElementReference args)
        {
            TooltipService.Open(args, "Send Exam SMS to Member");
        }

        protected async System.Threading.Tasks.Task SendSMSMouseLeave(Microsoft.AspNetCore.Components.ElementReference args)
        {
            TooltipService.Close();
        }

        protected async System.Threading.Tasks.Task CreateStudentPDFMouseEnter(Microsoft.AspNetCore.Components.ElementReference args)
        {
            TooltipService.Open(args, "Create Student Exam PDF");
        }

        protected async System.Threading.Tasks.Task CreateStudentPDFMouseLeave(Microsoft.AspNetCore.Components.ElementReference args)
        {
            TooltipService.Close();
        }

        protected async System.Threading.Tasks.Task CreateAnswerKeyPDFMouseEnter(Microsoft.AspNetCore.Components.ElementReference args)
        {
            TooltipService.Open(args, "Create Exam Answer Key PDF");
        }

        protected async System.Threading.Tasks.Task CreateAnswerKeyPDFMouseLeave(Microsoft.AspNetCore.Components.ElementReference args)
        {
            TooltipService.Close();
        }

        protected async System.Threading.Tasks.Task SendEmailMouseEnter(Microsoft.AspNetCore.Components.ElementReference args)
        {
            TooltipService.Open(args, "Send Exam Email to Member");
        }

        protected async System.Threading.Tasks.Task SendEmailMouseLeave(Microsoft.AspNetCore.Components.ElementReference args)
        {
            TooltipService.Close();

        }

        protected async System.Threading.Tasks.Task TakeTestButtonClick(Microsoft.AspNetCore.Components.Web.MouseEventArgs args, IOGKFExams.Server.Models.IOGKFExamsDb.Exam exam)
        {
            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", $"https://demoapp.crown.software/take-exam/{exam.ExamGuid}");
            NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Success", Detail = "Exam Url Copied to Clipboard" });
        }

        protected async System.Threading.Tasks.Task CopyExamUrlMouseEnter(Microsoft.AspNetCore.Components.ElementReference args)
        {
            TooltipService.Open(args, "Copy Exam Url", new TooltipOptions { Position = TooltipPosition.Top });

        }

        protected async System.Threading.Tasks.Task CopyExamUrlMouseLeave(Microsoft.AspNetCore.Components.ElementReference args)
        {
            TooltipService.Close();
        }

        protected async System.Threading.Tasks.Task CreateStudenPDFButtonClick(Microsoft.AspNetCore.Components.Web.MouseEventArgs args, IOGKFExams.Server.Models.IOGKFExamsDb.Exam exam)
        {
            try
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
                                columnContent.AddContent(0, "Creating Exam Pdf and Answer Key.  Please wait...");
                            }));
                            rowContent.CloseComponent();
                        }));

                        dialogContent.CloseComponent();
                    };
                    return content;
                }, new DialogOptions() { ShowTitle = false, Style = "min-height:auto;min-width:auto;width:auto", CloseDialogOnEsc = false });
                await BatchFunctionsService.GenerateExamPdf(exam.ExamGuid);
                DialogService.Close();
                await grid0.Reload();

            }
            catch (Exception ex)
            {
                DialogService.Close();
                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Error", Detail = $"{ex.Message}" });

            }
        }

        protected async System.Threading.Tasks.Task CreateAnswerKeyPdfButtonClick(Microsoft.AspNetCore.Components.Web.MouseEventArgs args, IOGKFExams.Server.Models.IOGKFExamsDb.Exam exam)
        {
            try
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
                                columnContent.AddContent(0, "Creating Exam Pdf and Answer Key.  Please wait...");
                            }));
                            rowContent.CloseComponent();
                        }));

                        dialogContent.CloseComponent();
                    };
                    return content;
                }, new DialogOptions() { ShowTitle = false, Style = "min-height:auto;min-width:auto;width:auto", CloseDialogOnEsc = false });
                await BatchFunctionsService.GenerateExamPdf(exam.ExamGuid);
                DialogService.Close();  
                await grid0.Reload();
            }
            catch (Exception ex)
            {
                DialogService.Close();
                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Error", Detail = $"{ex.Message}" });

            }
        }
    }
}