using System;
using System.Collections.Generic;

namespace TurkiyeFinans.Models;

public partial class AccountYatirim
{
    public decimal AccountId { get; set; }

    public string? StockName { get; set; }

    public int? Lot { get; set; }

    public virtual Account Account { get; set; } = null!;
}
