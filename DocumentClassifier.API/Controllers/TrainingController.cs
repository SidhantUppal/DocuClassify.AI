using DocumentClassifier.Core.DTOs;
using DocumentClassifier.Core.Interfaces;
using DocumentClassifier.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocumentClassifier.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrainingController : ControllerBase
{
    private readonly IModelTrainingService _trainingService;
    private readonly ITrainingDataRepository _trainingDataRepository;
    private readonly ITextExtractionService _textExtractionService;
    private readonly ILogger<TrainingController> _logger;

    public TrainingController(
        IModelTrainingService trainingService,
        ITrainingDataRepository trainingDataRepository,
        ITextExtractionService textExtractionService,
        ILogger<TrainingController> logger)
    {
        _trainingService = trainingService;
        _trainingDataRepository = trainingDataRepository;
        _textExtractionService = textExtractionService;
        _logger = logger;
    }

    /// <summary>
    /// Uploads a training data file with a label, extracts its text, and saves it for model training.
    /// </summary>
    /// <param name="file">The training data file to upload.</param>
    /// <param name="label">The label for the training data.</param>
    /// <returns>Training data DTO with details.</returns>
    [HttpPost("upload-training-data")]
    public async Task<ActionResult<TrainingDataDto>> UploadTrainingData(
        IFormFile file, 
        [FromForm] string label)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            if (string.IsNullOrWhiteSpace(label))
                return BadRequest("Label is required");

            // Save file
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "training-data");
            Directory.CreateDirectory(uploadsPath);
            
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Extract text
            var extractedText = await _textExtractionService.ExtractTextAsync(filePath);

            // Save training data
            var trainingData = new TrainingData
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                FilePath = filePath,
                Label = label,
                ExtractedText = extractedText,
                UploadDate = DateTime.UtcNow,
                Status = TrainingDataStatus.Pending
            };

            await _trainingDataRepository.AddAsync(trainingData);

            var result = new TrainingDataDto
            {
                Id = trainingData.Id,
                FileName = trainingData.FileName,
                Label = trainingData.Label,
                Status = trainingData.Status.ToString(),
                UploadDate = trainingData.UploadDate
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading training data");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Retrieves all uploaded training data.
    /// </summary>
    /// <returns>List of training data DTOs.</returns>
    [HttpGet("training-data")]
    public async Task<ActionResult<IEnumerable<TrainingDataDto>>> GetTrainingData()
    {
        try
        {
            var trainingData = await _trainingDataRepository.GetAllAsync();
            
            var result = trainingData.Select(td => new TrainingDataDto
            {
                Id = td.Id,
                FileName = td.FileName,
                Label = td.Label,
                Status = td.Status.ToString(),
                UploadDate = td.UploadDate
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving training data");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Validates a specific training data entry by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the training data.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("training-data/{id}/validate")]
    public async Task<ActionResult> ValidateTrainingData(Guid id)
    {
        try
        {
            var trainingData = await _trainingDataRepository.GetByIdAsync(id);
            if (trainingData == null)
                return NotFound();

            trainingData.Status = TrainingDataStatus.Validated;
            await _trainingDataRepository.UpdateAsync(trainingData);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating training data {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Starts the model training process using the uploaded and validated training data.
    /// </summary>
    /// <returns>Training job DTO with job ID and status.</returns>
    [HttpPost("start-training")]
    public async Task<ActionResult<TrainingJobDto>> StartTraining()
    {
        try
        {
            var jobId = await _trainingService.StartTrainingAsync();
            
            var result = new TrainingJobDto
            {
                JobId = jobId,
                Status = "Started",
                StartTime = DateTime.UtcNow
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting training");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets the status of a specific training job by its job ID.
    /// </summary>
    /// <param name="jobId">The unique identifier of the training job.</param>
    /// <returns>Training job DTO with status.</returns>
    [HttpGet("training-status/{jobId}")]
    public async Task<ActionResult<TrainingJobDto>> GetTrainingStatus(Guid jobId)
    {
        try
        {
            var status = await _trainingService.GetTrainingStatusAsync(jobId);
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting training status for job {JobId}", jobId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Retrieves the current model's evaluation metrics.
    /// </summary>
    /// <returns>Model metrics DTO.</returns>
    [HttpGet("model-metrics")]
    public async Task<ActionResult<ModelMetricsDto>> GetModelMetrics()
    {
        try
        {
            var metrics = await _trainingService.GetModelMetricsAsync();
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving model metrics");
            return StatusCode(500, "Internal server error");
        }
    }
}