using System;
using System.Collections.Generic;

namespace Fitness_Center.Models;

public partial class Bankaccount
{
    public decimal BankAccountId { get; set; }

    public decimal MemberId { get; set; }

    public string CardNumber { get; set; } = null!;

    public string CardHolderName { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public string Cvv { get; set; } = null!;

    public decimal Balance { get; set; }

    public virtual Member Member { get; set; } = null!;
}
