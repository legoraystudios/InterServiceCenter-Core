using System;
using System.Collections.Generic;

namespace InterServiceCenter_Core.Models;

public partial class IscStatusbarcolor
{
    public int Id { get; set; }

    public string ColorName { get; set; } = null!;

    public virtual ICollection<IscStatusbarproperty> IscStatusbarproperties { get; set; } = new List<IscStatusbarproperty>();
}
