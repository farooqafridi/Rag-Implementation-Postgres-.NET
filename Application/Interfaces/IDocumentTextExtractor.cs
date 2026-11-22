using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IDocumentTextExtractor
    {
        Task<string> ExtractTextAsync(
            Stream stream,
            string fileName,
            CancellationToken cancellationToken = default);
    }
}
