using System;
using System.Collections.Generic;
using Core.Domain.Enums;

namespace Core.Domain.Entities;

public partial class Domain
{
    public string Host { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DomainApp App { get; set; }
}
