using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Documents
{
    public class DocumentIngestionService : IDocumentIngestionService
    {
        private readonly IDocumentTextExtractor _textExtractor;
        private readonly IEmbeddingService _embeddingService;
        private readonly IDocumentRepository _documentRepository;
        private readonly TextChunker _chunker;

        public DocumentIngestionService(
            IDocumentTextExtractor textExtractor,
            IEmbeddingService embeddingService,
            IDocumentRepository documentRepository,
            TextChunker chunker)
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
            var extractedText = await _textExtractor.ExtractTextAsync(
                fileStream,
                fileName,
                cancellationToken);

            if (string.IsNullOrWhiteSpace(extractedText))
                throw new InvalidOperationException(
                    "No text could be extracted from the document.");

            var chunks = _chunker.Split(extractedText);

            if (chunks.Count == 0)
                throw new InvalidOperationException(
                    "No document chunks were generated.");

            var document = new Document
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                StoragePath = storagePath,
                ExtractedText = extractedText,
                Status = "Indexed",
                CreatedAt = DateTime.UtcNow
            };

            for (var i = 0; i < chunks.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var chunkText = chunks[i];

                var vector = await _embeddingService.GenerateEmbeddingAsync(
                    chunkText,
                    cancellationToken);

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
                    //Embedding = new Pgvector.Vector(vector),
                    Embedding = vector,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _documentRepository.AddAsync(
                document,
                cancellationToken);

            return document.Id;
        }
    }
}
