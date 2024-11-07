using System;
using System.Collections.Generic;

namespace InterServiceCenter_Core.Models;

public partial class IscStatusbarproperty
{
    public int Id { get; set; }

    public double MessageInterval { get; set; }

    public int StatusBarColor { get; set; }

    public int StatusBarIcon { get; set; }

    public virtual IscStatusbarcolor StatusBarColorNavigation { get; set; } = null!;

    public virtual IscStatusbaricon StatusBarIconNavigation { get; set; } = null!;
}
