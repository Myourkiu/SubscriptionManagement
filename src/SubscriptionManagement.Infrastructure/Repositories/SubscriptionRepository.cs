using Microsoft.EntityFrameworkCore;
using SubscriptionManagement.Application.DTOs;
using SubscriptionManagement.Domain.Models;
using SubscriptionManagement.Infrastructure.Data;

namespace SubscriptionManagement.Infrastructure.Repositories
{
    public class SubscriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Subscription?> GetByIdAsync(Guid id)
        {
            return await _context.Subscriptions
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Subscription>> GetAllAsync(GetSubscriptionsRequest? filters = null)
        {
            var query = _context.Subscriptions.Include(s => s.Plan).AsQueryable();

            if (filters != null)
            {
                if (filters.StartDate.HasValue)
                {
                    var startDateUtc = filters.StartDate.Value.Kind == DateTimeKind.Utc
                        ? filters.StartDate.Value
                        : filters.StartDate.Value.ToUniversalTime();
                    query = query.Where(s => s.Period.StartDate >= startDateUtc);
                }

                if (filters.EndDate.HasValue)
                {
                    var endDateUtc = filters.EndDate.Value.Kind == DateTimeKind.Utc
                        ? filters.EndDate.Value
                        : filters.EndDate.Value.ToUniversalTime();
                    query = query.Where(s => s.Period.EndDate <= endDateUtc);
                }

                if (filters.Status.HasValue)
                {
                    query = query.Where(s => s.Status == filters.Status.Value);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<Subscription> CreateAsync(Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<Subscription> UpdateAsync(Subscription subscription)
        {
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task ExpireSubscriptionsAsync()
        {
            var now = DateTime.UtcNow;
            var activeSubscriptions = await _context.Subscriptions
                .Where(s => s.Status == SubscriptionStatus.Active)
                .ToListAsync();

            bool hasChanges = false;
            foreach (var subscription in activeSubscriptions)
            {
                if (subscription.Period.EndDate < now)
                {
                    subscription.Expire();
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}