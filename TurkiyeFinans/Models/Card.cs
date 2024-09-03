using System;
using System.Collections.Generic;

namespace TurkiyeFinans.Models;

public partial class Card
{
    public int CardId { get; set; }

    public string AccountId { get; set; } = null!;

    public string CardType { get; set; } = null!;

    public string CardNumber { get; set; } = null!;

    public string ExpiryDate { get; set; } = null!;

    public string Cvv { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;
}
