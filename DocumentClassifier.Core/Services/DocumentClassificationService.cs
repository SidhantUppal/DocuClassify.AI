using DocumentClassifier.Core.Interfaces;
using DocumentClassifier.Core.Models;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace DocumentClassifier.Core.Services;

/// <summary>
/// Service for classifying documents using a trained ML model.
/// </summary>
public class DocumentClassificationService : IDocumentClassificationService
{
    /// <summary>
    /// The ML.NET context for model operations.
    /// </summary>
    private readonly MLContext _mlContext;
    /// <summary>
    /// The loaded ML model for document classification.
    /// </summary>
    private ITransformer? _model;
    /// <summary>
    /// The file path to the trained model.
    /// </summary>
    private readonly string _modelPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentClassificationService"/> class and loads the model if available.
    /// </summary>
    public DocumentClassificationService()
    {
        _mlContext = new MLContext(seed: 0);
        _modelPath = Path.Combine(Directory.GetCurrentDirectory(), "Models", "document-classifier.zip");
        
        // Load existing model if available
        LoadModelIfExists();
    }

    /// <summary>
    /// Classifies the given text and returns the classification result.
    /// </summary>
    /// <param name="text">The text content of the document to classify.</param>
    /// <returns>Classification result including predicted label, confidence, and alternatives.</returns>
    public async Task<ClassificationResult> ClassifyDocumentAsync(string text)
    {
        var startTime = DateTime.UtcNow;

        if (_model == null)
        {
            // If no trained model exists, return a default classification
            return new ClassificationResult
            {
                PredictedLabel = "Unknown",
                Confidence = 0.5f,
                ProcessingTime = DateTime.UtcNow - startTime,
                Alternatives = new List<AlternativePrediction>
                {
                    new() { Label = "Invoice", Confidence = 0.3f },
                    new() { Label = "Resume", Confidence = 0.2f }
                }
            };
        }

        var predictionEngine = _mlContext.Model.CreatePredictionEngine<DocumentInput, DocumentPrediction>(_model);
        
        var input = new DocumentInput { Text = text };
        var prediction = predictionEngine.Predict(input);

        // Get top alternatives
        var alternatives = new List<AlternativePrediction>();
        var labels = new[] { "Invoice", "Resume", "Contract", "Purchase Order", "Agreement", "Report" };
        
        for (int i = 0; i < Math.Min(prediction.Score.Length, labels.Length); i++)
        {
            if (labels[i] != prediction.PredictedLabel)
            {
                alternatives.Add(new AlternativePrediction
                {
                    Label = labels[i],
                    Confidence = prediction.Score[i]
                });
            }
        }

        alternatives = alternatives.OrderByDescending(a => a.Confidence).Take(3).ToList();

        return new ClassificationResult
        {
            PredictedLabel = prediction.PredictedLabel,
            Confidence = prediction.Score.Max(),
            ProcessingTime = DateTime.UtcNow - startTime,
            Alternatives = alternatives
        };
    }

    /// <summary>
    /// Checks if a trained model is available for classification.
    /// </summary>
    /// <returns>True if a model is trained and available; otherwise, false.</returns>
    public async Task<bool> IsModelTrainedAsync()
    {
        return _model != null && File.Exists(_modelPath);
    }

    /// <summary>
    /// Reloads the trained model from storage.
    /// </summary>
    /// <returns>A task representing the reload operation.</returns>
    public async Task ReloadModelAsync()
    {
        LoadModelIfExists();
    }

    /// <summary>
    /// Loads the trained model from storage if it exists.
    /// </summary>
    private void LoadModelIfExists()
    {
        try
        {
            if (File.Exists(_modelPath))
            {
                _model = _mlContext.Model.Load(_modelPath, out var modelInputSchema);
            }
        }
        catch (Exception)
        {
            // Model loading failed, will use default classification
            _model = null;
        }
    }
}