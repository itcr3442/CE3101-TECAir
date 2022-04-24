namespace backend.dal;

using System;
using System.Collections.Generic;

public partial class Booking
{
    public Guid Flight { get; set; }
    public Guid Pax { get; set; }
    public Guid? Promo { get; set; }

    public virtual Flight FlightNavigation { get; set; } = null!;
    public virtual User PaxNavigation { get; set; } = null!;
    public virtual Promo? PromoNavigation { get; set; }
}
