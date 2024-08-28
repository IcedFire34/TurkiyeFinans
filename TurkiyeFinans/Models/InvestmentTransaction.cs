using System;
using System.Collections.Generic;

namespace TurkiyeFinans.Models;

public partial class InvestmentTransaction
{
    public int InvestmentTransactionId { get; set; }

    public int InvestmentId { get; set; }

    public int SecurityId { get; set; }

    public string TransactionType { get; set; } = null!;

    public double Quantity { get; set; }

    public double Price { get; set; }

    public string TransactionDate { get; set; } = null!;

    public virtual Investment Investment { get; set; } = null!;

    public virtual Security Security { get; set; } = null!;
}
