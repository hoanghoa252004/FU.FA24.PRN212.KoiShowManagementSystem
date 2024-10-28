using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class RegistrationDTO
    {
        public int Id { get; set; }

        public DateOnly CreateDate { get; set; }

        public decimal Size { get; set; }

        public string Description { get; set; } = null!;

        public string? KoiName { get; set; }
        public string? VarietyName { get; set; }

        public decimal? TotalScore { get; set; }

        public int? Rank { get; set; }

        public int KoiId { get; set; }

        public int ShowId { get; set; }

        public byte[] Image01 { get; set; } = null!;

        public byte[] Image02 { get; set; } = null!;

        public byte[] Image03 { get; set; } = null!;

        public string ShowStatus { get; set; } = null!;
        public virtual KoiDTO Koi { get; set; } = null!;

        public virtual ICollection<ScoreDTO> Scores { get; set; } = new List<ScoreDTO>();

        public virtual ShowDTO Show { get; set; } = null!;
    }
}
