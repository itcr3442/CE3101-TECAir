using System;
using System.Collections.Generic;

namespace backend
{
    public partial class Checkin
    {
        public Guid Segment { get; set; }
        public Guid Pax { get; set; }
        public int Seat { get; set; }

        public virtual User PaxNavigation { get; set; } = null!;
        public virtual Segment SegmentNavigation { get; set; } = null!;
    }
}
