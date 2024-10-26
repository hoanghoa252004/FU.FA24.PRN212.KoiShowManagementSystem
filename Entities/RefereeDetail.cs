using System;
using System.Collections.Generic;

namespace Entities;

public partial class RefereeDetail
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ShowId { get; set; }

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();

    public virtual Show Show { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
