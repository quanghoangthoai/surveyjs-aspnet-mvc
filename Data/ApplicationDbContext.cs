using Microsoft.EntityFrameworkCore;
using surveyjs_aspnet_mvc.Models;

namespace surveyjs_aspnet_mvc.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Survey> Surveys => Set<Survey>();
        public DbSet<SurveyResponse> SurveyResponses => Set<SurveyResponse>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Survey>(entity =>
            {
                entity.ToTable("Surveys");
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(200);
                entity.Property(e => e.Json)
                      .IsRequired()
                      .HasColumnType("nvarchar(max)");
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsSupplierEvaluation)
                      .HasDefaultValue(true);

                entity.HasOne(e => e.Supplier)
                      .WithOne(s => s.Survey)
                      .HasForeignKey<Supplier>(s => s.SurveyId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<SurveyResponse>(entity =>
            {
                entity.ToTable("SurveyResponses");
                entity.Property(e => e.PostId)
                      .IsRequired()
                      .HasMaxLength(128);
                entity.Property(e => e.ResultJson)
                      .IsRequired()
                      .HasColumnType("nvarchar(max)");
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.Survey)
                      .WithMany(p => p.Responses)
                      .HasForeignKey(d => d.SurveyId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Supplier)
                      .WithMany(p => p.Responses)
                      .HasForeignKey(d => d.SupplierId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Suppliers");
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(200);
                entity.Property(e => e.Description)
                      .HasMaxLength(1000);
                entity.HasIndex(e => e.DisplayOrder);
            });
        }
    }
}

