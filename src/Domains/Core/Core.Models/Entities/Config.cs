using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class Config
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool MaintenanceEnabled { get; set; }

    public bool BackgroundJobsEnabled { get; set; }
}
