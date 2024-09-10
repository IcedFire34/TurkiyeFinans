using System;
using System.Collections.Generic;

namespace TurkiyeFinans.Models;

public partial class AccountYatirim
{
    public int AccountId { get; set; }

    public string StockName { get; set; } = null!;

    public int Lot { get; set; }

    public int CustomerId { get; set; }

    public virtual Account Account { get; set; } = null!;
}
