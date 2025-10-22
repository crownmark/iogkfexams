
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Web;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Radzen;

namespace IOGKFExams.Client
{
    public partial class IOGKFExamsDbService
    {
        private readonly HttpClient httpClient;
        private readonly Uri baseUri;
        private readonly NavigationManager navigationManager;

        public IOGKFExamsDbService(NavigationManager navigationManager, HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;

            this.navigationManager = navigationManager;
            this.baseUri = new Uri($"{navigationManager.BaseUri}odata/IOGKFExamsDb/");
        }


        public async System.Threading.Tasks.Task ExportCountriesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/countries/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/countries/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportCountriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/countries/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/countries/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetCountries(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.Country>> GetCountries(Query query)
        {
            return await GetCountries(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.Country>> GetCountries(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string), string apply = default(string))
        {
            var uri = new Uri(baseUri, $"Countries");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count, apply:apply);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetCountries(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.Country>>(response);
        }

        partial void OnCreateCountry(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Country> CreateCountry(IOGKFExams.Server.Models.IOGKFExamsDb.Country country = default(IOGKFExams.Server.Models.IOGKFExamsDb.Country))
        {
            var uri = new Uri(baseUri, $"Countries");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(country), Encoding.UTF8, "application/json");

            OnCreateCountry(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.Country>(response);
        }

        partial void OnDeleteCountry(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteCountry(int countryId = default(int))
        {
            var uri = new Uri(baseUri, $"Countries({countryId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteCountry(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetCountryByCountryId(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Country> GetCountryByCountryId(string expand = default(string), int countryId = default(int))
        {
            var uri = new Uri(baseUri, $"Countries({countryId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetCountryByCountryId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.Country>(response);
        }

        partial void OnUpdateCountry(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateCountry(int countryId = default(int), IOGKFExams.Server.Models.IOGKFExamsDb.Country country = default(IOGKFExams.Server.Models.IOGKFExamsDb.Country))
        {
            var uri = new Uri(baseUri, $"Countries({countryId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(country), Encoding.UTF8, "application/json");

            OnUpdateCountry(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportExamAnswersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examanswers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examanswers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportExamAnswersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examanswers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examanswers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetExamAnswers(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer>> GetExamAnswers(Query query)
        {
            return await GetExamAnswers(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer>> GetExamAnswers(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string), string apply = default(string))
        {
            var uri = new Uri(baseUri, $"ExamAnswers");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count, apply:apply);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetExamAnswers(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer>>(response);
        }

        partial void OnCreateExamAnswer(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer> CreateExamAnswer(IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer examAnswer = default(IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer))
        {
            var uri = new Uri(baseUri, $"ExamAnswers");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(examAnswer), Encoding.UTF8, "application/json");

            OnCreateExamAnswer(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer>(response);
        }

        partial void OnDeleteExamAnswer(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteExamAnswer(int examAnswerId = default(int))
        {
            var uri = new Uri(baseUri, $"ExamAnswers({examAnswerId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteExamAnswer(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetExamAnswerByExamAnswerId(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer> GetExamAnswerByExamAnswerId(string expand = default(string), int examAnswerId = default(int))
        {
            var uri = new Uri(baseUri, $"ExamAnswers({examAnswerId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetExamAnswerByExamAnswerId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer>(response);
        }

        partial void OnUpdateExamAnswer(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateExamAnswer(int examAnswerId = default(int), IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer examAnswer = default(IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer))
        {
            var uri = new Uri(baseUri, $"ExamAnswers({examAnswerId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(examAnswer), Encoding.UTF8, "application/json");

            OnUpdateExamAnswer(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportExamQuestionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examquestions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examquestions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportExamQuestionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examquestions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examquestions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetExamQuestions(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion>> GetExamQuestions(Query query)
        {
            return await GetExamQuestions(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion>> GetExamQuestions(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string), string apply = default(string))
        {
            var uri = new Uri(baseUri, $"ExamQuestions");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count, apply:apply);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetExamQuestions(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion>>(response);
        }

        partial void OnCreateExamQuestion(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion> CreateExamQuestion(IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion examQuestion = default(IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion))
        {
            var uri = new Uri(baseUri, $"ExamQuestions");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(examQuestion), Encoding.UTF8, "application/json");

            OnCreateExamQuestion(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion>(response);
        }

        partial void OnDeleteExamQuestion(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteExamQuestion(int examQuestionsId = default(int))
        {
            var uri = new Uri(baseUri, $"ExamQuestions({examQuestionsId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteExamQuestion(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetExamQuestionByExamQuestionsId(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion> GetExamQuestionByExamQuestionsId(string expand = default(string), int examQuestionsId = default(int))
        {
            var uri = new Uri(baseUri, $"ExamQuestions({examQuestionsId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetExamQuestionByExamQuestionsId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion>(response);
        }

        partial void OnUpdateExamQuestion(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateExamQuestion(int examQuestionsId = default(int), IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion examQuestion = default(IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion))
        {
            var uri = new Uri(baseUri, $"ExamQuestions({examQuestionsId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(examQuestion), Encoding.UTF8, "application/json");

            OnUpdateExamQuestion(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportExamsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/exams/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/exams/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportExamsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/exams/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/exams/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetExams(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.Exam>> GetExams(Query query)
        {
            return await GetExams(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.Exam>> GetExams(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string), string apply = default(string))
        {
            var uri = new Uri(baseUri, $"Exams");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count, apply:apply);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetExams(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.Exam>>(response);
        }

        partial void OnCreateExam(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Exam> CreateExam(IOGKFExams.Server.Models.IOGKFExamsDb.Exam exam = default(IOGKFExams.Server.Models.IOGKFExamsDb.Exam))
        {
            var uri = new Uri(baseUri, $"Exams");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(exam), Encoding.UTF8, "application/json");

            OnCreateExam(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.Exam>(response);
        }

        partial void OnDeleteExam(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteExam(int examId = default(int))
        {
            var uri = new Uri(baseUri, $"Exams({examId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteExam(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetExamByExamId(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Exam> GetExamByExamId(string expand = default(string), int examId = default(int))
        {
            var uri = new Uri(baseUri, $"Exams({examId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetExamByExamId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.Exam>(response);
        }

        partial void OnUpdateExam(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateExam(int examId = default(int), IOGKFExams.Server.Models.IOGKFExamsDb.Exam exam = default(IOGKFExams.Server.Models.IOGKFExamsDb.Exam))
        {
            var uri = new Uri(baseUri, $"Exams({examId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(exam), Encoding.UTF8, "application/json");

            OnUpdateExam(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportExamStatusesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examstatuses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examstatuses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportExamStatusesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examstatuses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examstatuses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetExamStatuses(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus>> GetExamStatuses(Query query)
        {
            return await GetExamStatuses(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus>> GetExamStatuses(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string), string apply = default(string))
        {
            var uri = new Uri(baseUri, $"ExamStatuses");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count, apply:apply);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetExamStatuses(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus>>(response);
        }

        partial void OnCreateExamStatus(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus> CreateExamStatus(IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus examStatus = default(IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus))
        {
            var uri = new Uri(baseUri, $"ExamStatuses");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(examStatus), Encoding.UTF8, "application/json");

            OnCreateExamStatus(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus>(response);
        }

        partial void OnDeleteExamStatus(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteExamStatus(int examStatusId = default(int))
        {
            var uri = new Uri(baseUri, $"ExamStatuses({examStatusId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteExamStatus(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetExamStatusByExamStatusId(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus> GetExamStatusByExamStatusId(string expand = default(string), int examStatusId = default(int))
        {
            var uri = new Uri(baseUri, $"ExamStatuses({examStatusId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetExamStatusByExamStatusId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus>(response);
        }

        partial void OnUpdateExamStatus(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateExamStatus(int examStatusId = default(int), IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus examStatus = default(IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus))
        {
            var uri = new Uri(baseUri, $"ExamStatuses({examStatusId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(examStatus), Encoding.UTF8, "application/json");

            OnUpdateExamStatus(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportExamTemplateAnswersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examtemplateanswers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examtemplateanswers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportExamTemplateAnswersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examtemplateanswers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examtemplateanswers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetExamTemplateAnswers(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer>> GetExamTemplateAnswers(Query query)
        {
            return await GetExamTemplateAnswers(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer>> GetExamTemplateAnswers(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string), string apply = default(string))
        {
            var uri = new Uri(baseUri, $"ExamTemplateAnswers");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count, apply:apply);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetExamTemplateAnswers(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer>>(response);
        }

        partial void OnCreateExamTemplateAnswer(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer> CreateExamTemplateAnswer(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer examTemplateAnswer = default(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer))
        {
            var uri = new Uri(baseUri, $"ExamTemplateAnswers");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(examTemplateAnswer), Encoding.UTF8, "application/json");

            OnCreateExamTemplateAnswer(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer>(response);
        }

        partial void OnDeleteExamTemplateAnswer(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteExamTemplateAnswer(int examTemplateAnswerId = default(int))
        {
            var uri = new Uri(baseUri, $"ExamTemplateAnswers({examTemplateAnswerId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteExamTemplateAnswer(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetExamTemplateAnswerByExamTemplateAnswerId(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer> GetExamTemplateAnswerByExamTemplateAnswerId(string expand = default(string), int examTemplateAnswerId = default(int))
        {
            var uri = new Uri(baseUri, $"ExamTemplateAnswers({examTemplateAnswerId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetExamTemplateAnswerByExamTemplateAnswerId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer>(response);
        }

        partial void OnUpdateExamTemplateAnswer(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateExamTemplateAnswer(int examTemplateAnswerId = default(int), IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer examTemplateAnswer = default(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer))
        {
            var uri = new Uri(baseUri, $"ExamTemplateAnswers({examTemplateAnswerId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(examTemplateAnswer), Encoding.UTF8, "application/json");

            OnUpdateExamTemplateAnswer(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportExamTemplateQuestionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examtemplatequestions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examtemplatequestions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportExamTemplateQuestionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examtemplatequestions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examtemplatequestions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetExamTemplateQuestions(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion>> GetExamTemplateQuestions(Query query)
        {
            return await GetExamTemplateQuestions(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion>> GetExamTemplateQuestions(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string), string apply = default(string))
        {
            var uri = new Uri(baseUri, $"ExamTemplateQuestions");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count, apply:apply);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetExamTemplateQuestions(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion>>(response);
        }

        partial void OnCreateExamTemplateQuestion(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion> CreateExamTemplateQuestion(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion examTemplateQuestion = default(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion))
        {
            var uri = new Uri(baseUri, $"ExamTemplateQuestions");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(examTemplateQuestion), Encoding.UTF8, "application/json");

            OnCreateExamTemplateQuestion(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion>(response);
        }

        partial void OnDeleteExamTemplateQuestion(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteExamTemplateQuestion(int examTemplateQuestionsId = default(int))
        {
            var uri = new Uri(baseUri, $"ExamTemplateQuestions({examTemplateQuestionsId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteExamTemplateQuestion(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetExamTemplateQuestionByExamTemplateQuestionsId(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion> GetExamTemplateQuestionByExamTemplateQuestionsId(string expand = default(string), int examTemplateQuestionsId = default(int))
        {
            var uri = new Uri(baseUri, $"ExamTemplateQuestions({examTemplateQuestionsId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetExamTemplateQuestionByExamTemplateQuestionsId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion>(response);
        }

        partial void OnUpdateExamTemplateQuestion(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateExamTemplateQuestion(int examTemplateQuestionsId = default(int), IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion examTemplateQuestion = default(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion))
        {
            var uri = new Uri(baseUri, $"ExamTemplateQuestions({examTemplateQuestionsId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(examTemplateQuestion), Encoding.UTF8, "application/json");

            OnUpdateExamTemplateQuestion(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportExamTemplatesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examtemplates/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examtemplates/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportExamTemplatesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/examtemplates/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/examtemplates/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetExamTemplates(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate>> GetExamTemplates(Query query)
        {
            return await GetExamTemplates(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate>> GetExamTemplates(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string), string apply = default(string))
        {
            var uri = new Uri(baseUri, $"ExamTemplates");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count, apply:apply);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetExamTemplates(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate>>(response);
        }

        partial void OnCreateExamTemplate(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate> CreateExamTemplate(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate examTemplate = default(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate))
        {
            var uri = new Uri(baseUri, $"ExamTemplates");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(examTemplate), Encoding.UTF8, "application/json");

            OnCreateExamTemplate(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate>(response);
        }

        partial void OnDeleteExamTemplate(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteExamTemplate(int examTemplateId = default(int))
        {
            var uri = new Uri(baseUri, $"ExamTemplates({examTemplateId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteExamTemplate(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetExamTemplateByExamTemplateId(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate> GetExamTemplateByExamTemplateId(string expand = default(string), int examTemplateId = default(int))
        {
            var uri = new Uri(baseUri, $"ExamTemplates({examTemplateId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetExamTemplateByExamTemplateId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate>(response);
        }

        partial void OnUpdateExamTemplate(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateExamTemplate(int examTemplateId = default(int), IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate examTemplate = default(IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate))
        {
            var uri = new Uri(baseUri, $"ExamTemplates({examTemplateId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(examTemplate), Encoding.UTF8, "application/json");

            OnUpdateExamTemplate(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportLanguagesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/languages/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/languages/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportLanguagesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/languages/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/languages/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetLanguages(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.Language>> GetLanguages(Query query)
        {
            return await GetLanguages(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.Language>> GetLanguages(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string), string apply = default(string))
        {
            var uri = new Uri(baseUri, $"Languages");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count, apply:apply);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetLanguages(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.Language>>(response);
        }

        partial void OnCreateLanguage(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Language> CreateLanguage(IOGKFExams.Server.Models.IOGKFExamsDb.Language language = default(IOGKFExams.Server.Models.IOGKFExamsDb.Language))
        {
            var uri = new Uri(baseUri, $"Languages");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(language), Encoding.UTF8, "application/json");

            OnCreateLanguage(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.Language>(response);
        }

        partial void OnDeleteLanguage(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteLanguage(int languageId = default(int))
        {
            var uri = new Uri(baseUri, $"Languages({languageId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteLanguage(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetLanguageByLanguageId(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Language> GetLanguageByLanguageId(string expand = default(string), int languageId = default(int))
        {
            var uri = new Uri(baseUri, $"Languages({languageId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetLanguageByLanguageId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.Language>(response);
        }

        partial void OnUpdateLanguage(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateLanguage(int languageId = default(int), IOGKFExams.Server.Models.IOGKFExamsDb.Language language = default(IOGKFExams.Server.Models.IOGKFExamsDb.Language))
        {
            var uri = new Uri(baseUri, $"Languages({languageId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(language), Encoding.UTF8, "application/json");

            OnUpdateLanguage(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportRanksToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/ranks/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/ranks/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportRanksToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/iogkfexamsdb/ranks/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/iogkfexamsdb/ranks/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetRanks(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.Rank>> GetRanks(Query query)
        {
            return await GetRanks(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.Rank>> GetRanks(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string), string apply = default(string))
        {
            var uri = new Uri(baseUri, $"Ranks");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count, apply:apply);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetRanks(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<IOGKFExams.Server.Models.IOGKFExamsDb.Rank>>(response);
        }

        partial void OnCreateRank(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> CreateRank(IOGKFExams.Server.Models.IOGKFExamsDb.Rank rank = default(IOGKFExams.Server.Models.IOGKFExamsDb.Rank))
        {
            var uri = new Uri(baseUri, $"Ranks");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(rank), Encoding.UTF8, "application/json");

            OnCreateRank(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.Rank>(response);
        }

        partial void OnDeleteRank(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteRank(int rankId = default(int))
        {
            var uri = new Uri(baseUri, $"Ranks({rankId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteRank(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetRankByRankId(HttpRequestMessage requestMessage);

        public async Task<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> GetRankByRankId(string expand = default(string), int rankId = default(int))
        {
            var uri = new Uri(baseUri, $"Ranks({rankId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetRankByRankId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<IOGKFExams.Server.Models.IOGKFExamsDb.Rank>(response);
        }

        partial void OnUpdateRank(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateRank(int rankId = default(int), IOGKFExams.Server.Models.IOGKFExamsDb.Rank rank = default(IOGKFExams.Server.Models.IOGKFExamsDb.Rank))
        {
            var uri = new Uri(baseUri, $"Ranks({rankId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(rank), Encoding.UTF8, "application/json");

            OnUpdateRank(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }
    }
}