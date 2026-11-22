
using Application.Interfaces;
using System.Net.Http.Json;

namespace Infrastructure.AI
{
    public class OllamaChatService : IChatService
    {
        private readonly HttpClient _httpClient;

        public OllamaChatService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GenerateAnswerAsync(
            string systemPrompt,
            string userPrompt,
            CancellationToken cancellationToken = default)
        {
            var request = new
            {
                model = "llama3.2",
                stream = false,
                messages = new[]
                {
                new
                {
                    role = "system",
                    content = systemPrompt
                },
                new
                {
                    role = "user",
                    content = userPrompt
                }
            }
            };

            var response = await _httpClient.PostAsJsonAsync(
                "/api/chat",
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ChatResponse>(
                cancellationToken: cancellationToken);

            return result?.Message?.Content
                ?? throw new InvalidOperationException(
                    "Chat response was empty.");
        }

        private sealed class ChatResponse
        {
            public ChatMessage? Message { get; set; }
        }

        private sealed class ChatMessage
        {
            public string? Content { get; set; }
        }
    }
}
