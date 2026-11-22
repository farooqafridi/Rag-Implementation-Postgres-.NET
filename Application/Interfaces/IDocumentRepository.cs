using Domain.Entities;

namespace Application.Interfaces
{
    public interface IDocumentRepository
    {
        Task<Document?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<List<Document>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Document document,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<List<DocumentChunk>> GetChunksByDocumentIdAsync(
            Guid documentId,
            CancellationToken cancellationToken = default);
    }
}
