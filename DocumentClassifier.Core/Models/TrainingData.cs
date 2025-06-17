namespace DocumentClassifier.Core.Models;

/// <summary>
/// Represents a training data entry used for model training.
/// </summary>
public class TrainingData
{
    /// <summary>
    /// Unique identifier for the training data entry.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Name of the uploaded training file.
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    /// <summary>
    /// File system path to the uploaded training file.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;
    /// <summary>
    /// Label/category assigned to the training data.
    /// </summary>
    public string Label { get; set; } = string.Empty;
    /// <summary>
    /// Extracted text content from the training file.
    /// </summary>
    public string ExtractedText { get; set; } = string.Empty;
    /// <summary>
    /// Date and time when the training data was uploaded.
    /// </summary>
    public DateTime UploadDate { get; set; }
    /// <summary>
    /// Current status of the training data (e.g., Pending, Validated).
    /// </summary>
    public TrainingDataStatus Status { get; set; }
}

/// <summary>
/// Enum representing the status of a training data entry.
/// </summary>
public enum TrainingDataStatus
{
    /// <summary>
    /// The training data is pending validation.
    /// </summary>
    Pending,
    /// <summary>
    /// The training data has been validated.
    /// </summary>
    Validated,
    /// <summary>
    /// The training data is being used for training.
    /// </summary>
    Training,
    /// <summary>
    /// The training process for this data is completed.
    /// </summary>
    Completed
}