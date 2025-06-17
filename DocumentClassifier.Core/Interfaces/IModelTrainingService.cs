using DocumentClassifier.Core.DTOs;

namespace DocumentClassifier.Core.Interfaces;

/// <summary>
/// Service interface for managing model training operations.
/// </summary>
public interface IModelTrainingService
{
    /// <summary>
    /// Starts a new model training job asynchronously.
    /// </summary>
    /// <returns>The unique identifier of the started training job.</returns>
    Task<Guid> StartTrainingAsync();
    /// <summary>
    /// Gets the status and details of a specific training job.
    /// </summary>
    /// <param name="jobId">The unique identifier of the training job.</param>
    /// <returns>Training job DTO with status and metrics.</returns>
    Task<TrainingJobDto> GetTrainingStatusAsync(Guid jobId);
    /// <summary>
    /// Retrieves the evaluation metrics of the current model.
    /// </summary>
    /// <returns>Model metrics DTO.</returns>
    Task<ModelMetricsDto> GetModelMetricsAsync();
    /// <summary>
    /// Checks if a training job is currently in progress.
    /// </summary>
    /// <returns>True if training is in progress; otherwise, false.</returns>
    Task<bool> IsTrainingInProgressAsync();
}