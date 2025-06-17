namespace DocumentClassifier.API.Extensions;

public class FileUploadOptions
{
    public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10MB
    public string[] AllowedExtensions { get; set; } = { ".pdf", ".docx", ".txt" };
    public string UploadPath { get; set; } = "uploads";
}