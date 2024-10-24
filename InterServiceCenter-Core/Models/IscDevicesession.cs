using System;
using System.Collections.Generic;

namespace InterServiceCenter_Core.Models;

public partial class IscDevicesession
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string DeviceName { get; set; } = null!;

    public string DeviceId { get; set; } = null!;

    public string IpAddress { get; set; } = null!;

    public DateTime? ExpireIn { get; set; }
}
