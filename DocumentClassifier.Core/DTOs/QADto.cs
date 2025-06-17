namespace DocumentClassifier.Core.DTOs;

/// <summary>
/// Data Transfer Object representing a question asked about a document.
/// </summary>
public class QARequestDto
{
    /// <summary>
    /// The question to be answered about the document.
    /// </summary>
    public string Question { get; set; } = string.Empty;
    /// <summary>
    /// The unique identifier of the document to ask about.
    /// </summary>
    public Guid DocumentId { get; set; }
}

/// <summary>
/// Data Transfer Object representing the response to a question about a document.
/// </summary>
public class QAResponseDto
{
    /// <summary>
    /// The original question asked.
    /// </summary>
    public string Question { get; set; } = string.Empty;
    /// <summary>
    /// The answer generated for the question.
    /// </summary>
    public string Answer { get; set; } = string.Empty;
    /// <summary>
    /// Confidence score of the answer.
    /// </summary>
    public float Confidence { get; set; }
    /// <summary>
    /// The unique identifier of the document the answer is about.
    /// </summary>
    public Guid DocumentId { get; set; }
    /// <summary>
    /// Timestamp when the answer was generated.
    /// </summary>
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Data Transfer Object representing a chat message sent to the assistant about a document.
/// </summary>
public class ChatRequestDto
{
    /// <summary>
    /// The message sent by the user.
    /// </summary>
    public string Message { get; set; } = string.Empty;
    /// <summary>
    /// The unique identifier of the document the chat is about.
    /// </summary>
    public Guid DocumentId { get; set; }
    /// <summary>
    /// Optional conversation history for context.
    /// </summary>
    public List<ChatMessage>? ConversationHistory { get; set; }
}

/// <summary>
/// Data Transfer Object representing a chat response from the assistant.
/// </summary>
public class ChatResponseDto
{
    /// <summary>
    /// The message sent by the assistant.
    /// </summary>
    public string Message { get; set; } = string.Empty;
    /// <summary>
    /// The unique identifier of the document the chat is about.
    /// </summary>
    public Guid DocumentId { get; set; }
    /// <summary>
    /// Timestamp when the response was generated.
    /// </summary>
    public DateTime Timestamp { get; set; }
    /// <summary>
    /// Indicates if the message is from the assistant.
    /// </summary>
    public bool IsFromAssistant { get; set; }
}

/// <summary>
/// Represents a single message in a chat conversation.
/// </summary>
public class ChatMessage
{
    /// <summary>
    /// The role of the message sender ("user" or "assistant").
    /// </summary>
    public string Role { get; set; } = string.Empty; // "user" or "assistant"
    /// <summary>
    /// The content of the chat message.
    /// </summary>
    public string Content { get; set; } = string.Empty;
    /// <summary>
    /// Timestamp when the message was sent.
    /// </summary>
    public DateTime Timestamp { get; set; }
}