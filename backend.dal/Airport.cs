namespace backend.dal;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public partial class Airport
{
    public Airport()
    {
        EndpointFromLocNavigations = new HashSet<Endpoint>();
        EndpointToLocNavigations = new HashSet<Endpoint>();
        SegmentFromLocNavigations = new HashSet<Segment>();
        SegmentToLocNavigations = new HashSet<Segment>();
    }

    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string? Comment { get; set; }

    [JsonIgnore]
    public virtual ICollection<Endpoint> EndpointFromLocNavigations { get; set; }
    [JsonIgnore]
    public virtual ICollection<Endpoint> EndpointToLocNavigations { get; set; }
    [JsonIgnore]
    public virtual ICollection<Segment> SegmentFromLocNavigations { get; set; }
    [JsonIgnore]
    public virtual ICollection<Segment> SegmentToLocNavigations { get; set; }
}
