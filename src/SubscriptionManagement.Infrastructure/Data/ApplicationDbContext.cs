using Microsoft.EntityFrameworkCore;
using SubscriptionManagement.Domain.Models;
using SubscriptionManagement.Domain.ValueObjects;

namespace SubscriptionManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Plan> Plans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Plan
        modelBuilder.Entity<Plan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.DurationInDays)
                .IsRequired();
        });

        // Configuração da entidade Subscription
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Plan)
                .WithMany()
                .HasForeignKey("PlanId")
                .IsRequired();

            // Configuração do Value Object SubscriptionPeriod
            entity.OwnsOne(e => e.Period, period =>
            {
                period.Property(p => p.StartDate)
                    .HasColumnName("StartDate")
                    .IsRequired()
                    .HasConversion(
                        v => v.ToUniversalTime(),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
                period.Property(p => p.EndDate)
                    .HasColumnName("EndDate")
                    .IsRequired()
                    .HasConversion(
                        v => v.ToUniversalTime(),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            });

            // Configuração do Status (Enum)
            entity.Property(e => e.Status)
                .HasConversion<int>()
                .IsRequired();
        });
    }
}
