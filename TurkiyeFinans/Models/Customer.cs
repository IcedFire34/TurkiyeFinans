using System;
using System.Collections.Generic;

namespace TurkiyeFinans.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string DateOfBirth { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string IdentificationNumber { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
