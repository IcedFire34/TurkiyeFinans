using System;
using System.Collections.Generic;

namespace TurkiyeFinans.Models;

public partial class LoanPayment
{
    public int PaymentId { get; set; }

    public int LoanId { get; set; }

    public double Amount { get; set; }

    public string PaymentDate { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Loan Loan { get; set; } = null!;
}
