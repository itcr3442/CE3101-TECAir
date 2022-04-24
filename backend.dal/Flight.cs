namespace backend.dal;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public partial class Flight
{
    public Flight()
    {
        Bags = new HashSet<Bag>();
        Promos = new HashSet<Promo>();
        Segments = new HashSet<Segment>();
    }

    public Guid Id { get; set; }
    public int No { get; set; }
    public string? Comment { get; set; }
    public decimal Price { get; set; }
    public FlightState State { get; set; }

    [JsonIgnore]
    public virtual ICollection<Bag> Bags { get; set; }
    [JsonIgnore]
    public virtual ICollection<Promo> Promos { get; set; }
    [JsonIgnore]
    public virtual ICollection<Segment> Segments { get; set; }
}
