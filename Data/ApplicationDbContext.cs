using Microsoft.EntityFrameworkCore;
using JobExchangeMvc.Models;

namespace JobExchangeMvc.Data;

/// <summary>
/// DbContext cho ứng dụng Job Exchange
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<JobView> JobViews { get; set; } // ✅ FIX #14: Job view tracking

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasDefaultValue("Applicant");
        });

        // Company configuration
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasOne(e => e.Employer)
                  .WithOne(e => e.Company)
                  .HasForeignKey<Company>(e => e.EmployerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Job configuration
        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasOne(e => e.Company)
                  .WithMany(e => e.Jobs)
                  .HasForeignKey(e => e.CompanyId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                  .WithMany(e => e.Jobs)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.Status).IsRequired().HasDefaultValue("Pending");
        });

        // Application configuration
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasOne(e => e.Job)
                  .WithMany(e => e.Applications)
                  .HasForeignKey(e => e.JobId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Applicant)
                  .WithMany(e => e.Applications)
                  .HasForeignKey(e => e.ApplicantId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.Status).IsRequired().HasDefaultValue("Pending");

            // Prevent duplicate applications
            entity.HasIndex(e => new { e.JobId, e.ApplicantId }).IsUnique();
        });

        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasOne(e => e.User)
                  .WithMany(e => e.RefreshTokens)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Token);
        });

        // ✅ FIX #14: JobView configuration
        modelBuilder.Entity<JobView>(entity =>
        {
            entity.HasOne(e => e.Job)
                  .WithMany()
                  .HasForeignKey(e => e.JobId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.SetNull);

            // Index for performance (query by JobId, UserId, ViewedAt)
            entity.HasIndex(e => new { e.JobId, e.UserId, e.ViewedAt });
            entity.HasIndex(e => new { e.JobId, e.IpAddress, e.ViewedAt });
        });
    }
}
