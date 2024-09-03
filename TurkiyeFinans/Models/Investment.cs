using System;
using System.Collections.Generic;

namespace TurkiyeFinans.Models;

public partial class Investment
{
    public int InvestmentId { get; set; }

    public string AccountId { get; set; } = null!;

    public string InvestmentType { get; set; } = null!;

    public double Amount { get; set; }

    public string PurchaseDate { get; set; } = null!;

    public double CurrentValue { get; set; }

    public string LastValueUpdate { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<InvestmentTransaction> InvestmentTransactions { get; set; } = new List<InvestmentTransaction>();
}
