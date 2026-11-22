namespace Domain.Entities
{
    public class Document
    {
        public Guid Id { get; set; }

        public string FileName { get; set; } = null!;

        public string? ContentType { get; set; }

        public string StoragePath { get; set; } = null!;

        public string? ExtractedText { get; set; }

        public string Status { get; set; } = "Uploaded";

        public DateTime CreatedAt { get; set; } =
            DateTime.UtcNow;

        public ICollection<DocumentChunk> Chunks { get; set; }
            = new List<DocumentChunk>();
    }
}
