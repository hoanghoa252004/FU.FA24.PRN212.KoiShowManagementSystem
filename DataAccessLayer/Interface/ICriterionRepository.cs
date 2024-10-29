using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface ICriterionRepository
    {
        Task<IEnumerable<CriterionDTO>> GetCriterionByShow(int showId);
    }
}
