using System;
using System.Collections.Generic;

namespace InterServiceCenter_Core.Models;

public partial class IscDirectoryperson
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string JobPosition { get; set; } = null!;

    public int FacilityPhoneNumberId { get; set; }

    public string PhoneExtension { get; set; } = null!;

    public string CorporateCellphone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int DirectoryDepartmentId { get; set; }

    public virtual IscDirectorydepartment? DirectoryDepartment { get; set; } = null!;

    public virtual IscFacilityphonenumber? FacilityPhoneNumber { get; set; } = null!;
}
