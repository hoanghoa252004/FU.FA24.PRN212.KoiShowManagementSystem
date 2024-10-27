using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class ShowDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateOnly RegisterStartDate { get; set; }

        public DateOnly RegisterEndDate { get; set; }

        public decimal EntranceFee { get; set; }

        public string Status { get; set; } = null!;

        public virtual IEnumerable<CriterionDTO> Criteria { get; set; } = new List<CriterionDTO>();

        public virtual IEnumerable<VarietyDTO> Varieties { get; set; } = new List<VarietyDTO>();

        public virtual IEnumerable<UserDTO> Referees { get; set; } = new List<UserDTO>();
    }
}
