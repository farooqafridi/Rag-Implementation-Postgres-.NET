using Domain.Entities;
using Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Document> Documents => Set<Document>();

    public DbSet<DocumentChunk> DocumentChunks => Set<DocumentChunk>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("vector");

        // Documents
        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("documents");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("uuid");

            entity.Property(x => x.FileName)
                .HasColumnName("file_name")
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(x => x.ContentType)
                .HasColumnName("content_type")
                .HasMaxLength(100);

            entity.Property(x => x.StoragePath)
                .HasColumnName("storage_path")
                .IsRequired();

            entity.Property(x => x.ExtractedText)
                .HasColumnName("extracted_text");

            entity.Property(x => x.Status)
                .HasColumnName("status")
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.HasMany(x => x.Chunks)
                .WithOne(x => x.Document)
                .HasForeignKey(x => x.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Document chunks
        modelBuilder.Entity<DocumentChunk>(entity =>
        {
            entity.ToTable("document_chunks");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("uuid");

            entity.Property(x => x.DocumentId)
                .HasColumnName("document_id")
                .HasColumnType("uuid")
                .IsRequired();

            entity.Property(x => x.Content)
                .HasColumnName("content")
                .IsRequired();

            entity.Property(x => x.ChunkIndex)
                .HasColumnName("chunk_index")
                .IsRequired();

            entity.Property(x => x.PageNumber)
                .HasColumnName("page_number");

            entity.Property(x => x.Embedding)
                .HasColumnName("embedding")
                .HasConversion(new VectorConverter())
                .HasColumnType("vector(1024)")
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.HasIndex(x => x.DocumentId)
                .HasDatabaseName("ix_document_chunks_document_id");

            entity.HasIndex(x => x.Embedding)
                .HasDatabaseName("ix_document_chunks_embedding")
                .HasMethod("hnsw")
                .HasOperators("vector_cosine_ops");
        });
    }
}