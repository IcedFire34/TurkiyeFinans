using System;
using System.Collections.Generic;

namespace TurkiyeFinans.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public decimal AccountId { get; set; }

    public string TransactionType { get; set; } = null!;

    public double Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string TransactionDate { get; set; } = null!;

    public string? Description { get; set; }

    public virtual Account Account { get; set; } = null!;
}
