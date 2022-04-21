using System;
using System.Collections.Generic;

namespace backend
{
    public partial class Endpoint
    {
        public Guid Flight { get; set; }
        public Guid FromLoc { get; set; }
        public Guid ToLoc { get; set; }

        public virtual Flight FlightNavigation { get; set; } = null!;
        public virtual Airport FromLocNavigation { get; set; } = null!;
        public virtual Airport ToLocNavigation { get; set; } = null!;
    }
}
