using System;
using System.Collections.Generic;

namespace TurkiyeFinans.Models;

public partial class Security
{
    public int SecurityId { get; set; }

    public string Symbol { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public double CurrentPrice { get; set; }

    public string LastPriceUpdate { get; set; } = null!;

    public virtual ICollection<InvestmentTransaction> InvestmentTransactions { get; set; } = new List<InvestmentTransaction>();
}
