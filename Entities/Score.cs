﻿using System;
using System.Collections.Generic;

namespace Entities;

public partial class Score
{
    public int Id { get; set; }

    public decimal Score1 { get; set; }

    public int RegistrationId { get; set; }

    public int RefereeDetailId { get; set; }

    public int CriteriaId { get; set; }

    public virtual Criterion Criteria { get; set; } = null!;

    public virtual RefereeDetail RefereeDetail { get; set; } = null!;

    public virtual Registration Registration { get; set; } = null!;
}
