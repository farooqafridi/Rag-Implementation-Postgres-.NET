using Application.Features.Documents.GetDocument;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DoMgmtSystem.Controllers
{
    [ApiController]
    [Route("api/documents")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentStorage _storage;
        private readonly IDocumentIngestionService _ingestion;
        private readonly GetDocumentHandler _getDocumentHandler;
        public DocumentsController(IDocumentStorage storage, IDocumentIngestionService ingestion, GetDocumentHandler getDocumentHandler)
        {
            _storage = storage;

            _ingestion = ingestion;

            _getDocumentHandler = getDocumentHandler;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(20_000_000)]
        public async Task<IActionResult> Upload(
        IFormFile file,
        CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please select a document.");

            var allowedExtensions = new[]
            {
            ".pdf", ".docx", ".txt"
        };

            var extension = Path.GetExtension(file.FileName)
                .ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Unsupported file format.");

            string storagePath;

            await using (var fileStream = file.OpenReadStream())
            {
                storagePath = await _storage.SaveAsync(fileStream, file.FileName, cancellationToken);
            }

            try
            {
                await using var storedFile = await _storage.OpenReadAsync(storagePath, cancellationToken);

                var documentId = await _ingestion.ProcessAsync(
                    storedFile,
                    Path.GetFileName(file.FileName),
                    storagePath,
                    cancellationToken);

                return Ok(new
                {
                    DocumentId = documentId,
                    FileName = file.FileName,
                    Message = "Document uploaded and indexed successfully."
                });
            }
            catch
            {
                await _storage.DeleteAsync(
                    storagePath,
                    cancellationToken);

                throw;
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id,CancellationToken cancellationToken)
        {
            var query = new GetDocumentQuery(id);

            var result = await _getDocumentHandler.HandleAsync(
                query,
                cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }
    }
}