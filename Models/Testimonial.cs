using System;
using System.Collections.Generic;

namespace Fitness_Center.Models;

public partial class Testimonial
{
    public decimal TestimonialId { get; set; }

    public decimal MemberId { get; set; }

    public string Content { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Member Member { get; set; } = null!;
}
