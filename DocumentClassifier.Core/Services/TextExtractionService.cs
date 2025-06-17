using DocumentClassifier.Core.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Text;

namespace DocumentClassifier.Core.Services;

/// <summary>
/// Service for extracting text from PDF, DOCX, and TXT files.
/// </summary>
public class TextExtractionService : ITextExtractionService
{
    /// <summary>
    /// Extracts text from a file, automatically detecting the file type.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>The extracted text content.</returns>
    public async Task<string> ExtractTextAsync(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        
        return extension switch
        {
            ".pdf" => await ExtractTextFromPdfAsync(filePath),
            ".docx" => await ExtractTextFromDocxAsync(filePath),
            ".txt" => await ExtractTextFromTxtAsync(filePath),
            _ => throw new NotSupportedException($"File type {extension} is not supported")
        };
    }

    /// <summary>
    /// Extracts text specifically from a PDF file.
    /// </summary>
    /// <param name="filePath">The path to the PDF file.</param>
    /// <returns>The extracted text content.</returns>
    public async Task<string> ExtractTextFromPdfAsync(string filePath)
    {
        try
        {
            using var reader = new PdfReader(filePath);
            using var document = new PdfDocument(reader);
            
            var text = new StringBuilder();
            
            for (int i = 1; i <= document.GetNumberOfPages(); i++)
            {
                var page = document.GetPage(i);
                var pageText = PdfTextExtractor.GetTextFromPage(page);
                text.AppendLine(pageText);
            }
            
            return text.ToString();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to extract text from PDF: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Extracts text specifically from a DOCX file.
    /// </summary>
    /// <param name="filePath">The path to the DOCX file.</param>
    /// <returns>The extracted text content.</returns>
    public async Task<string> ExtractTextFromDocxAsync(string filePath)
    {
        try
        {
            using var document = WordprocessingDocument.Open(filePath, false);
            var body = document.MainDocumentPart?.Document.Body;
            
            if (body == null)
                return string.Empty;
            
            var text = new StringBuilder();
            
            foreach (var paragraph in body.Elements<Paragraph>())
            {
                text.AppendLine(paragraph.InnerText);
            }
            
            return text.ToString();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to extract text from DOCX: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Extracts text specifically from a TXT file.
    /// </summary>
    /// <param name="filePath">The path to the TXT file.</param>
    /// <returns>The extracted text content.</returns>
    public async Task<string> ExtractTextFromTxtAsync(string filePath)
    {
        try
        {
            return await File.ReadAllTextAsync(filePath);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to read text file: {ex.Message}", ex);
        }
    }
}