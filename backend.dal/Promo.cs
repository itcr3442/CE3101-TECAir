namespace backend.dal;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

// Modelo de promoción
public partial class Promo
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public Guid Flight { get; set; }
    public decimal Price { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public byte[]? Img { get; set; }

	// Navegación por lazy evaluation
    [JsonIgnore]
    public virtual Flight FlightNavigation { get; set; } = null!;
}
