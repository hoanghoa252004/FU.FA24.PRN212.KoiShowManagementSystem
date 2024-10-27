using System;
using System.Collections.Generic;

namespace Entities;

public partial class Variety
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Koi> Kois { get; set; } = new List<Koi>();

    public virtual ICollection<Show> Shows { get; set; } = new List<Show>();
}
