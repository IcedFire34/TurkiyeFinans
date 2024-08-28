using System;
using System.Collections.Generic;

namespace TurkiyeFinans.Models;

public partial class Currency
{
    public string CurrencyId { get; set; } = null!;

    public double Value { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<CurrencyExchange> CurrencyExchangeFromCurrencyNavigations { get; set; } = new List<CurrencyExchange>();

    public virtual ICollection<CurrencyExchange> CurrencyExchangeToCurrencyNavigations { get; set; } = new List<CurrencyExchange>();

    public virtual ICollection<ExchangeRate> ExchangeRateFromCurrencyNavigations { get; set; } = new List<ExchangeRate>();

    public virtual ICollection<ExchangeRate> ExchangeRateToCurrencyNavigations { get; set; } = new List<ExchangeRate>();

    public virtual ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();
}
