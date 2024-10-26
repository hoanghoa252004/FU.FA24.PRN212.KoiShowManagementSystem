using System;
using System.Collections.Generic;

namespace Entities;

public partial class Show
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateOnly RegisterStartDate { get; set; }

    public DateOnly RegisterEndDate { get; set; }

    public decimal EntranceFee { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Criterion> Criteria { get; set; } = new List<Criterion>();

    public virtual ICollection<RefereeDetail> RefereeDetails { get; set; } = new List<RefereeDetail>();

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
