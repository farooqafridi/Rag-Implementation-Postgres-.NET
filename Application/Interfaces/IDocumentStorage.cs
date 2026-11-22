using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IDocumentStorage
    {
        Task<string> SaveAsync(
            Stream fileStream,
            string fileName,
            CancellationToken cancellationToken = default);

        Task<Stream> OpenReadAsync(
            string storagePath,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            string storagePath,
            CancellationToken cancellationToken = default);
    }
}
