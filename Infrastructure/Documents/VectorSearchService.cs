using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;

namespace Infrastructure.Documents
{
    public class VectorSearchService : IVectorSearchService
    {
        private readonly ApplicationDbContext _db;

        public VectorSearchService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<DocumentChunk>> SearchAsync(
            float[] queryEmbedding,
            int topK = 5,
            CancellationToken cancellationToken = default)
        {
            var vector = new Pgvector.Vector(queryEmbedding);

            return await _db.DocumentChunks
                .AsNoTracking()
                .Where(x => x.Document.Status == "Indexed")
                .OrderBy(x => x.Embedding.CosineDistance(vector))
                .Take(topK)
                .Include(x => x.Document)
                .ToListAsync(cancellationToken);
        }
    }
}
