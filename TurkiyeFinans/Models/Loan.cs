using System;
using System.Collections.Generic;

namespace TurkiyeFinans.Models;

public partial class Loan
{
    public int LoanId { get; set; }

    public int CustomerId { get; set; }

    public string LoanType { get; set; } = null!;

    public int Amount { get; set; }

    public double IntersetRate { get; set; }

    public int Term { get; set; }

    public string StartDate { get; set; } = null!;

    public string EndDate { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<LoanPayment> LoanPayments { get; set; } = new List<LoanPayment>();
}
