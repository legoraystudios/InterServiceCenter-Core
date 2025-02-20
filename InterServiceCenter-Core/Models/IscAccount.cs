﻿using System;
using System.Collections.Generic;

namespace InterServiceCenter_Core.Models;

public partial class IscAccount
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string HashedPassword { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? ProfilePhotoFile { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<IscPasswordresettoken> IscPasswordresettokens { get; set; } = new List<IscPasswordresettoken>();

    public virtual ICollection<IscPost> IscPosts { get; set; } = new List<IscPost>();

    public virtual ICollection<IscStatusbarmessage> IscStatusbarmessageCreatedByNavigations { get; set; } = new List<IscStatusbarmessage>();

    public virtual ICollection<IscStatusbarmessage> IscStatusbarmessageModifiedByNavigations { get; set; } = new List<IscStatusbarmessage>();
}
