using System;
using System.Collections.Generic;

namespace TurkiyeFinans.Models;

public partial class Transfer
{
    public int TransferId { get; set; }

    public decimal FromAccountId { get; set; }

    public decimal ToAccountId { get; set; }

    public double Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string TransferDate { get; set; } = null!;

    public double FromBalance { get; set; }

    public virtual Currency CurrencyNavigation { get; set; } = null!;

    public virtual Account FromAccount { get; set; } = null!;

    public virtual Account ToAccount { get; set; } = null!;
}
