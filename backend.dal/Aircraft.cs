namespace backend.dal;

using System;
using System.Collections.Generic;

public partial class Aircraft
{
    public Aircraft()
    {
        Segments = new HashSet<Segment>();
    }

    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public int Seats { get; set; }

    public virtual ICollection<Segment> Segments { get; set; }
}
