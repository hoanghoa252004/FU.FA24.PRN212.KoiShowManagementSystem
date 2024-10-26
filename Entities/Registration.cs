using System;
using System.Collections.Generic;

namespace Entities;

public partial class Registration
{
    public int Id { get; set; }

    public DateOnly CreateDate { get; set; }

    public decimal Size { get; set; }

    public string Description { get; set; } = null!;

    public string? Note { get; set; }

    public decimal? TotalScore { get; set; }

    public int? Rank { get; set; }

    public bool IsBestVote { get; set; }

    public int KoiId { get; set; }

    public int ShowId { get; set; }

    public byte[] Image01 { get; set; } = null!;

    public byte[] Image02 { get; set; } = null!;

    public byte[] Image03 { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Koi Koi { get; set; } = null!;

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();

    public virtual Show Show { get; set; } = null!;
}
