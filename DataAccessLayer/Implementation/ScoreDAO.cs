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

    }
}
