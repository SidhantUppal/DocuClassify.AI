using DocumentClassifier.Core.DTOs;
using DocumentClassifier.Core.Models;

namespace DocumentClassifier.Core.Interfaces;

/// <summary>
/// Repository interface for managing document entities in the data store.
/// </summary>
public interface IDocumentRepository
{
    /// <summary>
    /// Retrieves a document by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the document.</param>
    /// <returns>The document if found; otherwise, null.</returns>
    Task<Document?> GetByIdAsync(Guid id);
    /// <summary>
    /// Retrieves a paginated list of documents, optionally filtered by search term and document type.
    /// </summary>
    /// <param name="searchTerm">Optional search term to filter documents.</param>
    /// <param name="documentType">Optional document type to filter documents.</param>
    /// <param name="page">Page number for pagination.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <returns>List of documents matching the criteria.</returns>
    Task<IEnumerable<Document>> GetDocumentsAsync(string? searchTerm = null, string? documentType = null, int page = 1, int pageSize = 10);
    /// <summary>
    /// Adds a new document to the data store.
    /// </summary>
    /// <param name="document">The document to add.</param>
    /// <returns>The added document.</returns>
    Task<Document> AddAsync(Document document);
    /// <summary>
    /// Updates an existing document in the data store.
    /// </summary>
    /// <param name="document">The document to update.</param>
    Task UpdateAsync(Document document);
    /// <summary>
    /// Deletes a document by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the document to delete.</param>
    Task DeleteAsync(Guid id);
    /// <summary>
    /// Retrieves statistics about the documents in the system.
    /// </summary>
    /// <returns>Document statistics DTO.</returns>
    Task<DocumentStatsDto> GetDocumentStatsAsync();
}