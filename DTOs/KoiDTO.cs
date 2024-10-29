using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class KoiDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Size { get; set; }
        public byte[] Image { get; set; } = null!;
        public int UserId { get; set; }
        public int VarietyId { get; set; }
        public string VarietyName { get; set; } = null!;
        public bool Status { get; set; }
    }
}
