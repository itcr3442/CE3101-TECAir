using System;
using System.Collections.Generic;

namespace backend
{
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

        public virtual Endpoint Endpoint { get; set; } = null!;
        public virtual ICollection<Bag> Bags { get; set; }
        public virtual ICollection<Promo> Promos { get; set; }
        public virtual ICollection<Segment> Segments { get; set; }
    }
}
