namespace DocumentClassifier.Core.Interfaces;

/// <summary>
/// Service interface for extracting text from various document file types.
/// </summary>
public interface ITextExtractionService
{
    /// <summary>
    /// Extracts text from a file, automatically detecting the file type.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>The extracted text content.</returns>
    Task<string> ExtractTextAsync(string filePath);
    /// <summary>
    /// Extracts text specifically from a PDF file.
    /// </summary>
    /// <param name="filePath">The path to the PDF file.</param>
    /// <returns>The extracted text content.</returns>
    Task<string> ExtractTextFromPdfAsync(string filePath);
    /// <summary>
    /// Extracts text specifically from a DOCX file.
    /// </summary>
    /// <param name="filePath">The path to the DOCX file.</param>
    /// <returns>The extracted text content.</returns>
    Task<string> ExtractTextFromDocxAsync(string filePath);
    /// <summary>
    /// Extracts text specifically from a TXT file.
    /// </summary>
    /// <param name="filePath">The path to the TXT file.</param>
    /// <returns>The extracted text content.</returns>
    Task<string> ExtractTextFromTxtAsync(string filePath);
}