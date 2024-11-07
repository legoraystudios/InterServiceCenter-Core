using System;
using System.Collections.Generic;

namespace InterServiceCenter_Core.Models;

public partial class IscStatusbarmessage
{
    public int Id { get; set; }

    public string Message { get; set; } = null!;

    public int? Icon { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public DateTime? ExpiresIn { get; set; }

    public virtual IscAccount? CreatedByNavigation { get; set; }

    public virtual IscStatusbaricon? IconNavigation { get; set; }

    public virtual IscAccount? ModifiedByNavigation { get; set; }
}
