using Microsoft.ML.Data;

namespace DocumentClassifier.Core.Models;

/// <summary>
/// Represents the input data for document classification, used by the ML model.
/// </summary>
public class DocumentInput
{
    /// <summary>
    /// The text content of the document.
    /// </summary>
    [LoadColumn(0)]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// The label or category of the document (for training).
    /// </summary>
    [LoadColumn(1)]
    public string Label { get; set; } = string.Empty;
}

/// <summary>
/// Represents the prediction result from the ML model for a document.
/// </summary>
public class DocumentPrediction
{
    /// <summary>
    /// The predicted label or category for the document.
    /// </summary>
    [ColumnName("PredictedLabel")]
    public string PredictedLabel { get; set; } = string.Empty;

    /// <summary>
    /// The array of confidence scores for each possible label.
    /// </summary>
    [ColumnName("Score")]
    public float[] Score { get; set; } = Array.Empty<float>();
}