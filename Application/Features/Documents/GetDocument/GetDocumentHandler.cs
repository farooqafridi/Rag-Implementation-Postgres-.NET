using Application.Interfaces;

namespace Application.Features.Documents.GetDocument
{
    public class GetDocumentHandler
    {
        private readonly IDocumentRepository _repository;

        public GetDocumentHandler(IDocumentRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetDocumentResponse?> HandleAsync(
            GetDocumentQuery query,
            CancellationToken cancellationToken = default)
        {
            var document = await _repository.GetByIdAsync(
                query.Id,
                cancellationToken);

            if (document is null)
                return null;

            return new GetDocumentResponse(
                document.Id,
                document.FileName,
                document.ContentType,
                document.Status,
                document.CreatedAt,
                document.Chunks.Count);
        }
    }
}
