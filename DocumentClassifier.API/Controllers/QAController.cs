using DocumentClassifier.Core.DTOs;
using DocumentClassifier.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DocumentClassifier.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QAController : ControllerBase
{
    private readonly IOpenAIService _openAIService;
    private readonly IDocumentRepository _documentRepository;
    private readonly ILogger<QAController> _logger;

    public QAController(
        IOpenAIService openAIService,
        IDocumentRepository documentRepository,
        ILogger<QAController> logger)
    {
        _openAIService = openAIService;
        _documentRepository = documentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Answers a question about a specific document using OpenAI, based on the document's extracted text and predicted type.
    /// </summary>
    /// <param name="request">The question and document ID.</param>
    /// <returns>Answer, confidence, and related metadata.</returns>
    [HttpPost("ask")]
    public async Task<ActionResult<QAResponseDto>> AskQuestion([FromBody] QARequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest("Question is required");

            if (request.DocumentId == Guid.Empty)
                return BadRequest("Document ID is required");

            // Get document
            var document = await _documentRepository.GetByIdAsync(request.DocumentId);
            if (document == null)
                return NotFound("Document not found");

            // Ask OpenAI
            var response = await _openAIService.AskQuestionAboutDocumentAsync(
                request.Question, 
                document.ExtractedText,
                document.PredictedType);

            var result = new QAResponseDto
            {
                Question = request.Question,
                Answer = response.Answer,
                Confidence = response.Confidence,
                DocumentId = request.DocumentId,
                Timestamp = DateTime.UtcNow
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Q&A request");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Processes a chat message in the context of a specific document, maintaining conversation history.
    /// </summary>
    /// <param name="request">The chat message, document ID, and conversation history.</param>
    /// <returns>Chat response from the assistant.</returns>
    [HttpPost("chat")]
    public async Task<ActionResult<ChatResponseDto>> Chat([FromBody] ChatRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("Message is required");

            if (request.DocumentId == Guid.Empty)
                return BadRequest("Document ID is required");

            // Get document
            var document = await _documentRepository.GetByIdAsync(request.DocumentId);
            if (document == null)
                return NotFound("Document not found");

            // Process chat with context
            var response = await _openAIService.ProcessChatAsync(
                request.Message,
                document.ExtractedText,
                document.PredictedType,
                request.ConversationHistory);

            var result = new ChatResponseDto
            {
                Message = response.Message,
                DocumentId = request.DocumentId,
                Timestamp = DateTime.UtcNow,
                IsFromAssistant = true
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat request");
            return StatusCode(500, "Internal server error");
        }
    }
}