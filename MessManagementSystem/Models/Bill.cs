using System;
using System.Collections.Generic;

namespace MessManagementSystem.Models;

public partial class Bill
{
    public int BillId { get; set; }

    public int? UserId { get; set; }

    public int BillMonth { get; set; }

    public int BillYear { get; set; }

    public decimal? FoodAmount { get; set; }

    public decimal? WaterAmount { get; set; }

    public decimal? PreviousArrears { get; set; }

    public decimal TotalAmount { get; set; }

    public bool? IsPaid { get; set; }

    public DateTime? GeneratedDate { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User? User { get; set; }
}
