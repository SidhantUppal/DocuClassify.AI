using DocumentClassifier.Core.DTOs;
using DocumentClassifier.Core.Interfaces;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace DocumentClassifier.Core.Services;

/// <summary>
/// Service for interacting with OpenAI to perform Q&A and chat about documents.
/// </summary>
public class OpenAIService : IOpenAIService
{
    /// <summary>
    /// The OpenAI chat client used for communication.
    /// </summary>
    private readonly ChatClient _chatClient;
    /// <summary>
    /// The OpenAI configuration options.
    /// </summary>
    private readonly OpenAIOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIService"/> class.
    /// </summary>
    /// <param name="options">The OpenAI configuration options.</param>
    public OpenAIService(IOptions<OpenAIOptions> options)
    {
        _options = options.Value;
        _chatClient = new ChatClient("gpt-3.5-turbo", _options.ApiKey);
    }

    /// <summary>
    /// Asks a question about a document and returns the answer using OpenAI.
    /// </summary>
    /// <param name="question">The question to ask.</param>
    /// <param name="documentText">The text content of the document.</param>
    /// <param name="documentType">The type or category of the document.</param>
    /// <returns>Q&A response DTO with answer and confidence.</returns>
    public async Task<QAResponseDto> AskQuestionAboutDocumentAsync(string question, string documentText, string documentType)
    {
        var systemPrompt = $@"You are an AI assistant specialized in analyzing {documentType} documents. 
        You will be provided with the text content of a document and asked questions about it.
        Please provide accurate, concise answers based only on the information available in the document.
        If the information is not available in the document, clearly state that.";

        var userPrompt = $@"Document Type: {documentType}
        Document Content: {documentText}
        
        Question: {question}
        
        Please answer the question based on the document content provided.";

        var messages = new List<OpenAI.Chat.ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userPrompt)
        };

        try
        {
            var response = await _chatClient.CompleteChatAsync(messages);
            
            return new QAResponseDto
            {
                Question = question,
                Answer = response.Value.Content[0].Text,
                Confidence = 0.9f, // OpenAI doesn't provide confidence scores
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            return new QAResponseDto
            {
                Question = question,
                Answer = $"I apologize, but I encountered an error while processing your question: {ex.Message}",
                Confidence = 0.0f,
                Timestamp = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Processes a chat message in the context of a document using OpenAI, maintaining conversation history.
    /// </summary>
    /// <param name="message">The chat message from the user.</param>
    /// <param name="documentText">The text content of the document.</param>
    /// <param name="documentType">The type or category of the document.</param>
    /// <param name="conversationHistory">Optional conversation history for context.</param>
    /// <returns>Chat response DTO from the assistant.</returns>
    public async Task<ChatResponseDto> ProcessChatAsync(string message, string documentText, string documentType, List<OpenAI.Chat.ChatMessage>? conversationHistory = null)
    {
        var systemPrompt = $@"You are an AI assistant specialized in analyzing {documentType} documents. 
        You are having a conversation with a user about a specific document.
        Provide helpful, accurate responses based on the document content.
        Maintain context from the conversation history.";

        var messages = new List<OpenAI.Chat.ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage($"Document Type: {documentType}\nDocument Content: {documentText}")
        };

        // Add conversation history
        if (conversationHistory != null)
        {
            foreach (var historyMessage in conversationHistory.TakeLast(10)) // Limit history to last 10 messages
            {
                //if (historyMessage.Role == "user")
                //{
                //    messages.Add(new UserChatMessage(historyMessage.Content));
                //}
                //else
                //{
                //    messages.Add(new AssistantChatMessage(historyMessage.Content));
                //}
                messages.Add(new AssistantChatMessage(historyMessage.Content));
            }
        }

        // Add current message
        messages.Add(new UserChatMessage(message));

        try
        {
            var response = await _chatClient.CompleteChatAsync(messages);
            
            return new ChatResponseDto
            {
                Message = response.Value.Content[0].Text,
                Timestamp = DateTime.UtcNow,
                IsFromAssistant = true
            };
        }
        catch (Exception ex)
        {
            return new ChatResponseDto
            {
                Message = $"I apologize, but I encountered an error: {ex.Message}",
                Timestamp = DateTime.UtcNow,
                IsFromAssistant = true
            };
        }
    }

    /// <summary>
    /// Not implemented: Processes a chat message using DTOs.ChatMessage conversation history.
    /// </summary>
    public Task<ChatResponseDto> ProcessChatAsync(string message, string documentText, string documentType, List<DTOs.ChatMessage>? conversationHistory = null)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Configuration options for OpenAI integration.
/// </summary>
public class OpenAIOptions
{
    /// <summary>
    /// The API key for OpenAI authentication.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
    /// <summary>
    /// The model name to use for OpenAI requests.
    /// </summary>
    public string Model { get; set; } = "gpt-3.5-turbo";
}