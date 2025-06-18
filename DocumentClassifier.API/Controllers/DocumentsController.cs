using DocumentClassifier.Core.DTOs;
using DocumentClassifier.Core.Interfaces;
using DocumentClassifier.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocumentClassifier.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentClassificationService _classificationService;
    private readonly ITextExtractionService _textExtractionService;
    private readonly IDocumentRepository _documentRepository;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(
        IDocumentClassificationService classificationService,
        ITextExtractionService textExtractionService,
        IDocumentRepository documentRepository,
        ILogger<DocumentsController> logger)
    {
        _classificationService = classificationService;
        _textExtractionService = textExtractionService;
        _documentRepository = documentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Uploads a document file, extracts its text, classifies it, saves the result, and returns the classification result.
    /// </summary>
    /// <param name="file">The document file to upload (PDF, DOCX, or TXT).</param>
    /// <returns>Classification result including predicted type, confidence, and alternatives.</returns>
    [HttpPost("upload")]
    public async Task<ActionResult<DocumentClassificationResult>> UploadDocument(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            // Validate file type
            var allowedExtensions = new[] { ".pdf", ".docx", ".txt" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("Unsupported file type");

            // Save file
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            Directory.CreateDirectory(uploadsPath);
            
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, fileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Extract text
            var extractedText = await _textExtractionService.ExtractTextAsync(filePath);
            
            // Classify document
            var classification = await _classificationService.ClassifyDocumentAsync(extractedText);
            
            // Save to database
            var document = new Document
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                FilePath = filePath,
                FileSize = file.Length,
                ContentType = file.ContentType,
                ExtractedText = extractedText,
                PredictedType = classification.PredictedLabel,
                Confidence = classification.Confidence,
                UploadDate = DateTime.UtcNow,
                Status = DocumentStatus.Processed
            };

            await _documentRepository.AddAsync(document);

            var result = new DocumentClassificationResult
            {
                Id = document.Id,
                FileName = document.FileName,
                PredictedType = classification.PredictedLabel,
                Confidence = classification.Confidence,
                Alternatives = classification.Alternatives?.Select(a => new AlternativeClassification
                {
                    Type = a.Label,
                    Confidence = a.Confidence
                }).ToList() ?? new List<AlternativeClassification>(),
                ProcessingTime = classification.ProcessingTime
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing document upload");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Retrieves a paginated list of documents, optionally filtered by search term and document type.
    /// </summary>
    /// <param name="searchTerm">Optional search term to filter documents.</param>
    /// <param name="documentType">Optional document type to filter documents.</param>
    /// <param name="page">Page number for pagination.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <returns>List of document DTOs.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocuments(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? documentType = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var documents = await _documentRepository.GetDocumentsAsync(searchTerm, documentType, page, pageSize);
            
            var documentDtos = documents.Select(d => new DocumentDto
            {
                Id = d.Id,
                FileName = d.FileName,
                PredictedType = d.PredictedType,
                Confidence = d.Confidence,
                FileSize = d.FileSize,
                UploadDate = d.UploadDate,
                Status = d.Status.ToString()
            });

            return Ok(documentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving documents");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Retrieves a single document by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the document.</param>
    /// <returns>Document DTO with details and extracted text.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentDto>> GetDocument(Guid id)
    {
        try
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
                return NotFound();

            var documentDto = new DocumentDto
            {
                Id = document.Id,
                FileName = document.FileName,
                PredictedType = document.PredictedType,
                Confidence = document.Confidence,
                FileSize = document.FileSize,
                UploadDate = document.UploadDate,
                Status = document.Status.ToString(),
                ExtractedText = document.ExtractedText
            };

            return Ok(documentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving document {DocumentId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Deletes a document by its unique identifier, including its file from disk and record from the database.
    /// </summary>
    /// <param name="id">The unique identifier of the document.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDocument(Guid id)
    {
        try
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
                return NotFound();

            // Delete file from disk
            if (System.IO.File.Exists(document.FilePath))
            {
                System.IO.File.Delete(document.FilePath);
            }

            await _documentRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document {DocumentId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Retrieves statistics about the documents in the system.
    /// </summary>
    /// <returns>Document statistics DTO.</returns>
    [HttpGet("stats")]
    public async Task<ActionResult<DocumentStatsDto>> GetDocumentStats()
    {
        try
        {
            var stats = await _documentRepository.GetDocumentStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving document stats");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Uploads one or more documents to Azure Blob Storage and returns their URIs.
    /// </summary>
    /// <param name="files">The document files to upload (PDF, DOCX, or TXT).</param>
    /// <returns>List of uploaded file names and their Azure Blob URIs.</returns>
    [HttpPost("upload-to-azure")]
    public async Task<ActionResult<IEnumerable<object>>> UploadDocumentsToAzureBlob([FromForm] IFormFileCollection files)
    {
        if (files == null || files.Count == 0)
            return BadRequest("No files uploaded");

        var allowedExtensions = new[] { ".pdf", ".docx", ".txt" };
        var results = new List<object>();

        foreach (var file in files)
        {
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                results.Add(new { FileName = file.FileName, Error = "Unsupported file type" });
                continue;
            }

            try
            {
                // TODO: Replace with your Azure Blob Storage upload logic
                // Simulate async upload
                await Task.Delay(10); // Remove this and use actual upload logic
                var blobUri = $"https://yourstorageaccount.blob.core.windows.net/yourcontainer/{Guid.NewGuid()}{fileExtension}";

                results.Add(new { FileName = file.FileName, BlobUri = blobUri });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file {FileName} to Azure Blob", file.FileName);
                results.Add(new { FileName = file.FileName, Error = "Upload failed" });
            }
        }

        return Ok(results);
    }

    /// <summary>
    /// Uploads one or more documents to AWS S3 and returns their URIs.
    /// </summary>
    /// <param name="files">The document files to upload (PDF, DOCX, or TXT).</param>
    /// <returns>List of uploaded file names and their AWS S3 URIs.</returns>
    [HttpPost("upload-to-aws")]
    public async Task<ActionResult<IEnumerable<object>>> UploadDocumentsToAWSBlob([FromForm] IFormFileCollection files)
    {
        if (files == null || files.Count == 0)
            return BadRequest("No files uploaded");

        var allowedExtensions = new[] { ".pdf", ".docx", ".txt" };
        var results = new List<object>();

        foreach (var file in files)
        {
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                results.Add(new { FileName = file.FileName, Error = "Unsupported file type" });
                continue;
            }

            try
            {
                // TODO: Replace with your AWS S3 upload logic
                // Simulate async upload
                await Task.Delay(10); // Remove this and use actual AWS S3 upload logic
                var s3Uri = $"https://yourbucket.s3.amazonaws.com/{Guid.NewGuid()}{fileExtension}";

                results.Add(new { FileName = file.FileName, S3Uri = s3Uri });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file {FileName} to AWS S3", file.FileName);
                results.Add(new { FileName = file.FileName, Error = "Upload failed" });
            }
        }

        return Ok(results);
    }

    /// <summary>
    /// Downloads the original document file by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the document.</param>
    /// <returns>The file stream of the document.</returns>
    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadDocument(Guid id)
    {
        try
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null || string.IsNullOrEmpty(document.FilePath) || !System.IO.File.Exists(document.FilePath))
                return NotFound();

            var stream = new FileStream(document.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var contentType = !string.IsNullOrEmpty(document.ContentType) ? document.ContentType : "application/octet-stream";
            var fileName = document.FileName ?? $"document_{id}";
            return File(stream, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading document {DocumentId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}