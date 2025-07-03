using System;
using System.Collections.Generic;

namespace NetAPIGrid.Models;

public partial class VwUserTeam
{
    public string Eid { get; set; } = null!;

    public short? TeamId { get; set; }

    public string? WorkType { get; set; }

    public int? DeptId { get; set; }

    public string? Segment { get; set; }

    public byte? ProjectId { get; set; }

    public string? Cluster { get; set; }
}
