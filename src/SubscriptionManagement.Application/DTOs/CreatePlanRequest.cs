using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubscriptionManagement.Application.DTOs
{
    public class CreatePlanRequest
    {
        public string Name { get; set; } = string.Empty;
        public int DurationInDays { get; set; }
    }
}