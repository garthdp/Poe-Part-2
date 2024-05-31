using System;
using System.Collections.Generic;

namespace Poe_Part_2.Models;

// User class
public partial class User
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string AccountType { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
