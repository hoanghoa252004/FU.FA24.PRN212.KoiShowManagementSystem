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
    public class RegistrationDAO : IRegistrationRepository
    {
        private static readonly RegistrationDAO _instance;
        private RegistrationDAO() { }
        static RegistrationDAO()
        {
            _instance = new RegistrationDAO();
        }
        public static RegistrationDAO Instance => _instance;


        //Tại một thời điểm chí có một show đang diễn ra nên chỉ lấy ra show đang có trạng thái là scoring 
        public async Task<IEnumerable<RegistrationDTO>> GetRegistrationsByReferee(int userId)
        {
            using (var context = new Prn212ProjectKoiShowManagementContext())
            {
                var ongoingShow = await context.Shows
                    .Include(s => s.Criteria) // Include criteria của show
                    .FirstOrDefaultAsync(s => s.Status == "Scoring");

                if (ongoingShow == null)
                {
                    return new List<RegistrationDTO>();
                }

                var result = await context.Registrations
                    .Include(r => r.Koi)
                        .ThenInclude(k => k.Variety)
                    .Include(r => r.Scores
                        .Where(s => s.RefereeDetail.UserId == userId)) // Chỉ lấy điểm của referee hiện tại
                    .Include(r => r.Show)
                        .ThenInclude(s => s.Criteria)
                    .Where(r => r.ShowId == ongoingShow.Id
                        && r.Show.RefereeDetails.Any(rd => rd.UserId == userId))
                    .Select(r => new RegistrationDTO
                    {
                        Id = r.Id,
                        KoiName = r.Koi.Name,
                        VarietyName = r.Koi.Variety.Name,
                        Size = r.Size,
                        Description = r.Description,
                        Image01 = r.Image01,
                        Image02 = r.Image02,
                        Image03 = r.Image03,
                        ShowId = r.ShowId,
                        Scores = r.Show.Criteria.Select(c => new ScoreDTO
                        {
                            CriteriaId = c.Id,
                            CriteriaName = c.Name,
                            RegistrationId = r.Id,
                            //Score1 = r.Scores
                            //    .FirstOrDefault(s => s.CriteriaId == c.Id &&
                            //        s.RefereeDetail.UserId == userId) ,
                            //RefereeDetailId = r.Show.RefereeDetails
                            //    .FirstOrDefault(rd => rd.UserId == userId).Id 
                        }).ToList()
                    })
                    .ToListAsync();

                return result;
            }
        }




    }
}
