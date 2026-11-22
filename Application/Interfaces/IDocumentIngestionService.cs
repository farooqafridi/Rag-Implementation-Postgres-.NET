using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IDocumentIngestionService
    {
        Task<Guid> ProcessAsync(
            Stream fileStream,
            string fileName,
            string storagePath,
            CancellationToken cancellationToken = default);
    }
}
