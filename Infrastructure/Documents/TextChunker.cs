using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Documents
{
    public class TextChunker
    {
        public List<string> Split(
            string text,
            int chunkSize = 3500,
            int overlap = 400)
        {
            if (chunkSize <= 0 || overlap < 0 || overlap >= chunkSize)
                throw new ArgumentException("Invalid chunk settings.");

            var chunks = new List<string>();

            for (int start = 0; start < text.Length;)
            {
                int length = Math.Min(chunkSize, text.Length - start);

                var chunk = text.Substring(start, length);

                if (!string.IsNullOrWhiteSpace(chunk))
                    chunks.Add(chunk);

                if (start + length >= text.Length)
                    break;

                start += chunkSize - overlap;
            }

            return chunks;
        }
    }
}
