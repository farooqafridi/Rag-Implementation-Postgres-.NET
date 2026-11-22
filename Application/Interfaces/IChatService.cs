using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IChatService
    {
        Task<string> GenerateAnswerAsync(
            string systemPrompt,
            string userPrompt,
            CancellationToken cancellationToken = default);
    }
}
