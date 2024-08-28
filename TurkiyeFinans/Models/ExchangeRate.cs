using System;
using System.Collections.Generic;

namespace TurkiyeFinans.Models;

public partial class ExchangeRate
{
    public int RateId { get; set; }

    public string FromCurrency { get; set; } = null!;

    public string ToCurrency { get; set; } = null!;

    public double Rate { get; set; }

    public string LastUpdate { get; set; } = null!;

    public virtual Currency FromCurrencyNavigation { get; set; } = null!;

    public virtual Currency ToCurrencyNavigation { get; set; } = null!;
}
