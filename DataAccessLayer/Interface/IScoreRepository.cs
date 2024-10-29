using DTOs;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IScoreRepository
    {
        Task InsertScores(int userId, int registrationId, List<ScoreDTO> scores);
        

    }
}
