using System;
using System.Collections.Generic;

namespace InterServiceCenter_Core.Models;

public partial class IscPost
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime PublishedAt { get; set; }

    public int PublishedBy { get; set; }

    public string? FrontBannerFile { get; set; }

    public virtual IscAccount PublishedByNavigation { get; set; } = null!;
}
