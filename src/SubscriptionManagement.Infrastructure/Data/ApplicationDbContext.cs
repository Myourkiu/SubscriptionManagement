using Microsoft.EntityFrameworkCore;
using SubscriptionManagement.Domain.Models;

namespace SubscriptionManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Subscription
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Price)
                .HasPrecision(18, 2);
            entity.Property(e => e.DurationDays)
                .IsRequired();
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        });
    }
}
