using System;
using System.Collections.Generic;

namespace Entities;

public partial class Criterion
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Percentage { get; set; }

    public int ShowId { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();

    public virtual Show Show { get; set; } = null!;
}
