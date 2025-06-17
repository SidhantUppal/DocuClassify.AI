using DocumentClassifier.Core.DTOs;

namespace DocumentClassifier.Core.Interfaces;

/// <summary>
/// Service interface for interacting with OpenAI for Q&A and chat about documents.
/// </summary>
public interface IOpenAIService
{
    /// <summary>
    /// Asks a question about a document and returns the answer using OpenAI.
    /// </summary>
    /// <param name="question">The question to ask.</param>
    /// <param name="documentText">The text content of the document.</param>
    /// <param name="documentType">The type or category of the document.</param>
    /// <returns>Q&A response DTO with answer and confidence.</returns>
    Task<QAResponseDto> AskQuestionAboutDocumentAsync(string question, string documentText, string documentType);
    /// <summary>
    /// Processes a chat message in the context of a document using OpenAI, maintaining conversation history.
    /// </summary>
    /// <param name="message">The chat message from the user.</param>
    /// <param name="documentText">The text content of the document.</param>
    /// <param name="documentType">The type or category of the document.</param>
    /// <param name="conversationHistory">Optional conversation history for context.</param>
    /// <returns>Chat response DTO from the assistant.</returns>
    Task<ChatResponseDto> ProcessChatAsync(string message, string documentText, string documentType, List<ChatMessage>? conversationHistory = null);
}