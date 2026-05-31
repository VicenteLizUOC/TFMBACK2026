using System.Text;
using System.Text.Json;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Contracts;

namespace TFGBack._03_Infrastructure
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        private const string Endpoint =
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"]!;
        }

        public async Task<string> getTaskHelp(string taskName, string taskDescription)
        {
            if (string.IsNullOrWhiteSpace(taskName) || taskName.Trim().Length < 5 ||
                string.IsNullOrWhiteSpace(taskDescription) || taskDescription.Trim().Length < 10)
                return "The title and description are not explicit enough to help you.";

            var prompt = $@"You are an assistant that helps complete tasks concisely.
You are given a task with name: ""{taskName}"" and description: ""{taskDescription}"".
If the name or description are not clear or explicit enough to understand what needs to be done, respond only with: ""The title and description are not explicit enough to help you.""
If the task is clear, explain how to approach it in 4-5 lines maximum, in a direct and practical way. Do not use lists, only plain text.";

            var body = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{Endpoint}?key={_apiKey}", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;
        }
    }
}
