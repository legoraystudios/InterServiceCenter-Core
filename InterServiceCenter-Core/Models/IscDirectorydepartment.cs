using System;
using System.Collections.Generic;

namespace InterServiceCenter_Core.Models;

public partial class IscDirectorydepartment
{
    public int Id { get; set; }

    public string DepartmentName { get; set; } = null!;

    public string? DepartmentDescription { get; set; }

    public string? AddressNote { get; set; }

    public int FacilityPhoneNumberId { get; set; }

    public string PhoneExtension { get; set; } = null!;

    public int FacilityId { get; set; }

    public virtual IscFacility? Facility { get; set; } = null!;

    public virtual IscFacilityphonenumber? FacilityPhoneNumber { get; set; } = null!;

    public virtual ICollection<IscDirectoryperson> IscDirectorypeople { get; set; } = new List<IscDirectoryperson>();
}
