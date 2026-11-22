using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _db;

        public DocumentRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Document?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _db.Documents
                .Include(x => x.Chunks)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task<List<Document>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _db.Documents
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(
            Document document,
            CancellationToken cancellationToken = default)
        {
            await _db.Documents.AddAsync(document, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var document = await _db.Documents
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (document is null)
                return;

            _db.Documents.Remove(document);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<DocumentChunk>> GetChunksByDocumentIdAsync(
            Guid documentId,
            CancellationToken cancellationToken = default)
        {
            return await _db.DocumentChunks
                .AsNoTracking()
                .Where(x => x.DocumentId == documentId)
                .OrderBy(x => x.ChunkIndex)
                .ToListAsync(cancellationToken);
        }
    }
}
