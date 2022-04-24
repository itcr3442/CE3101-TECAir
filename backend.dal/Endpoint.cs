namespace backend.dal;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public partial class Endpoint
{
    public Guid Flight { get; set; }
    public Guid FromLoc { get; set; }
    public Guid ToLoc { get; set; }

    [JsonIgnore]
    public virtual Flight FlightNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Airport FromLocNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Airport ToLocNavigation { get; set; } = null!;
}
