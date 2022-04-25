namespace backend.dal;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

// Modelo de aeronave
public partial class Aircraft
{
    public Aircraft()
    {
        Segments = new HashSet<Segment>();
    }

    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public int Seats { get; set; }

	// navegación por lazy evaluation
    [JsonIgnore]
    public virtual ICollection<Segment> Segments { get; set; }
}
