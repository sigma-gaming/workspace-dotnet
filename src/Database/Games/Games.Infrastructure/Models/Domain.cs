using System;
using System.Collections.Generic;
using Games.Domain.Enums;

namespace Games.Infrastructure.Models;

public partial class Domain
{
    public string Host { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DomainApp App { get; set; }
}
