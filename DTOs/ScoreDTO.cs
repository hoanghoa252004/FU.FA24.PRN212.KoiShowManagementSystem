using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class ScoreDTO
    {
        public int Id { get; set; }

        public decimal Score1 { get; set; }
        public string? CriteriaName { get; set; }
        public int Percentage { get; set; }
        public int RegistrationId { get; set; }

        public int RefereeDetailId { get; set; }

        public int CriteriaId { get; set; }
        public decimal TotalScore1 { get; set; }

    }
}
