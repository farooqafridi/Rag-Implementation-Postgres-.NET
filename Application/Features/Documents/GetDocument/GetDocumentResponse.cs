using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Documents.GetDocument
{
    public record GetDocumentResponse(
     Guid Id,
     string FileName,
     string? ContentType,
     string Status,
     DateTime CreatedAt,
     int ChunkCount);
}
