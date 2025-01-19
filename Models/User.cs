using System;
using System.Collections.Generic;

namespace Fitness_Center.Models;

public partial class User
{
    public decimal UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Member? Member { get; set; }

    public virtual Trainer? Trainer { get; set; }
}
