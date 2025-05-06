using System;
using System.Collections.Generic;

namespace NetAPIGrid.Models;

public partial class VwUser
{
    public int UserId { get; set; }

    public string? O365Uid { get; set; }

    public string? Eid { get; set; }

    public string? NameOfUser { get; set; }

    public string? Email { get; set; }

    public int? RoleId { get; set; }

    public string? Status { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDt { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDt { get; set; }

    public string? RoleName { get; set; }
}
