using System;
using System.Collections.Generic;

namespace MessManagementSystem.Models;

public partial class Attendance
{
    public int AttendanceId { get; set; }

    public int? UserId { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public bool? IsPresent { get; set; }

    public DateTime? RecordedAt { get; set; }

    public virtual User? User { get; set; }
}
