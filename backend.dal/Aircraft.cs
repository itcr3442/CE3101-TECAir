namespace backend.dal;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public partial class Aircraft
{
    public Aircraft()
    {
        Segments = new HashSet<Segment>();
    }

    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public int Seats { get; set; }

    [JsonIgnore]
    public virtual ICollection<Segment> Segments { get; set; }
}
