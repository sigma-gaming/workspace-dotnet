using System;
using System.Collections.Generic;
using Core.Domain.Enums;

namespace Core.Domain.Entities;

public class GlobalTaskStatus
{
    public GlobalTaskKey TaskKey { get; set; }
    public Core.Domain.Enums.TaskStatus Status { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
}
