using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitness_Center.Models;

public partial class Member
{
    public decimal MemberId { get; set; }

    public decimal UserId { get; set; }

    public string FullName { get; set; } = null!;

    public DateTime? DateOfBirth { get; set; }

    public string? ContactNumber { get; set; }

    public string? Address { get; set; }

    public decimal? SubscriptionId { get; set; }

    public string? ProfilePicture { get; set; }

    [NotMapped]

    public virtual IFormFile ProfileFile { get; set; }

    public virtual ICollection<Bankaccount> Bankaccounts { get; set; } = new List<Bankaccount>();

    public virtual Subscription? Subscription { get; set; }

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Workout> Workouts { get; set; } = new List<Workout>();
}
