using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Pgvector;


namespace Infrastructure.Documents
{
   
    public class DocumentIngestionService1
    {
        private readonly ApplicationDbContext _db;
        private readonly IDocumentTextExtractor _extractor;
        private readonly IEmbeddingService _embedding;
        private readonly TextChunker _chunker;

        public DocumentIngestionService1(
            ApplicationDbContext db,
            IDocumentTextExtractor extractor,
            IEmbeddingService embedding,
            TextChunker chunker)
        {
            _db = db;
            _extractor = extractor;
            _embedding = embedding;
            _chunker = chunker;
        }

        public async Task<Guid> ProcessAsync(
        Stream file,
        string fileName,
        string storagePath,
        CancellationToken cancellationToken = default)
        {
            var document = new Document
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                StoragePath = storagePath,
                Status = "Processing"
            };

            try
            {
                var extractedText = await _extractor.ExtractTextAsync(
                    file,
                    fileName,
                    cancellationToken);

                if (string.IsNullOrWhiteSpace(extractedText))
                    throw new InvalidOperationException(
                        "No readable text found in document.");

                document.ExtractedText = extractedText;

                var chunks = _chunker.Split(extractedText);

                for (int i = 0; i < chunks.Count; i++)
                {
                    var vector = await _embedding.GenerateEmbeddingAsync(chunks[i], cancellationToken);

                    if (vector.Length != 1024)
                    {
                        throw new InvalidOperationException(
                            $"Unexpected embedding dimensions. " +
                            $"Expected 1024 but received {vector.Length}.");
                    }

                    document.Chunks.Add(new DocumentChunk
                    {
                        Id = Guid.NewGuid(),
                        DocumentId = document.Id,
                        Content = chunks[i],
                        ChunkIndex = i,
                        //Embedding = new Pgvector.Vector(vector)
                        Embedding = vector
                    });
                }

                document.Status = "Indexed";

                _db.Documents.Add(document);

                await _db.SaveChangesAsync(cancellationToken);

                return document.Id;
            }
            catch
            {
                // No database records have been saved yet.
                throw;
            }
        }
    }
}
