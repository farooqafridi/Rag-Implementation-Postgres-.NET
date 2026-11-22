using Application.Interfaces;
using Domain.Entities;
using System.Diagnostics.Metrics;

namespace Infrastructure.Documents
{
    public class DocumentIngestionService2
    {
        private readonly IDocumentTextExtractor _textExtractor;
        private readonly IEmbeddingService _embeddingService;
        private readonly IDocumentRepository _documentRepository;
        private readonly TextChunker _chunker;

        public DocumentIngestionService2(
            IDocumentTextExtractor textExtractor,
            IEmbeddingService embeddingService,
            IDocumentRepository documentRepository, TextChunker chunker)
        {
            _textExtractor = textExtractor;
            _embeddingService = embeddingService;
            _documentRepository = documentRepository;
            _chunker = chunker;
        }

        public async Task<Guid> ProcessAsync(
            Stream fileStream,
            string fileName,
            string storagePath,
            CancellationToken cancellationToken = default)
        {
            // 1. Extract text from the document
            var extractedText = await _textExtractor.ExtractTextAsync(
                fileStream,
                fileName,
                cancellationToken);

            if (string.IsNullOrWhiteSpace(extractedText))
                throw new InvalidOperationException(
                    "No text could be extracted from the document.");

            // 2. Split extracted text into chunks
            var chunks = _chunker.Split(extractedText);

            if (chunks.Count == 0)
                throw new InvalidOperationException(
                    "No document chunks were generated.");

            // 3. Create document entity
            var document = new Document
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                StoragePath = storagePath,
                ExtractedText = extractedText,
                Status = "Indexed",
                CreatedAt = DateTime.UtcNow
            };

            // 4. Generate embeddings and create chunk entities
            for (var i = 0; i < chunks.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var chunkText = chunks[i];

                var vector = await _embeddingService.GenerateEmbeddingAsync(
                    chunkText,
                    cancellationToken);

                // mxbai-embed-large produces 1024 dimensions
                if (vector.Length != 1024)
                {
                    throw new InvalidOperationException(
                        $"Expected 1024 embedding dimensions, " +
                        $"but received {vector.Length}.");
                }

                document.Chunks.Add(new DocumentChunk
                {
                    Id = Guid.NewGuid(),
                    DocumentId = document.Id,
                    Content = chunkText,
                    ChunkIndex = i,
                    // Embedding = new Pgvector.Vector(vector),
                    Embedding = vector,
                    CreatedAt = DateTime.UtcNow
                });
            }

            // 5. Persist document and all chunks through the repository
            await _documentRepository.AddAsync(
                document,
                cancellationToken);

            return document.Id;
        }
    }
}
