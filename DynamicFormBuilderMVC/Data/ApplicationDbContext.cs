using DynamicFormBuilderMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace DynamicFormBuilderMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<DynamicForm> Forms { get; set; }
        public DbSet<FormComponent> FormComponents { get; set; }
        public DbSet<EmailSchedule> EmailSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure FormComponent
            modelBuilder.Entity<FormComponent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ComponentId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Label).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Properties).HasColumnType("ntext");
                
                entity.HasOne(e => e.Form)
                    .WithMany(f => f.Components)
                    .HasForeignKey(e => e.FormId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure DynamicForm
            modelBuilder.Entity<DynamicForm>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
            });

            // Configure EmailSchedule
            modelBuilder.Entity<EmailSchedule>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                
                entity.HasOne(e => e.Form)
                    .WithMany(f => f.EmailSchedules)
                    .HasForeignKey(e => e.FormId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data
            modelBuilder.Entity<DynamicForm>().HasData(
                new DynamicForm
                {
                    Id = 1,
                    Title = "Sample Form",
                    Description = "This is a sample form for demonstration",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
        }
    }
}