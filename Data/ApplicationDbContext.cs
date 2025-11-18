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
            });
        }
    }
}

