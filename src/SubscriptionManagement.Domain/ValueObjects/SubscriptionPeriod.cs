using SubscriptionManagement.Domain.Common;
using SubscriptionManagement.Domain.Exceptions;

namespace SubscriptionManagement.Domain.ValueObjects
{
    public class SubscriptionPeriod : ValueObject
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public SubscriptionPeriod(DateTime startDate, DateTime endDate)
        {
            ValidatePeriod(startDate, endDate);

            this.StartDate = startDate;
            this.EndDate = endDate;
        }

        private static void ValidatePeriod(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
            {
                throw new DomainException("Período de assinatura inválido: A data de início deve ser anterior à data de término");
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StartDate;
            yield return EndDate;
        }
    }
}