using DocumentClassifier.Core.DTOs;
using DocumentClassifier.Core.Interfaces;
using DocumentClassifier.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace DocumentClassifier.Core.Services;

/// <summary>
/// Service for managing and executing model training operations.
/// </summary>
public class ModelTrainingService : IModelTrainingService
{
    /// <summary>
    /// The ML.NET context for model operations.
    /// </summary>
    private readonly MLContext _mlContext;
    /// <summary>
    /// Repository for accessing training data.
    /// </summary>
    private readonly ITrainingDataRepository _trainingDataRepository;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    /// <summary>
    /// Dictionary to track training jobs by their unique identifiers.
    /// </summary>
    private readonly Dictionary<Guid, TrainingJobDto> _trainingJobs;
    /// <summary>
    /// The file path to the trained model.
    /// </summary>
    private readonly string _modelPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelTrainingService"/> class.
    /// </summary>
    /// <param name="trainingDataRepository">Repository for accessing training data.</param>
    public ModelTrainingService(ITrainingDataRepository trainingDataRepository, IServiceScopeFactory serviceScopeFactory)
    {
        _mlContext = new MLContext(seed: 0);
        _trainingDataRepository = trainingDataRepository;
        _serviceScopeFactory = serviceScopeFactory;
        _trainingJobs = new Dictionary<Guid, TrainingJobDto>();
        _modelPath = Path.Combine(Directory.GetCurrentDirectory(), "Models", "document-classifier.zip");
        
        // Ensure Models directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(_modelPath)!);
    }

    /// <summary>
    /// Starts a new model training job asynchronously.
    /// </summary>
    /// <returns>The unique identifier of the started training job.</returns>
    public async Task<Guid> StartTrainingAsync()
    {
        var jobId = Guid.NewGuid();
        var job = new TrainingJobDto
        {
            JobId = jobId,
            Status = "Starting",
            StartTime = DateTime.UtcNow
        };

        _trainingJobs[jobId] = job;

        // Start training in background
        _ = Task.Run(async () => await ExecuteTrainingAsync(jobId));

        return jobId;
    }

    /// <summary>
    /// Gets the status and details of a specific training job.
    /// </summary>
    /// <param name="jobId">The unique identifier of the training job.</param>
    /// <returns>Training job DTO with status and metrics.</returns>
    public async Task<TrainingJobDto> GetTrainingStatusAsync(Guid jobId)
    {
        if (_trainingJobs.TryGetValue(jobId, out var job))
        {
            return job;
        }

        throw new ArgumentException($"Training job {jobId} not found");
    }

    /// <summary>
    /// Retrieves the evaluation metrics of the current model.
    /// </summary>
    /// <returns>Model metrics DTO.</returns>
    public async Task<ModelMetricsDto> GetModelMetricsAsync()
    {
        // Return mock metrics for now
        // In a real implementation, you would load these from the last training run
        return new ModelMetricsDto
        {
            Accuracy = 0.942,
            Precision = 0.938,
            Recall = 0.946,
            F1Score = 0.942,
            PerClassMetrics = new Dictionary<string, double>
            {
                ["Invoice"] = 0.95,
                ["Resume"] = 0.92,
                ["Contract"] = 0.89,
                ["Purchase Order"] = 0.94,
                ["Agreement"] = 0.91,
                ["Report"] = 0.88
            }
        };
    }

    /// <summary>
    /// Checks if a training job is currently in progress.
    /// </summary>
    /// <returns>True if training is in progress; otherwise, false.</returns>
    public async Task<bool> IsTrainingInProgressAsync()
    {
        return _trainingJobs.Values.Any(job => job.Status == "Training" || job.Status == "Starting");
    }

    /// <summary>
    /// Executes the model training process for a specific job ID.
    /// </summary>
    /// <param name="jobId">The unique identifier of the training job.</param>
    private async Task ExecuteTrainingAsync(Guid jobId)
    {
        try
        {
            var job = _trainingJobs[jobId];
            job.Status = "Loading Data";

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var trainingDataRepo = scope.ServiceProvider.GetRequiredService<ITrainingDataRepository>();

                var trainingData = await trainingDataRepo.GetValidatedDataAsync();

                if (!trainingData.Any())
                {
                    job.Status = "Failed";
                    job.ErrorMessage = "No validated training data available";
                    job.EndTime = DateTime.UtcNow;
                    job.TotalDocuments = 0;
                    job.ProcessedDocuments = 0;
                    job.EstimatedTimeSeconds = null;
                    return;
                }

                job.Status = "Training";
                job.TotalDocuments = trainingData.Count();
                job.ProcessedDocuments = 0;
                job.EstimatedTimeSeconds = null;
                var startTime = DateTime.UtcNow;

                // Prepare training data
                var dataList = trainingData.Select(td => new DocumentInput
                {
                    Text = td.ExtractedText,
                    Label = td.Label
                }).ToList();

                // Simulate per-document progress (for demo, since ML.NET trains in batch)
                for (int i = 0; i < dataList.Count; i++)
                {
                    // Simulate processing time per document
                    await Task.Delay(200); // Simulate 200ms per document
                    job.ProcessedDocuments = i + 1;
                    var elapsed = (DateTime.UtcNow - startTime).TotalSeconds;
                    var avgPerDoc = elapsed / (i + 1);
                    job.EstimatedTimeSeconds = avgPerDoc * (dataList.Count - (i + 1));
                }

                var dataView = _mlContext.Data.LoadFromEnumerable(dataList);
                var splitData = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
                var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("Label")
                    .Append(_mlContext.Transforms.Text.FeaturizeText("Features", "Text"))
                    .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                    .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

                var model = pipeline.Fit(splitData.TrainSet);
                var predictions = model.Transform(splitData.TestSet);
                var metrics = _mlContext.MulticlassClassification.Evaluate(predictions);
                _mlContext.Model.Save(model, dataView.Schema, _modelPath);

                // Update job status
                job.Status = "Completed";
                job.EndTime = DateTime.UtcNow;
                job.Metrics = new ModelMetricsDto
                {
                    Accuracy = metrics.MacroAccuracy,
                    Precision = metrics.MacroAccuracy, // Simplified for demo
                    Recall = metrics.MacroAccuracy,
                    F1Score = metrics.MacroAccuracy
                };
                job.ProcessedDocuments = job.TotalDocuments;
                job.EstimatedTimeSeconds = null;
            }
        }
        catch (Exception ex)
        {
            var job = _trainingJobs[jobId];
            job.Status = "Failed";
            job.ErrorMessage = ex.Message;
            job.EndTime = DateTime.UtcNow;
            job.EstimatedTimeSeconds = null;
            job.ProcessedDocuments = job.TotalDocuments;
        }
    }

    public string PredictDocumentType(string extractedText)
    {
        // Ensure the model exists
        if (!File.Exists(_modelPath))
            throw new FileNotFoundException("Trained model not found.", _modelPath);

        // Load the model
        ITransformer trainedModel;
        using (var stream = new FileStream(_modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            trainedModel = _mlContext.Model.Load(stream, out var modelInputSchema);
        }

        // Create prediction engine
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<DocumentInput, DocumentPrediction>(trainedModel);

        // Create input
        var input = new DocumentInput { Text = extractedText };

        // Predict
        var prediction = predictionEngine.Predict(input);

        return prediction.PredictedLabel;
    }
}

public class DocumentPrediction
{
    [ColumnName("PredictedLabel")]
    public string PredictedLabel { get; set; }
    public float[] Score { get; set; }
}