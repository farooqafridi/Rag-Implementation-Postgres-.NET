namespace Infrastructure.AI
{
    using Application.Interfaces;
    using System.Net.Http.Json;

    public class OllamaEmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _httpClient;

        public OllamaEmbeddingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<float[]> GenerateEmbeddingAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            var request = new
            {
                model = "mxbai-embed-large:latest",
                input = text
            };

            var response = await _httpClient.PostAsJsonAsync(
                "/api/embed",
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<EmbeddingResponse>(
                cancellationToken: cancellationToken);

            return result?.Embeddings?.FirstOrDefault()
                ?? throw new InvalidOperationException(
                    "Embedding generation failed.");
        }

        private sealed class EmbeddingResponse
        {
            public float[][] Embeddings { get; set; } = [];
        }
    }
}
