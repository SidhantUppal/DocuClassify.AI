using DocumentClassifier.Core.Models;

namespace DocumentClassifier.Core.Interfaces;

/// <summary>
/// Repository interface for managing training data entities in the data store.
/// </summary>
public interface ITrainingDataRepository
{
    /// <summary>
    /// Retrieves a training data entry by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the training data entry.</param>
    /// <returns>The training data entry if found; otherwise, null.</returns>
    Task<TrainingData?> GetByIdAsync(Guid id);
    /// <summary>
    /// Retrieves all training data entries.
    /// </summary>
    /// <returns>List of all training data entries.</returns>
    Task<IEnumerable<TrainingData>> GetAllAsync();
    /// <summary>
    /// Retrieves all validated training data entries.
    /// </summary>
    /// <returns>List of validated training data entries.</returns>
    Task<IEnumerable<TrainingData>> GetValidatedDataAsync();
    /// <summary>
    /// Adds a new training data entry to the data store.
    /// </summary>
    /// <param name="trainingData">The training data entry to add.</param>
    /// <returns>The added training data entry.</returns>
    Task<TrainingData> AddAsync(TrainingData trainingData);
    /// <summary>
    /// Updates an existing training data entry in the data store.
    /// </summary>
    /// <param name="trainingData">The training data entry to update.</param>
    Task UpdateAsync(TrainingData trainingData);
    /// <summary>
    /// Deletes a training data entry by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the training data entry to delete.</param>
    Task DeleteAsync(Guid id);
}