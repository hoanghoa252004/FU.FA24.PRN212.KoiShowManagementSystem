using DataAccessLayer.Interface;
using DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class CriterionDAO :ICriterionRepository
    {
        private static readonly CriterionDAO _instance;
        private CriterionDAO() { }
        static CriterionDAO()
        {
            _instance = new CriterionDAO();
        }
        public static CriterionDAO Instance => _instance;

        public async Task<IEnumerable<CriterionDTO>> GetCriterionByShow(int showId)
        {
            using (var context = new Prn212ProjectKoiShowManagementContext())
            {
                var result = await context.Criteria
                    .Where(c => c.ShowId == showId)
                    .Select(c => new CriterionDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Percentage = c.Percentage,
                }).ToListAsync();
                return result;
            }
        }
    }
}
