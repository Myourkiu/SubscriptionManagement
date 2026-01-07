using SubscriptionManagement.Infrastructure.Data;
using SubscriptionManagement.Domain.ValueObjects;

namespace SubscriptionManagement.Infrastructure.Repositories
{
    public class PlanRepository
    {
        private readonly ApplicationDbContext _context;

        public PlanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Plan> CreateAsync(Plan plan)
        {
            _context.Plans.Add(plan);
            await _context.SaveChangesAsync();
            return plan;
        }
    }
}