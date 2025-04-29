using Hangfire.Common;

namespace NetAPIGrid.jobs
{
    public class TestJob
    {
        //private readonly ILogger _logger;
        //public TestJob(ILogger<TestJob> logger) => _logger = logger;

        //public void WriteLog(string logMessage)
        //{
        //    _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd hh:mm:ss tt} {logMessage}");
        //}

        public class ApiCallService
        {
            private readonly HttpClient _httpClient;
            public ApiCallService(HttpClient httpClient)
            {
                _httpClient = httpClient;
            }
            public async Task CallApiEndpointAsync()
            {
                var response = await _httpClient.GetAsync("https://localhost:7155/api/Grid/InsertLogs");
                response.EnsureSuccessStatusCode();
                // Log success or handle the response as needed
            }
        }

    }
}
