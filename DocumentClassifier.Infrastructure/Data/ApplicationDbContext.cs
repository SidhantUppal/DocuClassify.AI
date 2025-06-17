using DocumentClassifier.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentClassifier.Infrastructure.Data;

/// <summary>
/// Represents the Entity Framework database context for the Document Classifier application.
/// Provides access to Documents and TrainingData entities and configures their schema.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class using the specified options.
    /// </summary>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> representing documents stored in the database.
    /// </summary>
    public DbSet<Document> Documents { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> representing training data used for model training.
    /// </summary>
    public DbSet<TrainingData> TrainingData { get; set; }

    /// <summary>
    /// Configures the schema needed for the context, including entity properties, keys, and indexes.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Document configuration
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.PredictedType).HasMaxLength(50);
            entity.Property(e => e.ExtractedText).HasColumnType("nvarchar(max)");
            entity.HasIndex(e => e.PredictedType);
            entity.HasIndex(e => e.UploadDate);
        });

        // TrainingData configuration
        modelBuilder.Entity<TrainingData>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Label).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ExtractedText).HasColumnType("nvarchar(max)");
            entity.HasIndex(e => e.Label);
            entity.HasIndex(e => e.Status);
        });
    }
}