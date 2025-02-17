using System;
using System.Collections.Generic;

namespace InterServiceCenter_Core.Models;

public partial class IscPasswordresettoken
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Token { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime ExpiresIn { get; set; }

    public int AccountId { get; set; }

    public virtual IscAccount Account { get; set; } = null!;
}
