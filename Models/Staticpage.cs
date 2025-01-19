using System;
using System.Collections.Generic;

namespace Fitness_Center.Models;

public partial class Staticpage
{
    public decimal PageId { get; set; }

    public string PageName { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }
}
