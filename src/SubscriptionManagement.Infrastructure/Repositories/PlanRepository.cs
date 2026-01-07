using Microsoft.EntityFrameworkCore;
using SubscriptionManagement.Infrastructure.Data;
using SubscriptionManagement.Domain.Models;

namespace SubscriptionManagement.Infrastructure.Repositories
{
    public class PlanRepository
    {
        private readonly ApplicationDbContext _context;

        public PlanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Plan?> GetByIdAsync(Guid id)
        {
            return await _context.Plans.FindAsync(id);
        }

        public async Task<List<Plan>> GetAllAsync()
        {
            return await _context.Plans.ToListAsync();
        }

        public async Task<Plan> CreateAsync(Plan plan)
        {
            _context.Plans.Add(plan);
            await _context.SaveChangesAsync();
            return plan;
        }
    }
}