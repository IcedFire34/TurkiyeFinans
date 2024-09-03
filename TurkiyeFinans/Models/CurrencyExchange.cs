using System;
using System.Collections.Generic;

namespace TurkiyeFinans.Models;

public partial class CurrencyExchange
{
    public int ExchangeId { get; set; }

    public string AccountId { get; set; } = null!;

    public string FromCurrency { get; set; } = null!;

    public string ToCurrency { get; set; } = null!;

    public double Amount { get; set; }

    public double ExchangeRate { get; set; }

    public string ExchangeDate { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;

    public virtual Currency FromCurrencyNavigation { get; set; } = null!;

    public virtual Currency ToCurrencyNavigation { get; set; } = null!;
}
