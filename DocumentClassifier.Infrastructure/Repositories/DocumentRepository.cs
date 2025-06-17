using DocumentClassifier.Core.DTOs;
using DocumentClassifier.Core.Interfaces;
using DocumentClassifier.Core.Models;
using DocumentClassifier.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DocumentClassifier.Infrastructure.Repositories;

/// <summary>
/// Repository for managing document entities in the database.
/// </summary>
public class DocumentRepository : IDocumentRepository
{
    /// <summary>
    /// The application's database context.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentRepository"/> class.
    /// </summary>
    /// <param name="context">The application's database context.</param>
    public DocumentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a document by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the document.</param>
    /// <returns>The document if found; otherwise, null.</returns>
    public async Task<Document?> GetByIdAsync(Guid id)
    {
        return await _context.Documents.FindAsync(id);
    }

    /// <summary>
    /// Retrieves a paginated list of documents, optionally filtered by search term and document type.
    /// </summary>
    /// <param name="searchTerm">Optional search term to filter documents.</param>
    /// <param name="documentType">Optional document type to filter documents.</param>
    /// <param name="page">Page number for pagination.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <returns>List of documents matching the criteria.</returns>
    public async Task<IEnumerable<Document>> GetDocumentsAsync(string? searchTerm = null, string? documentType = null, int page = 1, int pageSize = 10)
    {
        var query = _context.Documents.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(d => d.FileName.Contains(searchTerm) || d.PredictedType.Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(documentType) && documentType != "all")
        {
            query = query.Where(d => d.PredictedType == documentType);
        }

        return await query
            .OrderByDescending(d => d.UploadDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new document to the database.
    /// </summary>
    /// <param name="document">The document to add.</param>
    /// <returns>The added document.</returns>
    public async Task<Document> AddAsync(Document document)
    {
        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
        return document;
    }

    /// <summary>
    /// Updates an existing document in the database.
    /// </summary>
    /// <param name="document">The document to update.</param>
    public async Task UpdateAsync(Document document)
    {
        _context.Documents.Update(document);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a document by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the document to delete.</param>
    public async Task DeleteAsync(Guid id)
    {
        var document = await _context.Documents.FindAsync(id);
        if (document != null)
        {
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Retrieves statistics about the documents in the system.
    /// </summary>
    /// <returns>Document statistics DTO.</returns>
    public async Task<DocumentStatsDto> GetDocumentStatsAsync()
    {
        var totalDocuments = await _context.Documents.CountAsync();
        var averageConfidence = await _context.Documents.AverageAsync(d => d.Confidence);
        var processedToday = await _context.Documents
            .CountAsync(d => d.UploadDate.Date == DateTime.UtcNow.Date);

        var typeDistribution = await _context.Documents
            .GroupBy(d => d.PredictedType)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Type, x => x.Count);

        return new DocumentStatsDto
        {
            TotalDocuments = totalDocuments,
            AverageConfidence = averageConfidence,
            ProcessedToday = processedToday,
            DocumentTypeDistribution = typeDistribution
        };
    }
}