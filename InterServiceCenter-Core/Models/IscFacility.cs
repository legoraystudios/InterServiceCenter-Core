using System;
using System.Collections.Generic;

namespace InterServiceCenter_Core.Models;

public partial class IscFacility
{
    public int Id { get; set; }

    public string FacilityName { get; set; } = null!;

    public string AddressLineOne { get; set; } = null!;

    public string? AddressLineTwo { get; set; }

    public string City { get; set; } = null!;

    public int State { get; set; }

    public string ZipCode { get; set; } = null!;

    public virtual ICollection<IscDirectorydepartment> IscDirectorydepartments { get; set; } = new List<IscDirectorydepartment>();

    public virtual ICollection<IscFacilityphonenumber> IscFacilityphonenumbers { get; set; } = new List<IscFacilityphonenumber>();

    public virtual IscUsState StateNavigation { get; set; } = null!;
}
