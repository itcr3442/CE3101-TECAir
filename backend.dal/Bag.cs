namespace backend.dal;

using System;
using System.Collections.Generic;

public partial class Bag
{
    public Guid Id { get; set; }
    public Guid Owner { get; set; }
    public Guid Flight { get; set; }
    public int No { get; set; }
    public decimal Weight { get; set; }
    public string Color { get; set; } = null!;

    public virtual Flight FlightNavigation { get; set; } = null!;
    public virtual User OwnerNavigation { get; set; } = null!;
}
