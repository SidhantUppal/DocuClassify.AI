namespace DocumentClassifier.Core.DTOs;

/// <summary>
/// Data Transfer Object representing a single training data entry used for model training.
/// </summary>
public class TrainingDataDto
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
    /// Label/category assigned to the training data.
    /// </summary>
    public string Label { get; set; } = string.Empty;
    /// <summary>
    /// Current status of the training data (e.g., Pending, Validated).
    /// </summary>
    public string Status { get; set; } = string.Empty;
    /// <summary>
    /// Date and time when the training data was uploaded.
    /// </summary>
    public DateTime UploadDate { get; set; }
}

/// <summary>
/// Data Transfer Object representing a model training job and its status.
/// </summary>
public class TrainingJobDto
{
    /// <summary>
    /// Unique identifier for the training job.
    /// </summary>
    public Guid JobId { get; set; }
    /// <summary>
    /// Current status of the training job (e.g., Started, Completed, Failed).
    /// </summary>
    public string Status { get; set; } = string.Empty;
    /// <summary>
    /// Start time of the training job.
    /// </summary>
    public DateTime StartTime { get; set; }
    /// <summary>
    /// End time of the training job, if completed.
    /// </summary>
    public DateTime? EndTime { get; set; }
    /// <summary>
    /// Error message if the training job failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
    /// <summary>
    /// Model evaluation metrics after training completes.
    /// </summary>
    public ModelMetricsDto? Metrics { get; set; }
    /// <summary>
    /// Total number of documents to be processed in this training job.
    /// </summary>
    public int TotalDocuments { get; set; }
    /// <summary>
    /// Number of documents processed so far in this training job.
    /// </summary>
    public int ProcessedDocuments { get; set; }
    /// <summary>
    /// Estimated time to complete the training job, in seconds.
    /// </summary>
    public double? EstimatedTimeSeconds { get; set; }
}

/// <summary>
/// Data Transfer Object representing evaluation metrics for a trained model.
/// </summary>
public class ModelMetricsDto
{
    /// <summary>
    /// Overall accuracy of the model.
    /// </summary>
    public double Accuracy { get; set; }
    /// <summary>
    /// Precision score of the model.
    /// </summary>
    public double Precision { get; set; }
    /// <summary>
    /// Recall score of the model.
    /// </summary>
    public double Recall { get; set; }
    /// <summary>
    /// F1 score of the model.
    /// </summary>
    public double F1Score { get; set; }
    /// <summary>
    /// Per-class evaluation metrics (class name to score mapping).
    /// </summary>
    public Dictionary<string, double> PerClassMetrics { get; set; } = new();
}