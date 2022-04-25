namespace backend.dal;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

// Modelo de reservación
public partial class Booking
{
    public Guid Flight { get; set; }
    public Guid Pax { get; set; }
    public Guid? Promo { get; set; }

	// Navegación por lazy evaluation
    [JsonIgnore]
    public virtual Flight FlightNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual User PaxNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Promo? PromoNavigation { get; set; }
}
