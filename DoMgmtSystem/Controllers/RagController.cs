using Infrastructure.Documents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoMgmtSystem.Controllers
{
    [ApiController]
    [Route("api/rag")]
    public class RagController : ControllerBase
    {
        private readonly RagService _ragService;

        public RagController(RagService ragService)
        {
            _ragService = ragService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask(
            [FromBody] AskRequest request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest("Question is required.");

            var response = await _ragService.AskAsync(
                request.Question,
                cancellationToken);

            return Ok(response);
        }
    }

    public class AskRequest
    {
        public string Question { get; set; } = string.Empty;
    }
}
