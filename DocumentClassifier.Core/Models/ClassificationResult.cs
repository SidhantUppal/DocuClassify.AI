namespace DocumentClassifier.Core.Models;

/// <summary>
/// Represents the result of a document classification operation.
/// </summary>
public class ClassificationResult
{
    /// <summary>
    /// The predicted label or category for the document.
    /// </summary>
    public string PredictedLabel { get; set; } = string.Empty;
    /// <summary>
    /// Confidence score of the prediction.
    /// </summary>
    public float Confidence { get; set; }
    /// <summary>
    /// List of alternative predictions with their confidence scores.
    /// </summary>
    public List<AlternativePrediction>? Alternatives { get; set; }
    /// <summary>
    /// Time taken to process and classify the document.
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }
}

/// <summary>
/// Represents an alternative prediction for a document classification.
/// </summary>
public class AlternativePrediction
{
    /// <summary>
    /// The alternative label or category.
    /// </summary>
    public string Label { get; set; } = string.Empty;
    /// <summary>
    /// Confidence score for the alternative prediction.
    /// </summary>
    public float Confidence { get; set; }
}