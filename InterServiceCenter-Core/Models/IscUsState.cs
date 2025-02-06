using System;
using System.Collections.Generic;

namespace InterServiceCenter_Core.Models;

public partial class IscUsState
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<IscFacility> IscFacilities { get; set; } = new List<IscFacility>();
}
