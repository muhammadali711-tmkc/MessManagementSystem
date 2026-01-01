using System;
using System.Collections.Generic;

namespace MessManagementSystem.Models;

public partial class SystemConfig
{
    public int ConfigId { get; set; }

    public string KeyName { get; set; } = null!;

    public decimal Value { get; set; }

    public string? Description { get; set; }
}
