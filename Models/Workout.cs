using System;
using System.Collections.Generic;

namespace Fitness_Center.Models;

public partial class Workout
{
    public decimal WorkoutId { get; set; }

    public decimal TrainerId { get; set; }

    public decimal MemberId { get; set; }

    public string PlanName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual Trainer Trainer { get; set; } = null!;
}
