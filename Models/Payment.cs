using System;
using System.Collections.Generic;

namespace Fitness_Center.Models;

public partial class Payment
{
    public decimal PaymentId { get; set; }

    public decimal SubscriptionId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateTime? TransactionDate { get; set; }

    public virtual Subscription Subscription { get; set; } = null!;
}
