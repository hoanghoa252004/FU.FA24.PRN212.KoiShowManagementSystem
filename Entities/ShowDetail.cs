using System;
using System.Collections.Generic;

namespace Entities;

public partial class ShowDetail
{
    public int ShowId { get; set; }

    public int VarietyId { get; set; }

    public virtual Show Show { get; set; } = null!;

    public virtual Variety Variety { get; set; } = null!;
}
