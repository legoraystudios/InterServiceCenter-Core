using System;
using System.Collections.Generic;

namespace InterServiceCenter_Core.Models;

public partial class IscStatusbaricon
{
    public int Id { get; set; }

    public string IconName { get; set; } = null!;

    public virtual ICollection<IscStatusbarmessage> IscStatusbarmessages { get; set; } = new List<IscStatusbarmessage>();
}
