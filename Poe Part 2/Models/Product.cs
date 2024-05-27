using System;
using System.Collections.Generic;

namespace Poe_Part_2.Models;

public partial class Product
{
    public string Username { get; set; } = null!;

    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int CategoryId { get; set; }

    public DateOnly ProductionDate { get; set; }

    public string Info { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual User UsernameNavigation { get; set; } = null!;
}
