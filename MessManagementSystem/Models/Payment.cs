using System;
using System.Collections.Generic;

namespace MessManagementSystem.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? UserId { get; set; }

    public int? BillId { get; set; }

    public decimal AmountPaid { get; set; }

    public string TransactionToken { get; set; } = null!;

    public DateTime? PaymentDate { get; set; }

    public virtual Bill? Bill { get; set; }

    public virtual User? User { get; set; }
}
