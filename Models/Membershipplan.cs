using System;
using System.Collections.Generic;

namespace Fitness_Center.Models;

public partial class Membershipplan
{
    public decimal PlanId { get; set; }

    public string PlanName { get; set; } = null!;

    public decimal DurationMonths { get; set; }

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
