namespace backend.dal;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

// Modelo de segmento
public partial class Segment
{
    public Guid Id { get; set; }
    public Guid Flight { get; set; }
    public int SeqNo { get; set; }
    public Guid FromLoc { get; set; }
    public DateTimeOffset FromTime { get; set; }
    public Guid ToLoc { get; set; }
    public DateTimeOffset ToTime { get; set; }
    public Guid Aircraft { get; set; }

	// Navegación por lazy evaluation
    [JsonIgnore]
    public virtual Aircraft AircraftNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Flight FlightNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Airport FromLocNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Airport ToLocNavigation { get; set; } = null!;
}
