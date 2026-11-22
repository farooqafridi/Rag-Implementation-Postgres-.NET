using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Documents.Storage
{
    public class LocalDocumentStorage : IDocumentStorage
    {
        private readonly string _storageRoot;

        public LocalDocumentStorage(IConfiguration configuration)
        {
            _storageRoot = configuration["DocumentStorage:RootPath"]
                ?? Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "uploads");
        }

        public async Task<string> SaveAsync(
            Stream fileStream,
            string fileName,
            CancellationToken cancellationToken = default)
        {
            Directory.CreateDirectory(_storageRoot);

            var extension = Path.GetExtension(fileName);

            var storageFileName = $"{Guid.NewGuid()}{extension}";

            var fullPath = Path.Combine(
                _storageRoot,
                storageFileName);

            await using var outputStream = new FileStream(
                fullPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true);

            await fileStream.CopyToAsync(
                outputStream,
                cancellationToken);

            return fullPath;
        }

        public Task<Stream> OpenReadAsync(
            string storagePath,
            CancellationToken cancellationToken = default)
        {
            Stream stream = new FileStream(
                storagePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 81920,
                useAsync: true);

            return Task.FromResult(stream);
        }

        public Task DeleteAsync(
            string storagePath,
            CancellationToken cancellationToken = default)
        {
            if (File.Exists(storagePath))
            {
                File.Delete(storagePath);
            }

            return Task.CompletedTask;
        }
    }
}
