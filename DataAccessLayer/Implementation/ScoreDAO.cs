using DataAccessLayer.Interface;
using DTOs;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class ScoreDAO : IScoreRepository
    {
        private static readonly ScoreDAO _instance;
        private ScoreDAO() { }
        static ScoreDAO()
        {
            _instance = new ScoreDAO();
        }
        public static ScoreDAO Instance => _instance;

        public async Task InsertScores(int userId, int registrationId, List<ScoreDTO> scores)
        {
            using (var context = new Prn212ProjectKoiShowManagementContext())
            {
                var showId = context.Shows.Where(s => s.Status.ToLower()!.Equals("scoring")).Single().Id;
                var refereeId = await context.RefereeDetails.Where(r => r.UserId == userId && r.ShowId == showId).FirstOrDefaultAsync();
                foreach (var scoreDto in scores)
                {
                    var existingScore = await context.Scores
                        .FirstOrDefaultAsync(s => s.RegistrationId == registrationId
                                                  && s.RefereeDetailId == refereeId!.Id
                                                  && s.CriteriaId == scoreDto.CriteriaId);

                    if (existingScore != null)
                    {
                        existingScore.Score1 = scoreDto.Score1;
                    }
                    else
                    {
                        var newScore = new Score
                        {
                            Score1 = scoreDto.Score1,
                            RegistrationId = registrationId,
                            RefereeDetailId = refereeId!.Id,
                            CriteriaId = scoreDto.CriteriaId
                        };
                        context.Scores.Add(newScore);
                    }
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsScoreCompletely(int showId)
        {
            bool result = false;
            using (var _context = new Prn212ProjectKoiShowManagementContext())
            {
                // Lấy đơn được chấm điểm ( status = Accepted )
                var registrationInShow = await _context.Registrations
                    .Include(r => r.Scores)
                    .Where(r => r.ShowId == showId
                            && (r.Status.Equals("Accepted") || r.Status.Equals("Scored"))).ToListAsync();
                if (registrationInShow != null && registrationInShow.Any() == true)
                {
                    int totalRegistration = registrationInShow.Count();
                    int totalCriteria = _context.Criteria.Where(cr => cr.ShowId == showId).Count();
                    int totalReferee = _context.RefereeDetails.Where(rd => rd.ShowId == showId).Count();
                    int totalScoreRecords = totalCriteria * totalRegistration * totalReferee;
                    int scoringRecords = _context.Scores
                        .Include(sc => sc.Criteria)
                        .Where(sc => sc.Criteria.ShowId == showId).Count();
                    if(totalScoreRecords == scoringRecords)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
    }
}
