namespace IOGKFExams.Client
{
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    public partial class BatchFunctionsService
    {
        private readonly HttpClient httpClient;
        private readonly Uri baseUri;
        private readonly NavigationManager navigationManager;

        public BatchFunctionsService(NavigationManager navigationManager, HttpClient httpClient)
        {
            this.httpClient = httpClient;

            this.navigationManager = navigationManager;
            this.baseUri = new Uri($"{navigationManager.BaseUri}");
        }
        public int GenerateExamSessionCode(int examId)
        {
            // Combine examId with a precise UTC timestamp for uniqueness
            string input = $"{examId}_{DateTime.UtcNow:yyyyMMddHHmmssfff}";

            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                int hashValue = BitConverter.ToInt32(hashBytes, 0);
                int positiveHash = Math.Abs(hashValue);

                // Ensure result is always a 6-digit number between 100000–999999
                return 100000 + (positiveHash % 900000);
            }
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
