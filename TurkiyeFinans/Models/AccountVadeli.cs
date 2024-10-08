﻿using System;
using System.Collections.Generic;

namespace TurkiyeFinans.Models;

public partial class AccountVadeli
{
    public decimal AccountId { get; set; }

    public int Vade { get; set; }

    public virtual Account Account { get; set; } = null!;
}
