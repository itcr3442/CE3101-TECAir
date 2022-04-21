using System;
using System.Collections.Generic;

namespace backend
{
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

        public virtual Aircraft AircraftNavigation { get; set; } = null!;
        public virtual Flight FlightNavigation { get; set; } = null!;
        public virtual Airport FromLocNavigation { get; set; } = null!;
        public virtual Airport ToLocNavigation { get; set; } = null!;
    }
}
