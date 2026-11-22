using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Domain.Entities
{
    public class DocumentChunk
    {
        public Guid Id { get; set; }

        public Guid DocumentId { get; set; }

        public string Content { get; set; } = null!;

        public int ChunkIndex { get; set; }

        public int? PageNumber { get; set; }

        //public Pgvector.Vector Embedding { get; set; } = null!;

        public float[] Embedding { get; set; } = null!;


        public DateTime CreatedAt { get; set; } =
            DateTime.UtcNow;

        public Document Document { get; set; } = null!;
    }
}
