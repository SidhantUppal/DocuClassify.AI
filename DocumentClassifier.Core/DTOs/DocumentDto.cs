namespace DocumentClassifier.Core.DTOs;

/// <summary>
/// Data Transfer Object representing a document and its classification details.
/// </summary>
public class DocumentDto
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
    /// Predicted type or category of the document.
    /// </summary>
    public string PredictedType { get; set; } = string.Empty;
    /// <summary>
    /// Confidence score of the prediction.
    /// </summary>
    public float Confidence { get; set; }
    /// <summary>
    /// Size of the file in bytes.
    /// </summary>
    public long FileSize { get; set; }
    /// <summary>
    /// Date and time when the document was uploaded.
    /// </summary>
    public DateTime UploadDate { get; set; }
    /// <summary>
    /// Current status of the document (e.g., Processed, Uploading).
    /// </summary>
    public string Status { get; set; } = string.Empty;
    /// <summary>
    /// Extracted text content from the document, if available.
    /// </summary>
    public string? ExtractedText { get; set; }
}

/// <summary>
/// Data Transfer Object representing the result of a document classification operation.
/// </summary>
public class DocumentClassificationResult
{
    /// <summary>
    /// Unique identifier for the classified document.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Name of the classified document file.
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    /// <summary>
    /// Predicted type or category of the document.
    /// </summary>
    public string PredictedType { get; set; } = string.Empty;
    /// <summary>
    /// Confidence score of the prediction.
    /// </summary>
    public float Confidence { get; set; }
    /// <summary>
    /// List of alternative classifications with their confidence scores.
    /// </summary>
    public List<AlternativeClassification> Alternatives { get; set; } = new();
    /// <summary>
    /// Time taken to process and classify the document.
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }
}

/// <summary>
/// Represents an alternative classification result for a document.
/// </summary>
public class AlternativeClassification
{
    /// <summary>
    /// Alternative predicted type or category.
    /// </summary>
    public string Type { get; set; } = string.Empty;
    /// <summary>
    /// Confidence score for the alternative prediction.
    /// </summary>
    public float Confidence { get; set; }
}

/// <summary>
/// Data Transfer Object representing statistics about documents in the system.
/// </summary>
public class DocumentStatsDto
{
    /// <summary>
    /// Total number of documents in the system.
    /// </summary>
    public int TotalDocuments { get; set; }
    /// <summary>
    /// Average confidence score across all documents.
    /// </summary>
    public float AverageConfidence { get; set; }
    /// <summary>
    /// Number of documents processed today.
    /// </summary>
    public int ProcessedToday { get; set; }
    /// <summary>
    /// Distribution of document types (type name to count mapping).
    /// </summary>
    public Dictionary<string, int> DocumentTypeDistribution { get; set; } = new();
}