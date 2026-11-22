using Domain.Entities;

namespace Application.Interfaces
{
    public interface IVectorSearchService
    {
        Task<List<DocumentChunk>> SearchAsync(
            float[] queryEmbedding,
            int topK = 5,
            CancellationToken cancellationToken = default);
    }
}
