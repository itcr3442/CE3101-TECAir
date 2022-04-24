namespace backend.dal;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public partial class Checkin
{
    public Guid Segment { get; set; }
    public Guid Pax { get; set; }
    public int Seat { get; set; }

    [JsonIgnore]
    public virtual User PaxNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Segment SegmentNavigation { get; set; } = null!;
}
