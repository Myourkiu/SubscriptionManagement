using SubscriptionManagement.Application.DTOs;
using SubscriptionManagement.Domain.Common;
using SubscriptionManagement.Domain.Models;
using SubscriptionManagement.Domain.ValueObjects;
using SubscriptionManagement.Infrastructure.Repositories;

namespace SubscriptionManagement.Infrastructure.Services
{
    public class SubscriptionService
    {
        private readonly SubscriptionRepository _subscriptionRepository;
        private readonly PlanRepository _planRepository;

        public SubscriptionService(SubscriptionRepository subscriptionRepository, PlanRepository planRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
        }

        public async Task<List<Subscription>> GetAllAsync(GetSubscriptionsRequest? filters = null)
        {
            return await _subscriptionRepository.GetAllAsync(filters);
        }

        public async Task<Result<Subscription>> CreateAsync(CreateSubscriptionRequest request)
        {
            try
            {
                var plan = new Plan(name: request.PlanName, durationInDays: request.DurationInDays);

                await _planRepository.CreateAsync(plan);

                var period = new SubscriptionPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(request.DurationInDays));


                var subscription = new Subscription
                {
                    Plan = plan,
                    Period = period,
                    Status = SubscriptionStatus.Active
                };

                var createdSubscription = await _subscriptionRepository.CreateAsync(subscription);
                return Result<Subscription>.Success(createdSubscription);
            }

            catch (Exception ex)
            {
                return Result<Subscription>.Failure($"Erro ao criar assinatura: {ex.Message}");
            }
        }
    }
}