namespace backend.dal;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public partial class Booking
{
    public Guid Flight { get; set; }
    public Guid Pax { get; set; }
    public Guid? Promo { get; set; }

    [JsonIgnore]
    public virtual Flight FlightNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual User PaxNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Promo? PromoNavigation { get; set; }
}
