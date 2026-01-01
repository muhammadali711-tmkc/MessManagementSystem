using System;
using System.Collections.Generic;

namespace MessManagementSystem.Models;

public partial class DailyMenu
{
    public int MenuId { get; set; }

    public DateOnly MenuDate { get; set; }

    public string? BreakfastItem { get; set; }

    public string? LunchItem { get; set; }

    public string? DinnerItem { get; set; }

    public decimal DailyRate { get; set; }
}
