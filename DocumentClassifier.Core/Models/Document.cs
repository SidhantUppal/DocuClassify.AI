namespace DocumentClassifier.Core.Models;

/// <summary>
/// Represents a document entity with metadata and classification details.
/// </summary>
public class Document
{
    /// <summary>
    /// Unique identifier for the document.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Name of the uploaded document file.
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    /// <summary>
    /// File system path to the uploaded document.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;
    /// <summary>
    /// Size of the file in bytes.
    /// </summary>
    public long FileSize { get; set; }
    /// <summary>
    /// MIME content type of the document.
    /// </summary>
    public string ContentType { get; set; } = string.Empty;
    /// <summary>
    /// Extracted text content from the document.
    /// </summary>
    public string ExtractedText { get; set; } = string.Empty;
    /// <summary>
    /// Predicted type or category of the document.
    /// </summary>
    public string PredictedType { get; set; } = string.Empty;
    /// <summary>
    /// Confidence score of the prediction.
    /// </summary>
    public float Confidence { get; set; }
    /// <summary>
    /// Date and time when the document was uploaded.
    /// </summary>
    public DateTime UploadDate { get; set; }
    /// <summary>
    /// Current status of the document (e.g., Uploading, Processed).
    /// </summary>
    public DocumentStatus Status { get; set; }
}

/// <summary>
/// Enum representing the processing status of a document.
/// </summary>
public enum DocumentStatus
{
    /// <summary>
    /// The document is currently being uploaded.
    /// </summary>
    Uploading,
    /// <summary>
    /// The document is being processed.
    /// </summary>
    Processing,
    /// <summary>
    /// The document has been processed successfully.
    /// </summary>
    Processed,
    /// <summary>
    /// An error occurred during processing.
    /// </summary>
    Error
}