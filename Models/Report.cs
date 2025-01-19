using System;
using System.Collections.Generic;

namespace Fitness_Center.Models;

public partial class Report
{
    public decimal ReportId { get; set; }

    public string ReportType { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
