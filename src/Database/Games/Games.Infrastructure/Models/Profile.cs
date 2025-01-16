using System;
using System.Collections.Generic;
using Games.Domain.Enums;

namespace Games.Infrastructure.Models;

public partial class Profile
{
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string Name { get; set; } = null!;

    public string? Username { get; set; }

    public Guid UserId { get; set; }

    public bool HasCustomName { get; set; }

    public string? Image { get; set; }

    public int Id { get; set; }

    public AccountProvider UsedProvider { get; set; }

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual User User { get; set; } = null!;
}
