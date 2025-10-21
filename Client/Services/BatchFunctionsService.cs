namespace IOGKFExams.Client.Services
{
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text;
    using System.Threading.Tasks;
    using IOGKFExams.Server.Models;
    using IOGKFExams.Server.Models.IOGKFExamsDb;
    using Microsoft.AspNetCore.Components;
    public class BatchFunctionsService
    {
        private readonly HttpClient httpClient;
        private readonly Uri baseUri;
        private readonly NavigationManager navigationManager;

        public BatchFunctionsService(NavigationManager navigationManager, HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;

            this.navigationManager = navigationManager;
            this.baseUri = new Uri($"{navigationManager.BaseUri}");
        }
        public async Task<HttpResponseMessage> CreateSingleExam(int templateId, int examId)
        {
            var uri = new Uri(baseUri, $"BatchFunctions/createsingleexam?templateId={templateId}&examId={examId}");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return response;
        }
    }
}
