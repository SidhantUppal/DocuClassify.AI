using DocumentClassifier.Core.Models;

namespace DocumentClassifier.Core.Interfaces;

/// <summary>
/// Service interface for classifying documents using a trained model.
/// </summary>
public interface IDocumentClassificationService
{
    /// <summary>
    /// Classifies the given text and returns the classification result.
    /// </summary>
    /// <param name="text">The text content of the document to classify.</param>
    /// <returns>Classification result including predicted label, confidence, and alternatives.</returns>
    Task<ClassificationResult> ClassifyDocumentAsync(string text);
    /// <summary>
    /// Checks if a trained model is available for classification.
    /// </summary>
    /// <returns>True if a model is trained and available; otherwise, false.</returns>
    Task<bool> IsModelTrainedAsync();
    /// <summary>
    /// Reloads the trained model from storage.
    /// </summary>
    /// <returns>A task representing the reload operation.</returns>
    Task ReloadModelAsync();
}