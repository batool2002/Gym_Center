using System;
using System.Collections.Generic;

namespace Fitness_Center.Models;

public partial class Subscription
{
    public decimal SubscriptionId { get; set; }

    public decimal MemberId { get; set; }

    public decimal PlanId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public DateTime? PaymentDate { get; set; }

    public string? InvoicePdf { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Membershipplan Plan { get; set; } = null!;
}
