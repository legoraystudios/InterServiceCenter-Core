using System;
using System.Collections.Generic;

namespace InterServiceCenter_Core.Models;

public partial class IscFacilityphonenumber
{
    public int Id { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public int FacilityId { get; set; }

    public virtual IscFacility? Facility { get; set; } = null!;

    public virtual ICollection<IscDirectorydepartment>? IscDirectorydepartments { get; set; } = new List<IscDirectorydepartment>();

    public virtual ICollection<IscDirectoryperson>? IscDirectorypeople { get; set; } = new List<IscDirectoryperson>();
}
