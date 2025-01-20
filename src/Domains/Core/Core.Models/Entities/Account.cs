using System;
using System.Collections.Generic;
using Core.Domain.Enums;

namespace Core.Domain.Entities;

public partial class Account
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string ProviderUserId { get; set; } = null!;

    public string? ProviderUsername { get; set; }

    public string ProviderUserFirstName { get; set; } = null!;

    public string? ProviderUserLastName { get; set; }

    public string? ProviderUserImage { get; set; }

    public Guid UserId { get; set; }

    public AccountProvider Provider { get; set; }

    public virtual User User { get; set; } = null!;
}
