using DocumentClassifier.Core.Interfaces;
using DocumentClassifier.Core.Models;
using DocumentClassifier.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DocumentClassifier.Infrastructure.Repositories;

/// <summary>
/// Repository for managing training data entities in the database.
/// </summary>
public class TrainingDataRepository : ITrainingDataRepository
{
    /// <summary>
    /// The application's database context.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrainingDataRepository"/> class.
    /// </summary>
    /// <param name="context">The application's database context.</param>
    public TrainingDataRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a training data entry by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the training data entry.</param>
    /// <returns>The training data entry if found; otherwise, null.</returns>
    public async Task<TrainingData?> GetByIdAsync(Guid id)
    {
        return await _context.TrainingData.FindAsync(id);
    }

    /// <summary>
    /// Retrieves all training data entries.
    /// </summary>
    /// <returns>List of all training data entries.</returns>
    public async Task<IEnumerable<TrainingData>> GetAllAsync()
    {
        return await _context.TrainingData
            .OrderByDescending(td => td.UploadDate)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all validated training data entries.
    /// </summary>
    /// <returns>List of validated training data entries.</returns>
    public async Task<IEnumerable<TrainingData>> GetValidatedDataAsync()
    {
        return await _context.TrainingData
            .Where(td => td.Status == TrainingDataStatus.Validated)
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new training data entry to the database.
    /// </summary>
    /// <param name="trainingData">The training data entry to add.</param>
    /// <returns>The added training data entry.</returns>
    public async Task<TrainingData> AddAsync(TrainingData trainingData)
    {
        _context.TrainingData.Add(trainingData);
        await _context.SaveChangesAsync();
        return trainingData;
    }

    /// <summary>
    /// Updates an existing training data entry in the database.
    /// </summary>
    /// <param name="trainingData">The training data entry to update.</param>
    public async Task UpdateAsync(TrainingData trainingData)
    {
        _context.TrainingData.Update(trainingData);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a training data entry by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the training data entry to delete.</param>
    public async Task DeleteAsync(Guid id)
    {
        var trainingData = await _context.TrainingData.FindAsync(id);
        if (trainingData != null)
        {
            _context.TrainingData.Remove(trainingData);
            await _context.SaveChangesAsync();
        }
    }
}