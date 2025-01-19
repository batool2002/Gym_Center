using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitness_Center.Models;

public partial class Trainer
{
    public decimal TrainerId { get; set; }

    public decimal UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Specialization { get; set; }

    public decimal? ExperienceYears { get; set; }

    public string? ContactNumber { get; set; }

    public string? ProfilePicture { get; set; }

    [NotMapped]

    public virtual IFormFile ProfileFile {  get; set; } 


    public virtual User User { get; set; } = null!;

    public virtual ICollection<Workout> Workouts { get; set; } = new List<Workout>();
}
