using System;
using System.Collections.Generic;

namespace TurkiyeFinans.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public int CustomerId { get; set; }

    public string AccountType { get; set; } = null!;

    public double Balance { get; set; }

    public string Currency { get; set; } = null!;

    public string OpenDate { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? Iban { get; set; }

    public virtual AccountVadeli? AccountVadeli { get; set; }

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

    public virtual ICollection<CurrencyExchange> CurrencyExchanges { get; set; } = new List<CurrencyExchange>();

    public virtual Currency CurrencyNavigation { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Investment> Investments { get; set; } = new List<Investment>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<Transfer> TransferFromAccounts { get; set; } = new List<Transfer>();

    public virtual ICollection<Transfer> TransferToAccounts { get; set; } = new List<Transfer>();
}
