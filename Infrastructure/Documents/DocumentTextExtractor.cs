using Application.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using UglyToad.PdfPig;

namespace Infrastructure.Documents
{
    public class DocumentTextExtractor : IDocumentTextExtractor
    {
        public async Task<string> ExtractTextAsync(
            Stream stream,
            string fileName,
            CancellationToken cancellationToken = default)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            switch (extension)
            {
                case ".pdf":
                    {
                        using var document = PdfDocument.Open(stream);

                        return string.Join(
                            "\n",
                            document.GetPages().Select(
                                page => page.Text));
                    }

                case ".docx":
                    {
                        using var memory = new MemoryStream();

                        await stream.CopyToAsync(memory, cancellationToken);

                        memory.Position = 0;

                        using var document = WordprocessingDocument.Open(memory, false);

                        return document.MainDocumentPart?.Document.Body?.InnerText ?? string.Empty;
                    }

                case ".txt":
                    {
                        using var reader = new StreamReader(
                            stream, leaveOpen: true);

                        return await reader.ReadToEndAsync(
                            cancellationToken);
                    }

                default:
                    throw new NotSupportedException(
                        "Unsupported document format.");
            }
        }
    }
}
