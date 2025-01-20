using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class Budget
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime LastSyncAt { get; set; }

    public long Available { get; set; }
}
