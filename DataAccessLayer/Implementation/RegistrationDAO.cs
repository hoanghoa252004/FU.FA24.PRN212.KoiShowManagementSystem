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
    public class RegistrationDAO : IRegistrationRepository
    {
        private static readonly RegistrationDAO _instance;
        private RegistrationDAO() { }
        static RegistrationDAO()
        {
            _instance = new RegistrationDAO();
        }
        public static RegistrationDAO Instance => _instance;

        public async Task<bool> Add(RegistrationDTO dto)
        {
            bool result = false;
            if (dto != null)
            {
                using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
                {
                    var koi = await _context.Kois.FirstOrDefaultAsync(k => k.Id == dto.KoiId);
                    if (koi != null)
                    {
                        _context.Registrations.Add(new Registration()
                        {
                            CreateDate = DateOnly.FromDateTime(DateTime.Now),
                            KoiId = (int)dto.KoiId!,
                            Description = koi.Description!,
                            Size = (int)koi.Size!,
                            ShowId = dto.ShowId,
                            Image01 = dto.Image01!,
                            Image02 = dto.Image02!,
                            Image03 = dto.Image03!,
                        });
                        await _context.SaveChangesAsync();
                        result = true;
                    }
                }
            }
            return result;
        }

        public async Task<bool> Delete(int registrationId)
        {
            bool result = false;
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                var registration = await _context.Registrations.SingleOrDefaultAsync(r => r.Id == registrationId);
                if (registration != null)
                {
                    _context.Registrations.Remove(registration);
                    await _context.SaveChangesAsync();
                    result = true;
                }
            }
            return result;
        }

        public async Task<IEnumerable<RegistrationDTO>> GetAll()
        {
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                var result = await _context.Registrations
                    .Include(r => r.Koi)
                        .ThenInclude(k => k.Variety)
                    .Include(r => r.Show)
                .Select(registration => new RegistrationDTO()
                {
                    CreateDate = registration.CreateDate,
                    KoiId = registration.KoiId,
                    Description = registration.Description,
                    Size = registration.Size,
                    ShowId = registration.ShowId,
                    ShowName = registration.Show.Title,
                    Image01 = registration.Image01,
                    Image02 = registration.Image02,
                    Image03 = registration.Image03,
                    IsBestVote = registration.IsBestVote,
                    Note = registration.Note,
                    Rank = registration.Rank,
                    Status = registration.Status,
                    TotalScore = registration.TotalScore,
                    KoiName = registration.Koi.Name,
                    Id = registration.Id,
                    KoiVariety = registration.Koi.Variety.Name,
                })
                .OrderBy(r => r.Id)
                .ToListAsync();
                return result;
            }
        }

        public async Task<RegistrationDTO> GetById(int registrationId)
        {
            RegistrationDTO result = null!;
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                var registration =  await _context.Registrations
                    .Include(r => r.Koi)
                        .ThenInclude(k => k.Variety)
                    .Include(r => r.Show)
                    .SingleOrDefaultAsync(r => r.Id == registrationId);
                if(registration != null)
                {
                    result = new RegistrationDTO()
                    {
                        Id = registration.Id,
                        CreateDate = registration.CreateDate,
                        Description = registration.Description,
                        Image01 = registration.Image01,
                        Image02 = registration.Image02,
                        Image03 = registration.Image03,
                        IsBestVote = registration.IsBestVote,
                        Note = registration.Note,
                        Rank = registration.Rank,
                        Status = registration.Status,
                        TotalScore = registration.TotalScore,
                        KoiId = registration.Koi.Id,
                        KoiName = registration.Koi.Name,
                        KoiVariety = registration.Koi.Variety.Name,
                        ShowId = registration.ShowId,
                        ShowName = registration.Show.Title,
                        Size = registration.Size,
                    };
                }
            }
            return result;
        }

        public async Task<bool> Update(RegistrationDTO dto)
        {
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                bool result = false;
                var registration = await _context.Registrations.SingleOrDefaultAsync(r => r.Id == dto.Id);
                if(registration != null)
                {
                    if (dto.Note != null)
                    {
                        registration.Note = dto.Note;
                    }
                    if(dto.KoiId != null)
                    {
                        registration.KoiId = (int)dto.KoiId;
                    }
                    if(dto.Size != null)
                    {
                        registration.Size = (int)dto.Size;
                    }
                    if (dto.Description != null)
                    {
                        registration.Description = dto.Description;
                    }
                    if (dto.Status != null)
                    {
                        registration.Status = dto.Status;
                    }
                    if (dto.Image01 != null)
                    {
                        registration.Image01 = dto.Image01;
                    }
                    if (dto.Image02 != null)
                    {
                        registration.Image02 = dto.Image02;
                    }
                    if (dto.Image03 != null)
                    {
                        registration.Image03 = dto.Image03;
                    }
                    await _context.SaveChangesAsync();
                    result = true;
                }
                return result;
            }
        }

        public async Task CalculateTotalScoreAllForRegistation(int showId)
        {
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                var registrations = await _context.Registrations
                    .Where(r => r.ShowId == showId && r.Status.Equals("Accepted") && r.TotalScore == null)
                    .Include(r => r.Scores)
                        .ThenInclude(score => score.Criteria)
                        .ToListAsync();
                foreach(var registration in registrations)
                {
                    decimal totalScore = _context.Scores
                        .Where(s => s.RegistrationId == registration.Id)
                        .GroupBy(s => s.CriteriaId)
                        .Select(group => new // IGroup: bao gồm Key ( tiêu chí gom ) và IEnumerable<kiểu dữ liệu của các records>
                        {
                            CriteriaId = group.Key,
                            WeightedAverage = group.Average(s => s.Score1) * group.First().Criteria.Percentage / 100,
                        })
                        .Sum(x => x.WeightedAverage);
                    registration.TotalScore = totalScore;
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task Ranking(int showId)
        {
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                var registrations = await _context.Registrations
                    .Where(r => r.ShowId == showId && r.Status.Equals("Accepted") && r.TotalScore != null)
                    .Include(r => r.Scores)
                        .ThenInclude(score => score.Criteria)
                    .Select(r => new
                    {
                        Registration = r,
                        ScoreList = _context.Scores
                            .Where(s => s.RegistrationId == r.Id)
                            .GroupBy(s => s.CriteriaId)
                            .Select(group => new // IGroup: bao gồm Key ( tiêu chí gom ) và IEnumerable<kiểu dữ liệu của các records>.
                            {
                                CriteriaId = group.Key,
                                TotalScoreByCriteria = group.Average(s => s.Score1) * group.First().Criteria.Percentage / 100,
                                Percentage = group.First().Criteria.Percentage,
                            })
                            .OrderByDescending(x => x.Percentage).ToList(), // Chuyển về dạng list để xác định index,
                                                                            // sort tiêu chí trọng số lớn lên ưu tiên
                    })
                    .OrderByDescending(x => x.ScoreList.Sum(sl => sl.TotalScoreByCriteria))
                    .ThenByDescending(x => x.ScoreList[1])
                    .ThenByDescending(x => x.ScoreList[1])
                    .ThenByDescending(x => x.ScoreList[1])
                    .ToListAsync();
            }
        }

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
                        && r.Show.RefereeDetails.Any(rd => rd.UserId == userId) && r.Status.Equals("Accepted"))
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
                            //Score1 = r.Scores.FirstOrDefault(s => s.CriteriaId == c.Id)?.Score1,
                            TotalScore1 = Math.Round(r.Scores
                                .Where(s => s.CriteriaId == c.Id)
                                .Select(s => s.Score1 * c.Percentage / 100).FirstOrDefault(), 1),
                            Score1 = r.Scores.Where(s => s.CriteriaId == c.Id && s.RegistrationId == r.Id).Select(s => s.Score1).FirstOrDefault(),

                        }).ToList()
                    })
                    .OrderBy(r => r.Id)
                    .ToListAsync();

                return result;
            }
        }

        public async Task<IEnumerable<RegistrationDTO>> GetRegistrationsByMember(int userId)
        {
            IEnumerable<RegistrationDTO> result = null!;
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                result = await _context.Registrations
                    .Include(r => r.Koi)
                        .ThenInclude(k => k.User)
                    .Include(r => r.Koi)
                        .ThenInclude(k => k.Variety)
                    .Include(r => r.Show)
                    .Where(r => r.Koi.User.Id == userId)
                    .Select(registration => new RegistrationDTO()
                    {
                        Id = registration.Id,
                        CreateDate = registration.CreateDate,
                        Description = registration.Description,
                        Image01 = registration.Image01,
                        Image02 = registration.Image02,
                        Image03 = registration.Image03,
                        IsBestVote = registration.IsBestVote,
                        Note = registration.Note,
                        Rank = registration.Rank,
                        Status = registration.Status,
                        TotalScore = registration.TotalScore,
                        KoiId = registration.Koi.Id,
                        KoiName = registration.Koi.Name,
                        KoiVariety = registration.Koi.Variety.Name,
                        ShowId = registration.ShowId,
                        ShowName = registration.Show.Title,
                        Size = registration.Size,
                        MemberId = registration.Koi.User.Id,
                    })
                    .OrderBy(r => r.Id)
                    .ToListAsync();
            }
            return result;
        }

        public async Task<IEnumerable<RegistrationDTO>> GetRegistrationsByShow(int showId)
        {
            IEnumerable<RegistrationDTO> result = null!;
            using (Prn212ProjectKoiShowManagementContext _context = new Prn212ProjectKoiShowManagementContext())
            {
                result = await _context.Registrations
                    .Where(r => r.ShowId == showId)
                    .Include(r => r.Koi)
                        .ThenInclude(k => k.User)
                    .Include(r => r.Koi)
                        .ThenInclude(k => k.Variety)
                    .Include(r => r.Show)
                    .Select(registration => new RegistrationDTO()
                    {
                        Id = registration.Id,
                        CreateDate = registration.CreateDate,
                        Description = registration.Description,
                        Image01 = registration.Image01,
                        Image02 = registration.Image02,
                        Image03 = registration.Image03,
                        IsBestVote = registration.IsBestVote,
                        Note = registration.Note,
                        Rank = registration.Rank,
                        Status = registration.Status,
                        TotalScore = registration.TotalScore,
                        KoiId = registration.Koi.Id,
                        KoiName = registration.Koi.Name,
                        KoiVariety = registration.Koi.Variety.Name,
                        ShowId = registration.ShowId,
                        ShowName = registration.Show.Title,
                        Size = registration.Size,
                        MemberId = registration.Koi.User.Id,
                    })
                    .OrderBy(r => r.Id)
                    .ToListAsync();
            }
            return result;
        }
    }
}
