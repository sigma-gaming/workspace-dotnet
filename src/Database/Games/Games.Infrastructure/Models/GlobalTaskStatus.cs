using System;
using System.Collections.Generic;
using Games.Domain.Enums;

namespace Games.Infrastructure.Models;

public class GlobalTaskStatus
{
    public GlobalTaskKey TaskKey { get; set; }
    public Games.Domain.Enums.TaskStatus Status { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
}
