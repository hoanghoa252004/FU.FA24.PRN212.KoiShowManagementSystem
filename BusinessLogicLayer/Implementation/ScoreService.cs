using BusinessLogicLayer.Interface;
using DataAccessLayer;
using DataAccessLayer.Interface;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Implementation
{
    public class ScoreService : IScoreService
    {
        private static readonly ScoreService _instance;
        private readonly Repository _repository;
        private ScoreService()
        {
            _repository = Repository.Instance;
        }
        static ScoreService()
        {
            _instance = new ScoreService();
        }
        public static ScoreService Instance => _instance;

        public async Task InsertScores(int userId, int registrationId, List<ScoreDTO> scores)
        {
            await _repository.Score.InsertScores(userId, registrationId, scores);
        }
    }
}
